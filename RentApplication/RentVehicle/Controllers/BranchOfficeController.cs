using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
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
        private static object lockObjectForBranchOffices = new object();

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        private readonly IHostingEnvironment _environment;
        private readonly IConfiguration _configuration;

        public BranchOfficeController(UserManager<ApplicationUser> userManager,
           IUnitOfWork unitOfWork,
           IHostingEnvironment environment,
           IConfiguration configuration)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _environment = environment;
            _configuration = configuration;
        }

        // GET: api/BranchOffices/GetBranchOffices
        [HttpGet]
        [AllowAnonymous]
        [Route("GetBranchOffices")]
        public IActionResult GetBranchOffices()
        {
            IEnumerable<BranchOffice> branchOffices = _unitOfWork.BranchOffices.GetAll();
            return Ok(branchOffices.ToList());
        }

        // GET: api/BranchOffices/GetBranchOffices/5
        [HttpGet]
        [AllowAnonymous]
        [Route("GetVehicleBranchOffices")]
        public IActionResult GetVehicleBranchOffices([FromForm] long vehicleId)
        {
            Vehicle vehicle = _unitOfWork.Vehicles.Get(vehicleId);
            if (vehicle == null)
            {
                return NotFound();
            }

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

        // GET: api/BranchOffices/GetBranchOffice
        [HttpGet]
        [AllowAnonymous]
        [Route("GetBranchOffice")]
        public IActionResult GetBranchOffice([FromForm] long branchOfficeId)
        {
            BranchOffice branchOffice = _unitOfWork.BranchOffices.Get(branchOfficeId);
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
        public IActionResult LoadImage([FromForm] string imageId)
        {
            Stream readStream =  ImageHelper.ReadImageFromServer(_environment.WebRootPath, imageId);
            return File(readStream, "image/jpeg");
        }

        // PUT: api/BranchOffices/PutBranchOffice
        [HttpPut]
        [Route("PutBranchOffice")]
        [Authorize(Roles = "Administrator, Manager")]
        public async Task<IActionResult> PutBranchOffice([FromForm] long serviceId, EditBranchOfficeBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            Service serviceForValidation = _unitOfWork.Services.Get(serviceId);
            if (serviceForValidation == null)
            {
                return BadRequest("Service don't exists.");
            }
            if (_userManager.IsInRoleAsync(new ApplicationUser { Id = user.Id }, "Manager").Result && _userManager.IsInRoleAsync(new ApplicationUser { Id = user.Id }, "Administrator").Result && serviceForValidation.Creator != user.Id)
            {
                return BadRequest("Access denied.");
            }

            BranchOffice branchOffice = _unitOfWork.BranchOffices.Get(model.Id);

            if (branchOffice == null)
            {
                return BadRequest("Branch office don't exists.");
            }

            if (!serviceForValidation.BranchOfficces.Contains(branchOffice))
            {
                return BadRequest("Branch office don't exists.");
            }

            ImageHelper.DeleteImage(_environment.WebRootPath, branchOffice.Image);

            branchOffice.Address = model.Address;
            branchOffice.Latitude = model.Latitude;
            branchOffice.Longitude = model.Longitude;
            /*
            branchOffice.Image = ImageHelper.UploadImageToServer(_environment.WebRootPath, model.);
            */
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

            return Ok("BranchOffice successfully edited");
        }

        // POST: api/BranchOffices/PostBranchOffice
        [HttpPost]
        [Route("PostBranchOffice")]
        [Authorize(Roles = "Administrator, Manager")]
        public async Task<IActionResult> PostBranchOffice(CreateBranchOfficeBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            Service service = _unitOfWork.Services.Get(model.ServiceId);
            if (service == null)
            {
                return BadRequest("Service don't exists.");
            }

            if (_userManager.IsInRoleAsync(new ApplicationUser { Id = user.Id }, "Manager").Result && !_userManager.IsInRoleAsync(new ApplicationUser { Id = user.Id }, "Administrator").Result && service.Creator != user.Id)
            {
                return BadRequest("Access denied.");
            }

            BranchOffice branchOffice = new BranchOffice
            {
                Address = model.Address,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                //Image = ImageHelper.SaveImageToServer(httpRequest.Files[0])
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

            return Ok("Branch office successfully created");
        }

        // DELETE: api/BranchOffices/DeleteBranchOffice
        [HttpDelete]
        [Route("DeleteBranchOffice")]
        [Authorize(Roles = "Administrator, Manager")]
        public async Task<IActionResult> DeleteBranchOffice([FromForm] long serviceId, [FromForm] long branchOfficeId)
        {
            BranchOffice branchOffice = _unitOfWork.BranchOffices.Get(branchOfficeId);
            if (branchOffice == null)
            {
                return NotFound();
            }

            Service service = _unitOfWork.Services.Get(serviceId);
            if (service == null)
            {
                return NotFound();
            }

            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (_userManager.IsInRoleAsync(new ApplicationUser { Id = user.Id }, "Manager").Result && !_userManager.IsInRoleAsync(new ApplicationUser { Id = user.Id }, "Administrator").Result && service.Creator != user.Id)
            {
                return BadRequest("Access denied.");
            }

            //ImageHelper.DeleteImage(branchOffice.Image);

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

            return Ok("Branch office successfully deleted.");
        }

        private bool BranchOfficeExists(long id)
        {
            return _unitOfWork.BranchOffices.Get(id) != null;
        }

    }
}