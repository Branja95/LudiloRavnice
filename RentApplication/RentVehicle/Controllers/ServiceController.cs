﻿using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using AccountManaging.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using RentVehicle.Helpers;
using RentVehicle.Hubs;
using RentVehicle.Models.Entities;
using RentVehicle.Models.IdentityUsers;
using RentVehicle.Persistance.UnitOfWork;
using static RentVehicle.Models.Bindings.ServiceBindingModel;

namespace RentVehicle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : ControllerBase
    {
        private static object lockObjectForServices = new object();

        private static readonly string folderPath = @"App_Data\rent-vehicle-service\";

        private readonly string _isUserInRoleEndpoint;
        private readonly string _findUserEndpoint;
        private readonly string _isManagerBannedEndpoint;
        private readonly string _deleteCommentsEndpoint;
        private readonly string _deleteRatingsEndpoint;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly IHubContext<NotificationHub> _hubContext;

        private readonly IHostingEnvironment _environment;
        private readonly IConfiguration _configuration;

        public ServiceController(IUnitOfWork unitOfWork,
           IEmailService emailService,
            IHubContext<NotificationHub> hubContext,
           IHostingEnvironment environment,
           IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _hubContext = hubContext;
            _environment = environment;
            _configuration = configuration;
            _isUserInRoleEndpoint = configuration["AccountService:IsUserInRoleEndpoint"];
            _findUserEndpoint = configuration["AccountService:FindUserEndpoint"];
            _isManagerBannedEndpoint = configuration["AccountService:IsManagerBannedEndpoint"];
            _deleteCommentsEndpoint = configuration["BookingService:DeleteCommentsForServiceEndpoint"];
            _deleteRatingsEndpoint = configuration["BookingService:DeleteRatingsForServiceEndpoint"];
        }


        [HttpGet]
        [Route("GetService")]
        [AllowAnonymous]
        public IActionResult GetService([FromQuery] long serviceId)
        {
            Service service = _unitOfWork.Services.Get(serviceId);
            if (service == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(service);
            }
        }


        [HttpGet]
        [Route("GetServices")]
        [AllowAnonymous]
        public IActionResult GetServices()
        {
            return Ok(_unitOfWork.Services.Find(service => service.IsApproved));
        }


        [HttpGet]
        [Route("LoadImage")]
        [AllowAnonymous]
        public IActionResult LoadImage([FromQuery] string imageId)
        {
            string filePath = Path.Combine(_environment.WebRootPath, folderPath, imageId);
            string fileExtension = Path.GetExtension(filePath);

            Stream readStream = ImageHelper.ReadImageFromServer(_environment.WebRootPath, folderPath, imageId);
            return File(readStream, "image/" + fileExtension);
        }


        [HttpGet]
        [Route("GetBranchOffices")]
        [AllowAnonymous]
        public IActionResult GetBranchOffices([FromQuery] long serviceId)
        {
            Service service = _unitOfWork.Services.Get(serviceId);
            if (service == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(service.BranchOfficces);
            }
        }


        [HttpGet]
        [Route("GetVehicles")]
        [AllowAnonymous]
        public IActionResult GetVehicles([FromQuery] long serviceId)
        {
            Service service = _unitOfWork.Services.Get(serviceId);
            if (service == null)
            {
                return BadRequest();
            }
            else
            {
                return Ok(service.Vehicles);
            }
        }


        [HttpGet]
        [Route("ServicesForApproval")]
        [Authorize(Roles = "Administrator")]
        public IActionResult ServicesForApproval()
        {
            return Ok(_unitOfWork.Services.Find(service => !service.IsApproved));
        }

        [HttpGet]
        [Route("ServicesForApprovalCount")]
        [Authorize(Roles = "Administrator")]
        public IActionResult ServicesForApprovalCount()
        {
            return Ok(_unitOfWork.Services.Find(service => !service.IsApproved).Count());
        }

        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("ApproveService")]
        public async Task<IActionResult> ApproveService([FromBody] long serviceId)
        {
            if (_unitOfWork.Services.Get(serviceId) == null)
            {
                return BadRequest();
            }
            else
            {
                ServiceForApproval serviceForApproval = _unitOfWork.ServicesForApproval.Get(serviceId);
                Service service = _unitOfWork.Services.Get(serviceId);
                service.IsApproved = true;

                ApplicationUser user = await FindUser();
                _emailService.SendMail("Service approved", "Your service " + service.Name + " is approved, now you can add branch offices and vehicles.", service.EmailAddress);

                try
                {
                    lock (lockObjectForServices)
                    {
                        _unitOfWork.Services.Update(service);
                        _unitOfWork.ServicesForApproval.Remove(serviceForApproval);
                        _unitOfWork.Complete();
                    }
                }
                catch (DBConcurrencyException)
                {
                    return NotFound();
                }

                await _hubContext.Clients.All.SendAsync("newRentAVehicleServiceToApprove", _unitOfWork.ServicesForApproval.Count());
                return Ok();
            }
        }


        [HttpPost]
        [Authorize(Roles = "Administrator")]
        [Route("RejectService")]
        public async Task<IActionResult> RejectService([FromBody] long serviceId)
        {
            if (_unitOfWork.Services.Get(serviceId) == null)
            {
                return BadRequest();
            }
            else
            {
                ServiceForApproval serviceForApproval = _unitOfWork.ServicesForApproval.Get(serviceId);

                ApplicationUser user = await FindUser();
                _emailService.SendMail("Service rejected", "Your service " + serviceForApproval.Service.Name + " is rejected.", user.Email);
                try
                {
                    lock (lockObjectForServices)
                    {
                        _unitOfWork.ServicesForApproval.Remove(serviceForApproval);
                        _unitOfWork.Complete();
                    }
                }
                catch (DBConcurrencyException)
                {
                    return NotFound();
                }

                await _hubContext.Clients.All.SendAsync("newRentAVehicleServiceToApprove", _unitOfWork.ServicesForApproval.Count());
                return Ok();
            }
        }


        [HttpPost]
        [Route("PostService")]
        [Authorize(Roles = "Administrator, Manager")]
        public async Task<IActionResult> PostService([FromForm] CreateRentVehicleServiceBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                ApplicationUser user = await FindUser();
                if(user == null)
                {
                    return BadRequest();
                }
                else
                {
                    if (IsUserInRole("Manager").Result)
                    {
                        using (HttpClient httpClient = new HttpClient())
                        {
                            HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(_isManagerBannedEndpoint + User.FindFirstValue(ClaimTypes.NameIdentifier)).ConfigureAwait(true);
                            if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                            {
                                bool isBanned = await httpResponseMessage.Content.ReadAsAsync<bool>().ConfigureAwait(false);
                                if (isBanned)
                                {
                                    return Ok("You are banned!");
                                }
                            }
                            else
                            {
                                return BadRequest();
                            }
                        }
                    }
                    ImageHelper imageHelper = new ImageHelper();
                    string fileName = await imageHelper.UploadImageToServer(_environment.WebRootPath, folderPath, model.Image);
                    Service service = new Service()
                    {
                        Creator = user.Id,
                        Name = model.Name,
                        EmailAddress = model.ContactEmail,
                        Description = model.Description,
                        LogoImage = fileName,
                        IsApproved = false
                    };

                    ServiceForApproval serviceForApproval = new ServiceForApproval
                    {
                        Service = service
                    };

                    try
                    {
                        lock (lockObjectForServices)
                        {
                            _unitOfWork.Services.Add(service);
                            _unitOfWork.ServicesForApproval.Add(serviceForApproval);
                            _unitOfWork.Complete();
                        }
                    }
                    catch (DBConcurrencyException)
                    {
                        return NotFound();
                    }

                    await _hubContext.Clients.All.SendAsync("newRentAVehicleServiceToApprove", _unitOfWork.ServicesForApproval.Count());

                    return Ok();
                }
            }
        }


        [HttpPut]
        [Route("PutService")]
        [Authorize(Roles = "Administrator, Manager")]
        public async Task<IActionResult> PutService([FromForm] EditRentVehicleServiceBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                if (model.ServiceId != model.Id)
                {
                    return BadRequest();
                }

                Service serviceForValidation = _unitOfWork.Services.Get(model.ServiceId);
                if (serviceForValidation == null)
                {
                    return BadRequest();
                }
                else
                {
                    ApplicationUser user = await FindUser();
                    if(user == null)
                    {
                        return BadRequest();
                    }
                    else
                    {
                        if (IsUserInRole("Manager").Result && !IsUserInRole("Administrator").Result && serviceForValidation.Creator != user.Id)
                        {
                            return BadRequest("Access denied.");
                        }

                        Service oldService = _unitOfWork.Services.Get(model.ServiceId);
                        ImageHelper imageHelper = new ImageHelper();

                        string imageUri = string.Empty;
                        if(model.Image != null)
                        {
                            imageHelper.DeleteImage(_environment.WebRootPath, folderPath, oldService.LogoImage);
                            imageUri = await imageHelper.UploadImageToServer(_environment.WebRootPath, folderPath, model.Image);
                        }
                        else
                        {
                            imageUri = oldService.LogoImage;
                        }


                        oldService.Name = model.Name;
                        oldService.EmailAddress = model.EmailAddress;
                        oldService.Description = model.Description;
                        oldService.LogoImage = imageUri;

                        try
                        {
                            lock (lockObjectForServices)
                            {
                                _unitOfWork.Services.Update(oldService);
                                _unitOfWork.Complete();
                            }
                        }
                        catch (DBConcurrencyException)
                        {
                            if (_unitOfWork.Services.Get(oldService.Id) == null)
                            {
                                return NotFound();
                            }
                            else
                            {
                                throw;
                            }
                        }

                        return Ok(oldService);
                    }
               
                }
            }
        }


        [HttpDelete]
        [Route("DeleteService")]
        [Authorize(Roles = "Administrator, Manager")]
        public async Task<IActionResult> DeleteService([FromQuery] int serviceId)
        {
            Service service = _unitOfWork.Services.Get(serviceId);
            if (service == null)
            {
                return NotFound();
            }
            else
            {
                ApplicationUser user = await FindUser();
                if (IsUserInRole("Manager").Result && !IsUserInRole("Administrator").Result && service.Creator != user.Id)
                {
                    return BadRequest();
                }

                ImageHelper imageHelper = new ImageHelper();
                List<long> branchOfficeIds = new List<long>();
                if (service.BranchOfficces != null)
                {
                    foreach (BranchOffice branchOffice in service.BranchOfficces)
                    {
                        branchOfficeIds.Add(branchOffice.Id);
                        imageHelper.DeleteImage(_environment.WebRootPath, folderPath, branchOffice.Image);
                    }
                }

                List<long> vehiclesIds = new List<long>();
                if (service.Vehicles != null)
                {
                    foreach (Vehicle vehicle in service.Vehicles)
                    {
                        vehiclesIds.Add(vehicle.Id);
                        imageHelper.DeleteImages(_environment.WebRootPath, folderPath, vehicle.Images);
                    }
                }

                imageHelper.DeleteImage(_environment.WebRootPath, folderPath, service.LogoImage);

                foreach (long branchOfficeId in branchOfficeIds)
                {
                    BranchOffice branchOffice = _unitOfWork.BranchOffices.Get(branchOfficeId);
                    _unitOfWork.BranchOffices.Remove(branchOffice);
                }

                foreach (long vehicleId in vehiclesIds)
                {
                    Vehicle vehicle = _unitOfWork.Vehicles.Get(vehicleId);
                    _unitOfWork.Vehicles.Remove(vehicle);
                }

                ServiceForApproval serviceForApproval = _unitOfWork.ServicesForApproval.Find(x => x.Service.Id == serviceId).FirstOrDefault();
                _unitOfWork.ServicesForApproval.Remove(serviceForApproval);

                try
                {
                    lock (lockObjectForServices)
                    {
                        _unitOfWork.Services.Remove(service);
                        _unitOfWork.Complete();
                    }
                }
                catch (DBConcurrencyException ex)
                {
                    if (_unitOfWork.Services.Get(serviceId) == null)
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return Ok(_unitOfWork.Services.GetAll());
            }
        }


        private async Task<bool> IsUserInRole(string role)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(string.Format(_isUserInRoleEndpoint, User.FindFirstValue(ClaimTypes.NameIdentifier), role)).ConfigureAwait(true);
                if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                {
                    return await httpResponseMessage.Content.ReadAsAsync<bool>().ConfigureAwait(false);
                }
                else
                {
                    return false;
                }
            }
        }

        private async Task<ApplicationUser> FindUser()
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage httpResponseMessage = await httpClient.GetAsync(_findUserEndpoint + User.FindFirstValue(ClaimTypes.NameIdentifier)).ConfigureAwait(true);
                if (httpResponseMessage.StatusCode == HttpStatusCode.OK)
                {
                    ApplicationUser user = await httpResponseMessage.Content.ReadAsAsync<ApplicationUser>().ConfigureAwait(false);
                    return user;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
