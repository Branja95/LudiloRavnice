using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using RentApp.Models;
using RentApp.Models.Entities;
using RentApp.Providers;
using RentApp.Results;
using RentApp.Persistance.UnitOfWork;
using RentApp.Hubs;
using System.Linq;
using System.Collections;
using RentApp.Helpers;
using static RentApp.Models.ServiceBindingModel;
using System.Runtime.Remoting.Messaging;
using RentApp.Services;
using System.Web.Http.Description;
using System.Net;
using System.Data.Entity.Infrastructure;

namespace RentApp.Controllers
{
    [Authorize]
    [RoutePrefix("api/Services")]
    public class ServicesController : ApiController
    {
        private string validationErrorMessage;
        private readonly IUnitOfWork unitOfWork;
        private readonly ISMTPService SMTPService;
        public ApplicationUserManager UserManager { get; private set; }

        public ServicesController() { }

        public ServicesController(ApplicationUserManager userManager,
           IUnitOfWork unitOfWork,
           ISMTPService SMTPService)
        {
            UserManager = userManager;
            this.unitOfWork = unitOfWork;
            this.SMTPService = SMTPService;
        }
        
        // GET: api/Services/GetServices
        [HttpGet]
        [AllowAnonymous]
        [Route("GetServices")]
        public IHttpActionResult GetServices()
        {
            IEnumerable<Service> services = unitOfWork.Services.Find(s => s.IsApproved);

            List<Service> servicesList = new List<Service>(services);

            return Ok(servicesList);
        }

        // GET: api/Services/LoadImage
        [HttpGet]
        [Route("LoadImage")]
        [AllowAnonymous]
        public HttpResponseMessage LoadImage([FromUri] string imageId)
        {
            return ImageHelper.LoadImage(imageId);
        }

        // GET: api/Services/GetService/5
        [HttpGet]
        [AllowAnonymous]
        [Route("GetService")]
        public IHttpActionResult GetService([FromUri] long serviceId)
        {
            Service service = unitOfWork.Services.Get(serviceId);
            if (service == null)
            {
                return NotFound();
            }

            return Ok(service);
        }

        // GET: api/Services/GetBranchOffices
        [HttpGet]
        [AllowAnonymous]
        [Route("GetBranchOffices")]
        public IHttpActionResult GetBranchOffices([FromUri] long serviceId)
        {
            Service service = unitOfWork.Services.Get(serviceId);

            if(service == null)
            {
                return BadRequest();
            }

            return Ok(service.BranchOfficces);

        }

        // GET: /api/Services/GetVehicles/5
        [HttpGet]
        [AllowAnonymous]
        [Route("GetVehicles")]
        public IHttpActionResult GetVehicles([FromUri] long serviceId)
        {
            Service service = unitOfWork.Services.Get(serviceId);

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
        public IHttpActionResult GetComments([FromUri] long serviceId)
        {
            Service service = unitOfWork.Services.Get(serviceId);
            if (service == null)
            {
                return NotFound();
            }

            List<Comment> comments = new List<Comment>(service.Comments);

            return Ok(comments);
        }
        
        // GET: api/Services/GetRatings
        [HttpGet]
        [AllowAnonymous]
        [Route("GetRatings")]
        public IHttpActionResult GetRatings([FromUri] long serviceId)
        {
            Service service = unitOfWork.Services.Get(serviceId);
            if (service == null)
            {
                return NotFound();
            }

            List<Rating> ratings = new List<Rating>(service.Ratings);

            return Ok(ratings);
        }

        // GET: api/Services/HasUserCommented
        [HttpGet]
        [AllowAnonymous] // Fix this
        [Route("HasUserCommented")]
        public async Task<IHttpActionResult> HasUserCommented([FromUri] long serviceId)
        {
            bool hasUserCommented = false;

            Service service = unitOfWork.Services.Get(serviceId);
            if (service == null)
            {
                return NotFound();
            }

            RAIdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

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
        [AllowAnonymous] // Fix this
        [Route("HasUserRated")]
        public async Task<IHttpActionResult> HasUserRated([FromUri] long serviceId)
        {
            bool hasUserRated = false;

            Service service = unitOfWork.Services.Get(serviceId);
            if (service == null)
            {
                return NotFound();
            }

            RAIdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

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
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IHttpActionResult> PutService([FromUri] long serviceId, EditRentVehicleServiceBindingModel model)
        {
            RAIdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            Service serviceForValidation = unitOfWork.Services.Get(serviceId);

            if(serviceForValidation == null)
            {
                return BadRequest("Service don't exists.");
            }

            if (UserManager.IsInRole(user.Id, "Manager") && !UserManager.IsInRole(user.Id, "Admin"))
            {
                if (serviceForValidation.Creator != user.Id)
                {
                    return BadRequest("Access denied.");
                }
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (serviceId != model.Id)
            {
                return BadRequest();
            }

            HttpRequest httpRequest = HttpContext.Current.Request;

            if (!ImageHelper.ValidateImage(httpRequest.Files[0], out validationErrorMessage))
            {
                return BadRequest(validationErrorMessage);
            }

            Service oldService = unitOfWork.Services.Get(serviceId);

            ImageHelper.DeleteImage(oldService.LogoImage);

            oldService.Name = model.Name;
            oldService.EmailAddress = model.EmailAddress;
            oldService.Description = model.Description;
            oldService.LogoImage = ImageHelper.SaveImageToServer(httpRequest.Files[0]);

            unitOfWork.Services.Update(oldService);
            unitOfWork.Complete();

            return StatusCode(HttpStatusCode.NoContent);
        }
        
        // GET: api/Services/ServicesForApproves
        [HttpGet]
        [Route("ServicesForApproves")]
        [Authorize(Roles = "Admin")]
        public IHttpActionResult ServicesForApproves()
        {
            IEnumerable<Service> servicesForApproves = unitOfWork.Services.Find(s => !s.IsApproved);

            List<Service> servicesForApprovesList = new List<Service>(servicesForApproves);

            return Ok(servicesForApprovesList);
        }

        // POST api/Services/ApproveService
        [HttpPost]
        [Route("ApproveService")]
        [Authorize(Roles = "Admin")]
        public async Task<IHttpActionResult> ApproveService([FromBody] long id)
        {

            if (unitOfWork.Services.Get(id) == null)
            {
                return BadRequest("Bad request. Id don't exists.");
            }

            Service serviceForApprove = unitOfWork.Services.Get(id);

            RAIdentityUser user = await UserManager.FindByIdAsync(serviceForApprove.Creator);

            serviceForApprove.IsApproved = true;

            SMTPService.SendMail("Service approved", "Your service "+ serviceForApprove.Name + " is approved, now you can add branch offices and vehicles.", user.Email);

            unitOfWork.Services.Update(serviceForApprove);
            unitOfWork.Complete();

            return Ok("Service Successfully approved.");
        }

        // POST api/Services/RejectService
        [HttpPost]
        [Route("RejectService")]
        [Authorize(Roles = "Admin")]
        public async Task<IHttpActionResult> RejectService([FromBody]long id)
        {

            if (unitOfWork.Services.Get(id) == null)
            {
                return BadRequest("Bad request. Id don't exists.");
            }

            Service serviceForApprove = unitOfWork.Services.Get(id);

            RAIdentityUser user = await UserManager.FindByIdAsync(serviceForApprove.Creator);

            SMTPService.SendMail("Service rejected", "Your service " + serviceForApprove.Name + " is rejected.", user.Email);

            ImageHelper.DeleteImage(serviceForApprove.LogoImage);

            unitOfWork.Services.Remove(serviceForApprove);
            unitOfWork.Complete();

            return Ok("Service Successfully rejected.");
        }

        // POST: api/Services/PostService
        [HttpPost]
        [Route("PostService")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IHttpActionResult> PostService(CreateRentVehicleServiceBindingModel model)
        {
            HttpRequest httpRequest = HttpContext.Current.Request;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!ImageHelper.ValidateImage(httpRequest.Files[0], out validationErrorMessage))
            {
                return BadRequest(validationErrorMessage);
            }

            RAIdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            Service service = new Service()
            {
                Creator = user.Id,
                Name = model.Name,
                EmailAddress = model.ContactEmail,
                Description = model.Description,
                LogoImage = ImageHelper.SaveImageToServer(httpRequest.Files[0]),
                IsApproved = false
            };

            unitOfWork.Services.Add(service);
            unitOfWork.Complete();

            NotificationHub.NewRentVehicleServiceToApprove(unitOfWork.Services.Find(s => !s.IsApproved).Count());

            return Ok("RentVehicle Service succsessfully created");
        }

        // DELETE: api/Services/DeleteService/5
        [HttpDelete]
        [Route("DeleteService")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IHttpActionResult> DeleteService([FromUri] int serviceId)
        {
            Service service = unitOfWork.Services.Get(serviceId);

            if (service == null)
            {
                return NotFound();
            }

            RAIdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            if (UserManager.IsInRole(user.Id, "Manager") && !UserManager.IsInRole(user.Id, "Admin"))
            {
                if (service.Creator != user.Id)
                {
                    return BadRequest("Access denied.");
                }
            }

            List<long> branchOfficeIds = new List<long>();

            if (service.BranchOfficces != null)
            {
                foreach (BranchOffice branchOffice in service.BranchOfficces)
                {
                    branchOfficeIds.Add(branchOffice.Id);
                }
            }

            foreach(long branchOfficeId in branchOfficeIds)
            {
                BranchOffice branchOffice = unitOfWork.BranchOffices.Get(branchOfficeId);
                unitOfWork.BranchOffices.Remove(branchOffice);
            }

            unitOfWork.Complete();

            List<long> vehiclesIds = new List<long>();

            if (service.Vehicles != null)
            {
                foreach (Vehicle vehicle in service.Vehicles)
                {
                    vehiclesIds.Add(vehicle.Id);
                }
            }

            foreach (long vehicleId in vehiclesIds)
            {
                Vehicle vehicle = unitOfWork.Vehicles.Get(vehicleId);
                unitOfWork.Vehicles.Remove(vehicle);
            }

            unitOfWork.Complete();

            ImageHelper.DeleteImage(service.LogoImage);

            unitOfWork.Services.Remove(service);
            unitOfWork.Complete();

            return Ok(service);
        }

        private bool ServiceExists(long id)
        {
            return unitOfWork.Services.Get(id) != null;
        }

    }
}