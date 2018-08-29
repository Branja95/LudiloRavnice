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
using System.Runtime.Remoting.Messaging;
using RentApp.Services;
using System.Web.Http.Description;
using System.Net;
using System.Data.Entity.Infrastructure;
using static RentApp.Models.BranchOfficeBindingModel;


namespace RentApp.Controllers
{
    [Authorize]
    [RoutePrefix("api/BranchOffices")]
    public class BranchOfficesController : ApiController
    {
        private string validationErrorMessage;
        private readonly IUnitOfWork unitOfWork;
        private readonly ISMTPService SMTPService;

        public BranchOfficesController() { }

        public BranchOfficesController(ApplicationUserManager userManager,
           IUnitOfWork unitOfWork,
           ISMTPService SMTPService)
        {
            UserManager = userManager;
            this.unitOfWork = unitOfWork;
            this.SMTPService = SMTPService;
        }

        public ApplicationUserManager UserManager { get; private set; }

        // GET: api/BranchOffices/GetBranchOffices
        [HttpGet]
        [AllowAnonymous]
        [Route("GetBranchOffices")]
        public IHttpActionResult GetBranchOffices()
        {
            IEnumerable<BranchOffice> branchOffices = unitOfWork.BranchOffices.GetAll();

            List<BranchOffice> branchOfficesList = new List<BranchOffice>(branchOffices);

            return Ok(branchOfficesList);

        }

        // GET: api/BranchOffices/GetBranchOffices/5
        [HttpGet]
        [AllowAnonymous]
        [Route("GetVehicleBranchOffices")]
        public IHttpActionResult GetVehicleBranchOffices([FromUri] long vehicleId)
        {
            Vehicle vehicle = unitOfWork.Vehicles.Get(vehicleId);
            if(vehicle == null)
            {
                return NotFound();
            }

            IEnumerable<Service> services = unitOfWork.Services.GetAll();

            Service service = null;

            foreach(Service s in services)
            {
                if(s.Vehicles.Contains(vehicle))
                {
                    service = s;
                    break;
                }
            }

            if (service == null)
            {
                return NotFound();
            }

            return Ok(service.BranchOfficces);

        }


        // GET: api/BranchOffices/GetBranchOffice
        [HttpGet]
        [AllowAnonymous]
        [Route("GetBranchOffice")]
        public IHttpActionResult GetBranchOffice([FromUri] long branchOfficeId)
        {
            BranchOffice branchOffice = unitOfWork.BranchOffices.Get(branchOfficeId);
            if (branchOffice == null)
            {
                return NotFound();
            }

            return Ok(branchOffice);
        }

        // GET: api/BranchOffices/LoadImage
        [HttpGet]
        [Route("LoadImage")]
        [AllowAnonymous]
        public HttpResponseMessage LoadImage([FromUri] string imageId)
        {
            return ImageHelper.LoadImage(imageId);
        }

        // PUT: api/BranchOffices/PutBranchOffice
        [HttpPut]
        [Route("PutBranchOffice")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IHttpActionResult> PutBranchOffice([FromUri] long serviceId, EditBranchOfficeBindingModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            RAIdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            Service serviceForValidation = unitOfWork.Services.Get(serviceId);

            if (serviceForValidation == null)
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

            BranchOffice branchOffice = unitOfWork.BranchOffices.Get(model.Id);

            if (branchOffice == null)
            {
                return BadRequest("Branch office don't exists.");
            }

            HttpRequest httpRequest = HttpContext.Current.Request;

            if (!ImageHelper.ValidateImage(httpRequest.Files[0], out validationErrorMessage))
            {
                return BadRequest(validationErrorMessage);
            }

            if(!serviceForValidation.BranchOfficces.Contains(branchOffice))
            {
                return BadRequest("Branch office don't exists.");
            }

            ImageHelper.DeleteImage(branchOffice.Image);

            branchOffice.Address = model.Address;
            branchOffice.Latitude = model.Latitude;
            branchOffice.Longitude = model.Longitude;
            branchOffice.Image = ImageHelper.SaveImageToServer(httpRequest.Files[0]);

            unitOfWork.BranchOffices.Update(branchOffice);
            unitOfWork.Complete();

            return Ok("BranchOffice successfully edited");
        }

        // POST: api/BranchOffices/PostBranchOffice
        [HttpPost]
        [Route("PostBranchOffice")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IHttpActionResult> PostBranchOffice(CreateBranchOfficeBindingModel model)
        {

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            HttpRequest httpRequest = HttpContext.Current.Request;

            if (!ImageHelper.ValidateImage(httpRequest.Files[0], out validationErrorMessage))
            {
                return BadRequest(validationErrorMessage);
            }

            RAIdentityUser user = await UserManager.FindByIdAsync(User.Identity.GetUserId());

            Service service = unitOfWork.Services.Get(model.ServiceId);

            if (service == null)
            {
                return BadRequest("Service don't exists.");
            }

            if (UserManager.IsInRole(user.Id, "Manager") && !UserManager.IsInRole(user.Id, "Admin"))
            {
                if (service.Creator != user.Id)
                {
                    return BadRequest("Access denied.");
                }
            }

            BranchOffice branchOffice = new BranchOffice
            {
                Address = model.Address,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                Image = ImageHelper.SaveImageToServer(httpRequest.Files[0])
            };

            service.BranchOfficces.Add(branchOffice);

            unitOfWork.Services.Update(service);

            unitOfWork.BranchOffices.Add(branchOffice);
            unitOfWork.Complete();

            return Ok("Branch office successfully created");
        }

        // DELETE: api/BranchOffices/DeleteBranchOffice
        [HttpDelete]
        [Route("DeleteBranchOffice")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IHttpActionResult> DeleteBranchOffice([FromUri] long serviceId, [FromUri] long branchOfficeId)
        {
            BranchOffice branchOffice = unitOfWork.BranchOffices.Get(branchOfficeId);
            if (branchOffice == null)
            {
                return NotFound();
            }

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

            service.BranchOfficces.Remove(branchOffice);

            ImageHelper.DeleteImage(branchOffice.Image);

            unitOfWork.BranchOffices.Remove(branchOffice);
            unitOfWork.Complete();

            return Ok("Branch office successfully deleted.");
        }

        private bool BranchOfficeExists(long id)
        {
            return unitOfWork.BranchOffices.Get(id) != null;
        }

    }
}