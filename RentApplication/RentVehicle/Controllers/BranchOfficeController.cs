using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RentVehicle.Helpers;
using RentVehicle.Models.Entities;
using RentVehicle.Models.IdentityUsers;
using RentVehicle.Persistance.UnitOfWork;
using static RentVehicle.Models.Bindings.BranchOfficeBindingModel;

namespace RentVehicle.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BranchOfficeController : ControllerBase
    {
        private static readonly string folderPath = @"App_Data\branch-office\";
        private static object lockObjectForBranchOffices = new object();

        private readonly string _isUserInRoleEndpoint;
        private readonly string _findUserEndpoint;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHostingEnvironment _environment;

        public BranchOfficeController(IUnitOfWork unitOfWork,
           IHostingEnvironment environment,
           IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _environment = environment;
            _isUserInRoleEndpoint = configuration["AccountService:IsUserInRoleEndpoint"];
            _findUserEndpoint = configuration["AccountService:FindUserEndpoint"];
        }

        [HttpGet]
        [AllowAnonymous]
        [Route("GetBranchOffices")]
        public IActionResult GetBranchOffices()
        {
            IEnumerable<BranchOffice> branchOffices = _unitOfWork.BranchOffices.GetAll();
            return Ok(branchOffices.ToList());
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("GetVehicleBranchOffices")]
        public IActionResult GetVehicleBranchOffices([FromQuery] long vehicleId)
        {
            Vehicle vehicle = _unitOfWork.Vehicles.Get(vehicleId);
            if (vehicle == null)
            {
                return NotFound();
            }
            else
            {
                Service service = null;

                IEnumerable<Service> services = _unitOfWork.Services.GetAll();
                foreach (Service serviceVehicle in services)
                {
                    if (serviceVehicle.Vehicles.Contains(vehicle))
                    {
                        service = serviceVehicle;
                        break;
                    }
                }

                if (service == null)
                {
                    return NotFound();
                }
                else
                {
                    return Ok(service.BranchOfficces);
                }
            }
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("GetBranchOffice")]
        public IActionResult GetBranchOffice([FromQuery] long id)
        {
            BranchOffice branchOffice = _unitOfWork.BranchOffices.Get(id);
            if (branchOffice == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(branchOffice);
            }
        }


        [HttpGet]
        [Route("LoadImage")]
        [AllowAnonymous]
        public IActionResult LoadImage([FromQuery] string imageId)
        {
            string filePath = Path.Combine(_environment.WebRootPath, folderPath, imageId);
            string fileExtension = Path.GetExtension(filePath);

            Stream readStream =  ImageHelper.ReadImageFromServer(_environment.WebRootPath, folderPath, imageId);
            return File(readStream, "image/" + fileExtension);
        }


        [HttpPost]
        [Route("PostBranchOffice")]
        [Authorize(Roles = "Administrator, Manager")]
        public async Task<IActionResult> PostBranchOffice([FromForm] CreateBranchOfficeBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                Service service = _unitOfWork.Services.Get(model.ServiceId);
                if (service == null)
                {
                    return BadRequest();
                }

                ApplicationUser user = await FindUser();
                if (IsUserInRole("Manager").Result && !IsUserInRole("Administrator").Result && service.Creator != user.Id)
                {
                   return BadRequest();
                }

                ImageHelper.UploadImageToServer(_environment.WebRootPath, folderPath, model.Image);
                BranchOffice branchOffice = new BranchOffice
                {
                    Address = model.Address,
                    Latitude = model.Latitude,
                    Longitude = model.Longitude,
                    Image = model.Image.FileName
                };

                try
                {
                    lock (lockObjectForBranchOffices)
                    {
                        service.BranchOfficces.Add(branchOffice);

                        _unitOfWork.Services.Update(service);
                        _unitOfWork.BranchOffices.Add(branchOffice);
                        _unitOfWork.Complete();
                    }
                }
                catch (DBConcurrencyException)
                {
                    return NotFound();
                }

                return Ok();
            }
        }


        [HttpPut]
        [Route("PutBranchOffice")]
        [Authorize(Roles = "Administrator, Manager")]
        public async Task<IActionResult> PutBranchOffice([FromForm] EditBranchOfficeBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                ApplicationUser user = await FindUser();

                Service serviceForValidation = _unitOfWork.Services.Get(model.ServiceId);
                if (serviceForValidation == null)
                {
                    return BadRequest();
                }

                if (IsUserInRole("Manager").Result && !IsUserInRole("Administartor").Result && serviceForValidation.Creator != user.Id)
                {
                    return BadRequest("Access denied.");
                }

                BranchOffice branchOffice = _unitOfWork.BranchOffices.Get(model.Id);
                if (branchOffice == null)
                {
                    return BadRequest("Branch office don't exists.");
                }
                else
                {
                    if (!serviceForValidation.BranchOfficces.Contains(branchOffice))
                    {
                        return BadRequest("Branch office don't exists.");
                    }
                    ImageHelper.DeleteImage(_environment.WebRootPath, folderPath, branchOffice.Image);
                    ImageHelper.UploadImageToServer(_environment.WebRootPath, folderPath, model.Image);

                    branchOffice.Address = model.Address;
                    branchOffice.Latitude = model.Latitude;
                    branchOffice.Longitude = model.Longitude;
                    branchOffice.Image = model.Image.FileName;

                    try
                    {
                        lock (lockObjectForBranchOffices)
                        {
                            _unitOfWork.BranchOffices.Update(branchOffice);
                            _unitOfWork.Complete();
                        }
                    }
                    catch (DBConcurrencyException)
                    {
                        return NotFound();
                    }

                    return Ok();
                }
            }
        }


        [HttpDelete]
        [Route("DeleteBranchOffice")]
        [Authorize(Roles = "Administrator, Manager")]
        public async Task<IActionResult> DeleteBranchOffice([FromQuery] long serviceId, [FromQuery] long branchOfficeId)
        {
            BranchOffice branchOffice = _unitOfWork.BranchOffices.Get(branchOfficeId);
            Service service = _unitOfWork.Services.Get(serviceId);

            if (branchOffice == null || service == null)
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

                ImageHelper.DeleteImage(_environment.WebRootPath, folderPath, branchOffice.Image);
                try
                {
                    lock (lockObjectForBranchOffices)
                    {
                        service.BranchOfficces.Remove(branchOffice);
                        _unitOfWork.BranchOffices.Remove(branchOffice);
                        _unitOfWork.Complete();
                    }
                }
                catch (DBConcurrencyException)
                {
                    return NotFound();
                }

                return Ok();
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