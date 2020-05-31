using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AccountManaging.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using RentVehicle.Helpers;
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

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;

        private readonly IHostingEnvironment _environment;
        private readonly IConfiguration _configuration;

        public ServiceController(UserManager<ApplicationUser> userManager,
           IUnitOfWork unitOfWork,
           IEmailService emailService,
           IHostingEnvironment environment,
           IConfiguration configuration)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _environment = environment;
            _configuration = configuration;
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
        [Route("ServicesForApproves")]
        [Authorize(Roles = "Administrator")]
        public IActionResult ServicesForApproves()
        {
            IEnumerable<Service> servicesForApproves = _unitOfWork.Services.Find(s => !s.IsApproved);

            List<Service> servicesForApprovesList = new List<Service>(servicesForApproves);

            return Ok(servicesForApprovesList);
        }


        [HttpPost]
        [Route("ApproveService")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> ApproveService([FromBody] long id)
        {

            if (_unitOfWork.Services.Get(id) == null)
            {
                return BadRequest("Bad request. Id don't exists.");
            }

            Service serviceForApprove = _unitOfWork.Services.Get(id);

            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            serviceForApprove.IsApproved = true;
            _emailService.SendMail("Service approved", "Your service " + serviceForApprove.Name + " is approved, now you can add branch offices and vehicles.", user.Email);
            try
            {
                lock (lockObjectForServices)
                {
                    _unitOfWork.Services.Update(serviceForApprove);
                    _unitOfWork.Complete();
                }
            }
            catch (DBConcurrencyException)
            {
                return NotFound();
            }

            return Ok("Service Successfully approved.");
        }


        [HttpPost]
        [Route("RejectService")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> RejectService([FromBody] long id)
        {
            if (_unitOfWork.Services.Get(id) == null)
            {
                return BadRequest("Bad request. Id don't exists.");
            }

            Service serviceForApprove = _unitOfWork.Services.Get(id);

            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            _emailService.SendMail("Service rejected", "Your service " + serviceForApprove.Name + " is rejected.", user.Email);
            ImageHelper.DeleteImage(_environment.WebRootPath, folderPath, serviceForApprove.LogoImage);

            try
            {
                lock (lockObjectForServices)
                {
                    _unitOfWork.Services.Remove(serviceForApprove);
                    _unitOfWork.Complete();
                }
            }
            catch (DBConcurrencyException)
            {
                return NotFound();
            }

            return Ok("Service Successfully rejected.");
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
                ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
                /* communicate with account
                if (_userManager.IsInRoleAsync(user, "Manager").Result)
                {
                    if (_unitOfWork.BanedManagers.Find(bm => bm.User.Id == user.Id).Count() > 0)
                    {
                        return BadRequest("You are baned for add service.");
                    }
                }
                */

                ImageHelper.UploadImageToServer(_environment.WebRootPath, folderPath, model.Image);
                Service service = new Service()
                {
                    Creator = user.Id,
                    Name = model.Name,
                    EmailAddress = model.ContactEmail,
                    Description = model.Description,
                    LogoImage = model.Image.FileName,
                    IsApproved = false
                };

                try
                {
                    lock (lockObjectForServices)
                    {
                        _unitOfWork.Services.Add(service);
                        _unitOfWork.Complete();
                    }
                }
                catch (DBConcurrencyException)
                {
                    return NotFound();
                }
                //NotificationHub.NewRentVehicleServiceToApprove(unitOfWork.Services.Find(s => !s.IsApproved).Count());
                return Ok();
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
                    ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
                    if (_userManager.IsInRoleAsync(user, "Manager").Result && !_userManager.IsInRoleAsync(user, "Administrator").Result && serviceForValidation.Creator != user.Id)
                    {
                        return BadRequest("Access denied.");
                    }

                    Service oldService = _unitOfWork.Services.Get(model.ServiceId);

                    ImageHelper.DeleteImage(_environment.WebRootPath, folderPath, oldService.LogoImage);
                    ImageHelper.UploadImageToServer(_environment.WebRootPath, folderPath, model.Image);

                    oldService.Name = model.Name;
                    oldService.EmailAddress = model.EmailAddress;
                    oldService.Description = model.Description;
                    oldService.LogoImage = model.Image.FileName;

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

                    return Ok();
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
                ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
                if (_userManager.IsInRoleAsync(user, "Manager").Result && !_userManager.IsInRoleAsync(user, "Administrator").Result && service.Creator != user.Id)
                {
                    return BadRequest();
                }

                List<long> branchOfficeIds = new List<long>();
                if (service.BranchOfficces != null)
                {
                    foreach (BranchOffice branchOffice in service.BranchOfficces)
                    {
                        branchOfficeIds.Add(branchOffice.Id);
                        ImageHelper.DeleteImage(_environment.WebRootPath, folderPath, branchOffice.Image);
                    }
                }

                List<long> vehiclesIds = new List<long>();
                if (service.Vehicles != null)
                {
                    foreach (Vehicle vehicle in service.Vehicles)
                    {
                        vehiclesIds.Add(vehicle.Id);
                        ImageHelper.DeleteImages(_environment.WebRootPath, folderPath, vehicle.Images);
                    }
                }

                /* communicate with booking
                List<long> commentIds = new List<long>();
                if (service.Comments != null)
                {
                    foreach (Comment comment in service.Comments)
                    {
                        commentIds.Add(comment.Id);
                    }
                }
                List<long> raitingIds = new List<long>();
                if (service.Ratings != null)
                {
                    foreach (Rating rating in service.Ratings)
                    {
                        raitingIds.Add(rating.Id);
                    }
                }
                */

                ImageHelper.DeleteImage(_environment.WebRootPath, folderPath, service.LogoImage);
                try
                {
                    lock (lockObjectForServices)
                    {
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

                        /* communicate with bookig
                        foreach (long commentId in commentIds)
                        {
                            Comment comment = _unitOfWork.Comments.Get(commentId);
                            _unitOfWork.Comments.Remove(comment);
                        }

                        foreach (long raitingId in raitingIds)
                        {
                            Rating raiting = _unitOfWork.Ratings.Get(raitingId);
                            _unitOfWork.Ratings.Remove(raiting);
                        }
                        */

                        _unitOfWork.Services.Remove(service);
                        _unitOfWork.Complete();

                    }
                }
                catch (DBConcurrencyException)
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

                return Ok();
            }
        }


        #region Booking
        [HttpGet]
        [AllowAnonymous]
        [Route("GetComments")]
        public async Task<IActionResult> GetComments([FromForm] long serviceId)
        {
            Service service = _unitOfWork.Services.Get(serviceId);
            if (service == null)
            {
                return NotFound();
            }

            List<Comment> comments = new List<Comment>();

            foreach (Comment comment in service.Comments)
            {
                ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
                comments.Add(new Comment()
                {
                    Id = comment.Id,
                    UserId = user.Email,
                    Text = comment.Text,
                    DateTime = comment.DateTime
                });
            }

            return Ok(comments);
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("GetRatings")]
        public async Task<IActionResult> GetRatings([FromForm] long serviceId)
        {
            Service service = _unitOfWork.Services.Get(serviceId);
            if (service == null)
            {
                return NotFound();
            }

            List<Rating> ratings = new List<Rating>();

            foreach (Rating rating in service.Ratings)
            {
                ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
                ratings.Add(new Rating()
                {
                    Id = rating.Id,
                    UserId = user.Email,
                    Value = rating.Value
                });
            }

            return Ok(ratings);
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("HasUserCommented")]
        public async Task<IActionResult> HasUserCommented([FromForm] long serviceId)
        {
            bool hasUserCommented = false;

            Service service = _unitOfWork.Services.Get(serviceId);
            if (service == null)
            {
                return NotFound();
            }

            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                return Ok(true);
            }

            Comment comment = service.Comments.Find(comm => comm.UserId == user.Id);
            if (comment == null)
            {
                hasUserCommented = false;
            }
            else
            {
                hasUserCommented = true;
            }

            return Ok(hasUserCommented);
        }


        [HttpGet]
        [AllowAnonymous]
        [Route("HasUserRated")]
        public async Task<IActionResult> HasUserRated([FromForm] long serviceId)
        {
            bool hasUserRated = false;

            Service service = _unitOfWork.Services.Get(serviceId);
            if (service == null)
            {
                return NotFound();
            }

            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (user == null)
            {
                return Ok(true);
            }

            Rating rating = service.Ratings.Find(rate => rate.UserId == user.Id);
            if (rating == null)
            {
                hasUserRated = false;
            }
            else
            {
                hasUserRated = true;
            }

            return Ok(hasUserRated);
        }
        #endregion
    }
}
