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
        private string validationErrorMessage;

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
        [AllowAnonymous]
        [Route("GetServices")]
        public IActionResult GetServices()
        {
            IEnumerable<Service> services = _unitOfWork.Services.Find(s => s.IsApproved);

            return Ok(services.ToList());
        }

        // GET: api/Services/LoadImage
        [HttpGet]
        [Route("LoadImage")]
        [AllowAnonymous]
        public IActionResult LoadImage([FromForm] string imageId)
        {
            Stream readStream = ImageHelper.ReadImageFromServer(_environment.WebRootPath, imageId);
            return File(readStream, "image/jpeg");
        }

        // GET: api/Services/GetService/5
        [HttpGet]
        [AllowAnonymous]
        [Route("GetService")]
        public IActionResult GetService([FromForm] long serviceId)
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

        // GET: api/Services/GetBranchOffices
        [HttpGet]
        [AllowAnonymous]
        [Route("GetBranchOffices")]
        public IActionResult GetBranchOffices([FromForm] long serviceId)
        {
            Service service = _unitOfWork.Services.Get(serviceId);

            if (service == null)
            {
                return BadRequest();
            }

            return Ok(service.BranchOfficces);

        }

        // GET: /api/Services/GetVehicles/5
        [HttpGet]
        [AllowAnonymous]
        [Route("GetVehicles")]
        public IActionResult GetVehicles([FromForm] long serviceId)
        {
            Service service = _unitOfWork.Services.Get(serviceId);

            if (service == null)
            {
                return BadRequest();
            }

            return Ok(service.Vehicles);
        }

        // GET: api/Services/GetComments
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

        // GET: api/Services/GetRatings
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

        // GET: api/Services/HasUserCommented
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

        // GET: api/Services/HasUserRated
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

        // PUT: api/Services/PutService/5
        [HttpPut]
        [Route("PutService")]
        [Authorize(Roles = "Administrator, Manager")]
        public async Task<IActionResult> PutService([FromForm] long serviceId, EditRentVehicleServiceBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (serviceId != model.Id)
            {
                return BadRequest();
            }

            Service serviceForValidation = _unitOfWork.Services.Get(serviceId);
            if (serviceForValidation == null)
            {
                return BadRequest("Service don't exists.");
            }

            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (_userManager.IsInRoleAsync(user, "Manager").Result && !_userManager.IsInRoleAsync(user, "Administrator").Result && serviceForValidation.Creator != user.Id)
            {
                return BadRequest("Access denied.");
            }

            /*
            HttpRequest httpRequest = HttpContext.Current.Request;
            if (!ImageHelper.ValidateImage(httpRequest.Files[0], out validationErrorMessage))
            {
                return BadRequest(validationErrorMessage);
            }
            */

            Service oldService = _unitOfWork.Services.Get(serviceId);

            ImageHelper.DeleteImage(_environment.WebRootPath, oldService.LogoImage);

            oldService.Name = model.Name;
            oldService.EmailAddress = model.EmailAddress;
            oldService.Description = model.Description;
            //oldService.LogoImage = ImageHelper.SaveImageToServer(httpRequest.Files[0]);

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
                if (!ServiceExists(oldService.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Service successfully updated.");
        }

        // GET: api/Services/ServicesForApproves
        [HttpGet]
        [Route("ServicesForApproves")]
        [Authorize(Roles = "Administrator")]
        public IActionResult ServicesForApproves()
        {
            IEnumerable<Service> servicesForApproves = _unitOfWork.Services.Find(s => !s.IsApproved);

            List<Service> servicesForApprovesList = new List<Service>(servicesForApproves);

            return Ok(servicesForApprovesList);
        }

        // POST api/Services/ApproveService
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

        // POST api/Services/RejectService
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
            ImageHelper.DeleteImage(_environment.WebRootPath, serviceForApprove.LogoImage);

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

        // POST: api/Services/PostService
        [HttpPost]
        [Route("PostService")]
        [Authorize(Roles = "Administrator, Manager")]
        public async Task<IActionResult> PostService(CreateRentVehicleServiceBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            /*
            if (!ImageHelper.ValidateImage(httpRequest.Files[0], out validationErrorMessage))
            {
                return BadRequest(validationErrorMessage);
            }
            */

            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (_userManager.IsInRoleAsync(user, "Manager").Result)
            {
                /*
                if (_unitOfWork.BanedManagers.Find(bm => bm.User.Id == user.Id).Count() > 0)
                {
                    return BadRequest("You are baned for add service.");
                }
                */
            }

            Service service = new Service()
            {
                Creator = user.Id,
                Name = model.Name,
                EmailAddress = model.ContactEmail,
                Description = model.Description,
                //LogoImage = ImageHelper.SaveImageToServer(httpRequest.Files[0]),
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

            return Ok("RentVehicle Service succsessfully created");
        }

        // DELETE: api/Services/DeleteService/5
        [HttpDelete]
        [Route("DeleteService")]
        [Authorize(Roles = "Administrator, Manager")]
        public async Task<IActionResult> DeleteService([FromForm] int serviceId)
        {
            Service service = _unitOfWork.Services.Get(serviceId);

            if (service == null)
            {
                return NotFound();
            }

            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (_userManager.IsInRoleAsync(user, "Manager").Result && !_userManager.IsInRoleAsync(user, "Administrator").Result && service.Creator != user.Id)
            {
                return BadRequest("Access denied.");
            }

            List<long> branchOfficeIds = new List<long>();

            if (service.BranchOfficces != null)
            {
                foreach (BranchOffice branchOffice in service.BranchOfficces)
                {
                    branchOfficeIds.Add(branchOffice.Id);
                    ImageHelper.DeleteImage(_environment.WebRootPath, branchOffice.Image);
                }
            }

            List<long> vehiclesIds = new List<long>();

            if (service.Vehicles != null)
            {
                foreach (Vehicle vehicle in service.Vehicles)
                {
                    vehiclesIds.Add(vehicle.Id);
                    ImageHelper.DeleteImages(_environment.WebRootPath, vehicle.Images);
                }
            }

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

            ImageHelper.DeleteImage(_environment.WebRootPath, service.LogoImage);
            /*
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

                    _unitOfWork.Services.Remove(service);
                    _unitOfWork.Complete();

                }
            }
            catch (DBConcurrencyException)
            {
                if (!ServiceExists(service.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            */
            return Ok("Service successfully deleted.");
        }

        private bool ServiceExists(long id)
        {
            return _unitOfWork.Services.Get(id) != null;
        }
    }
}