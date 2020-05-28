using System;
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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RentVehicle.Helpers;
using RentVehicle.Models.Entities;
using RentVehicle.Models.IdentityUsers;
using RentVehicle.Persistance.UnitOfWork;
using static RentVehicle.Models.Bindings.VehicleBindingModel;

namespace RentVehicle.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class VehicleController : ControllerBase
    {
        private static object lockObjectForVehicles = new object();

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;

        private readonly IHostingEnvironment _environment;
        private readonly IConfiguration _configuration;

        public VehicleController(UserManager<ApplicationUser> userManager,
            IUnitOfWork unitOfWork,
            IHostingEnvironment environment,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _unitOfWork = unitOfWork;

            _environment = environment;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("GetVehicles")]
        [AllowAnonymous]
        public IEnumerable<Vehicle> GetVehicles()
        {
            return _unitOfWork.Vehicles.GetAll();
        }


        [HttpGet]
        [Route("GetVehicle")]
        [AllowAnonymous]
        public IActionResult GetVehicle([FromForm] int id)
        {
            Vehicle vehicle = _unitOfWork.Vehicles.Get(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(vehicle);
            }
        }


        [HttpGet]
        [Route("SearchVehicles")]
        [AllowAnonymous]
        public IActionResult SearchVehicles([FromForm] int vehicleTypeId, [FromForm] double vehiclePriceFrom, [FromForm] double vehiclePriceTo, [FromForm] string vehicleManufactor, [FromForm] string vehicleModel)
        {
            List<Vehicle> vehicles = FilterSearch(vehicleTypeId, vehiclePriceFrom, vehiclePriceTo, vehicleManufactor, vehicleModel);
            return Ok(vehicles);
        }


        [HttpGet]
        [Route("SearchNumberOfVehicles")]
        [AllowAnonymous]
        public IActionResult SearchNumberOfVehicles([FromForm] int vehicleTypeId, [FromForm] double vehiclePriceFrom, [FromForm] double vehiclePriceTo, [FromForm] string vehicleManufactor, [FromForm] string vehicleModel)
        {
            List<Vehicle> vehicles = FilterSearch(vehicleTypeId, vehiclePriceFrom, vehiclePriceTo, vehicleManufactor, vehicleModel);
            return Ok(vehicles.Count);
        }


        [HttpGet]
        [Route("GetNumberOfVehicles")]
        [AllowAnonymous]
        public IActionResult GetNumberOfVehicles()
        {
            IEnumerable<Vehicle> vehicles = _unitOfWork.Vehicles.GetAll();
            if (vehicles == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(vehicles.ToList().Count);
            }
        }

        [HttpGet]
        [Route("GetNumberOfVehicles")]
        [AllowAnonymous]
        public IActionResult GetNumberOfVehicles([FromForm] int pageIndex, [FromForm] int pageSize)
        {
            IEnumerable<Vehicle> vehicles = _unitOfWork.Vehicles.GetAll();
            if (vehicles == null)
            {
                return NotFound();
            }

            List<Vehicle> vehiclesList = new List<Vehicle>(vehicles);

            return Ok(vehiclesList.Count);
        }


        [HttpGet]
        [Route("GetSearchPagedVehicles")]
        [AllowAnonymous]
        public IActionResult GetSearchPagedVehicles([FromForm] int pageIndex, [FromForm] int pageSize, [FromForm] int vehicleTypeId, [FromForm] double vehiclePriceFrom, [FromForm] double vehiclePriceTo, [FromForm] string vehicleManufactor, [FromForm] string vehicleModel)
        {
            List<Vehicle> vehicles = FilterSearch(vehicleTypeId, vehiclePriceFrom, vehiclePriceTo, vehicleManufactor, vehicleModel);
            if (vehicles == null)
            {
                return NotFound();
            }

            List<Vehicle> vehiclesList = vehicles.OrderBy(vehicle => vehicle.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return Ok(vehiclesList);
        }


        [HttpGet]
        [Route("GetPagedVehicles")]
        [AllowAnonymous]
        public IActionResult GetPagedVehicles([FromForm] int pageIndex, [FromForm] int pageSize)
        {
            IEnumerable<Vehicle> vehicles = _unitOfWork.Vehicles.GetAll(pageIndex, pageSize);
            if (vehicles == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(vehicles.ToList());
            }
        }


        // GET: api/Vehicles/LoadImage
        [HttpGet]
        [Route("LoadImage")]
        [AllowAnonymous]
        public IActionResult LoadImage([FromForm] string imageId)
        {
            Stream readStream = ImageHelper.ReadImageFromServer(_environment.WebRootPath, imageId);
            return File(readStream, "image/jpeg");
        }

        // PUT: api/Vehicles/ChangeAvailability/5
        [HttpPut]
        [Route("ChangeAvailability")]
        [Authorize(Roles = "Administrator, Manager")]
        public async Task<IActionResult> ChangeAvailability(VehicleIdBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (_userManager.IsInRoleAsync(user, "Manager").Result && !_userManager.IsInRoleAsync(user, "Administrator").Result)
            {
                IEnumerable<Service> services = _unitOfWork.Services.GetAll();
                foreach (Service service in services)
                {
                    if (service.Vehicles.Find(v => v.Id == model.VehicleId) != null && service.Creator != user.Id)
                    {
                        return BadRequest("U can't edit vehicle at this service.");
                    }
                }
            }

            try
            {
                lock (lockObjectForVehicles)
                {
                    Vehicle vehicle = _unitOfWork.Vehicles.Get(model.VehicleId);
                    vehicle.IsAvailable = !vehicle.IsAvailable;
                    _unitOfWork.Vehicles.Update(vehicle);
                    _unitOfWork.Complete();
                }
            }
            catch (DBConcurrencyException)
            {
                return NotFound();
            }

            return Ok("Availability changed successfully.");
        }


        // PUT: api/Vehicles/PutVehicle/5
        [HttpPut]
        [Route("PutVehicle")]
        [Authorize(Roles = "Administrator, Manager")]
        public async Task<IActionResult> PutVehicle([FromForm] int id, EditVehicleBindingModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Vehicle vehicle = null;
            try
            {
                vehicle = _unitOfWork.Vehicles.Get(id);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VehicleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            if (vehicle == null)
            {
                return BadRequest();
            }

            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (_userManager.IsInRoleAsync(user, "Manager").Result && !_userManager.IsInRoleAsync(user, "Administrator").Result)
            {
                IEnumerable<Service> services = _unitOfWork.Services.GetAll();

                foreach (Service service in services)
                {
                    if (service.Vehicles.Find(v => v.Id == vehicle.Id) != null && service.Creator != user.Id)
                    {
                        return BadRequest("U can't edit vehicle at this service.");
                    }
                }
            }

            string[] oldImageUris = vehicle.Images.Split(new string[] { ";_;" }, StringSplitOptions.None);

            foreach (string imageId in oldImageUris)
            {
                ImageHelper.DeleteImage(_environment.WebRootPath, imageId);
            }

            string imageUris = string.Empty;
            int count = 0;

            /*
            foreach (string file in httpRequest.Files)
            {
                count++;
                HttpPostedFile uploadedImage = httpRequest.Files[file];

                string validationErrorMessage = string.Empty;

                if (ImageHelper.ValidateImage(uploadedImage, out validationErrorMessage))
                {
                    imageUris += ImageHelper.SaveImageToServer(uploadedImage);

                    if (count < httpRequest.Files.Count)
                    {
                        imageUris += ";_;";
                    }
                }
                else
                {
                    BadRequest(validationErrorMessage);
                }
            }
            */
            VehicleType vehicleType = _unitOfWork.VehicleTypes.Get(model.VehicleTypeId);

            vehicle.Description = model.Description;
            vehicle.Model = model.Model;
            vehicle.Manufactor = model.Manufactor;
            vehicle.PricePerHour = model.PricePerHour;
            vehicle.YearMade = model.YearMade;
            vehicle.IsAvailable = model.IsAvailable.Equals("Available") ? true : false;
            vehicle.Images = imageUris;
            vehicle.VehicleType = vehicleType;

            try
            {
                lock (lockObjectForVehicles)
                {
                    _unitOfWork.Vehicles.Update(vehicle);
                    _unitOfWork.Complete();
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VehicleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok("Vehicle successfully updated.");
        }

        // POST: api/Vehicles/PostVehicle/
        [HttpPost]
        [Route("PostVehicle")]
        [Authorize(Roles = "Administrator, Manager")]
        public async Task<IActionResult> PostVehicle(CreateVehicleBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            /*
            if (httpRequest.Files == null)
            {
                return BadRequest("Images cannot be left blank\n");
            }
            */

            Service service = _unitOfWork.Services.Get(model.ServiceId);
            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (_userManager.IsInRoleAsync(user, "Manager").Result && !_userManager.IsInRoleAsync(user, "Administrator").Result && service.Creator != user.Id)
            {
                return BadRequest("U can't edit vehicle at this service.");
            }

            string imageUris = string.Empty;
            int count = 0;

            /*
            foreach (string file in httpRequest.Files)
            {
                count++;
                HttpPostedFile uploadedImage = httpRequest.Files[file];

                string validationErrorMessage = string.Empty;

                if (ImageHelper.ValidateImage(uploadedImage, out validationErrorMessage))
                {
                    imageUris += ImageHelper.SaveImageToServer(uploadedImage);

                    if (count < httpRequest.Files.Count)
                    {
                        imageUris += ";_;";
                    }
                }
                else
                {
                    return BadRequest(validationErrorMessage);
                }
            }
            */
            VehicleType vehicleType = _unitOfWork.VehicleTypes.Get(model.VehicleTypeId);

            Vehicle vehicle = new Vehicle
            {
                Description = model.Description,
                Model = model.Model,
                Manufactor = model.Manufactor,
                PricePerHour = model.PricePerHour,
                YearMade = model.YearMade,
                IsAvailable = model.IsAvailable.Equals("IsAvailable") ? true : false,
                Images = imageUris,
                VehicleType = vehicleType,
            };


            try
            {
                lock (lockObjectForVehicles)
                {
                    service.Vehicles.Add(vehicle);
                    _unitOfWork.Services.Update(service);
                    _unitOfWork.Vehicles.Add(vehicle);
                    _unitOfWork.Complete();
                }
            }
            catch (DBConcurrencyException)
            {
                return NotFound();
            }

            return Ok("Vehicle successfully created.");

        }

        // DELETE: api/Vehicles/DeleteVehicle/5
        [HttpDelete]
        [Route("DeleteVehicle")]
        [Authorize(Roles = "Administrator, Manager")]
        public async Task<IActionResult> DeleteVehicle([FromForm] int id)
        {

            Vehicle vehicle = _unitOfWork.Vehicles.Get(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            ApplicationUser user = await _userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (_userManager.IsInRoleAsync(user, "Manager").Result && !_userManager.IsInRoleAsync(user, "Administrator").Result)
            {
                IEnumerable<Service> services = _unitOfWork.Services.GetAll();

                foreach (Service service in services)
                {
                    if (service.Vehicles.Find(v => v.Id == vehicle.Id) != null && service.Creator != user.Id)
                    {
                        return BadRequest("U can't edit vehicle at this service.");
                    }
                }
            }

            try
            {
                lock (lockObjectForVehicles)
                {
                    _unitOfWork.Vehicles.Remove(vehicle);
                    _unitOfWork.Complete();
                }
            }
            catch (DBConcurrencyException)
            {
                return NotFound();
            }

            return Ok($"Vehicle with id { vehicle.Id } successfully deleted.");
        }

        private bool VehicleExists(int id)
        {
            return _unitOfWork.Vehicles.Get(id) != null;
        }

        private VehicleType GetVehicleType(string vehicleType)
        {
            VehicleType vehicle = new VehicleType
            {
                TypeName = vehicleType
            };

            return vehicle;
        }

        private List<Vehicle> FilterSearch(int vehicleTypeId, double vehiclePriceFrom, double vehiclePriceTo, string vehicleManufactor, string vehicleModel)
        {
            List<Vehicle> vehicles;

            if (vehicleTypeId != -1 && vehiclePriceFrom != -1 && vehiclePriceTo != -1 && vehicleManufactor != "-1" && vehicleModel != "-1")
            {
                vehicles = _unitOfWork.Vehicles.Find(vehicle => vehicle.VehicleType.Id == vehicleTypeId &&
                                                                vehicle.PricePerHour >= vehiclePriceFrom &&
                                                                vehicle.PricePerHour <= vehiclePriceTo &&
                                                                vehicle.Manufactor == vehicleManufactor &&
                                                                vehicle.Model == vehicleModel
                                                                ).ToList();
            }
            else if (vehicleTypeId != -1 && vehiclePriceFrom != -1 && vehiclePriceTo != -1 && vehicleManufactor != "-1")
            {
                vehicles = _unitOfWork.Vehicles.Find(vehicle => vehicle.VehicleType.Id == vehicleTypeId &&
                                                                vehicle.PricePerHour >= vehiclePriceFrom &&
                                                                vehicle.PricePerHour <= vehiclePriceTo &&
                                                                vehicle.Manufactor == vehicleManufactor
                                                                ).ToList();
            }
            else if (vehicleTypeId != -1 && vehiclePriceFrom != -1 && vehiclePriceTo != -1 && vehicleManufactor != "-1")
            {
                vehicles = _unitOfWork.Vehicles.Find(vehicle => vehicle.VehicleType.Id == vehicleTypeId &&
                                                                vehicle.PricePerHour >= vehiclePriceFrom &&
                                                                vehicle.PricePerHour <= vehiclePriceTo &&
                                                                vehicle.Manufactor == vehicleManufactor
                                                                ).ToList();
            }
            else if (vehiclePriceFrom != -1 && vehiclePriceTo != -1 && vehicleModel != "-1" && vehicleManufactor != "-1")
            {
                vehicles = _unitOfWork.Vehicles.Find(vehicle => vehicle.VehicleType.Id == vehicleTypeId &&
                                                                vehicle.PricePerHour >= vehiclePriceFrom &&
                                                                vehicle.PricePerHour <= vehiclePriceTo &&
                                                                vehicle.Model == vehicleModel
                                                                ).ToList();
            }
            else if (vehicleTypeId != -1 && vehicleManufactor != "-1" && vehicleModel != "-1")
            {
                vehicles = _unitOfWork.Vehicles.Find(vehicle => vehicle.VehicleType.Id == vehicleTypeId &&
                                                                vehicle.Manufactor == vehicleManufactor &&
                                                                vehicle.Model == vehicleModel
                                                                ).ToList();
            }
            else if (vehiclePriceFrom != -1 && vehiclePriceTo != -1 && vehicleManufactor != "-1" && vehicleModel != "-1")
            {
                vehicles = _unitOfWork.Vehicles.Find(vehicle => vehicle.PricePerHour >= vehiclePriceFrom &&
                                                                vehicle.PricePerHour <= vehiclePriceTo &&
                                                                vehicle.Manufactor == vehicleManufactor &&
                                                                vehicle.Model == vehicleModel
                                                                ).ToList();
            }
            else if (vehicleTypeId != -1 && vehiclePriceFrom != -1 && vehiclePriceTo != -1)
            {
                vehicles = _unitOfWork.Vehicles.Find(vehicle => vehicle.VehicleType.Id == vehicleTypeId &&
                                                                vehicle.PricePerHour >= vehiclePriceFrom &&
                                                                vehicle.PricePerHour <= vehiclePriceTo
                                                                ).ToList();
            }
            else if (vehicleTypeId != -1 && vehicleManufactor != "-1" && vehiclePriceTo != -1)
            {
                vehicles = _unitOfWork.Vehicles.Find(vehicle => vehicle.VehicleType.Id == vehicleTypeId &&
                                                                vehicle.Manufactor == vehicleManufactor &&
                                                                vehicle.PricePerHour <= vehiclePriceTo
                                                                ).ToList();
            }
            else if (vehicleTypeId != -1 && vehicleModel != "-1" && vehiclePriceTo != -1)
            {
                vehicles = _unitOfWork.Vehicles.Find(vehicle => vehicle.VehicleType.Id == vehicleTypeId &&
                                                               vehicle.Model == vehicleModel &&
                                                               vehicle.PricePerHour <= vehiclePriceTo
                                                               ).ToList();
            }
            else if (vehiclePriceFrom != -1 && vehiclePriceTo != -1 && vehicleManufactor != "-1")
            {
                vehicles = _unitOfWork.Vehicles.Find(vehicle => vehicle.PricePerHour >= vehiclePriceFrom &&
                                                               vehicle.PricePerHour <= vehiclePriceTo &&
                                                                vehicle.Manufactor == vehicleManufactor
                                                                ).ToList();
            }
            else if (vehiclePriceFrom != -1 && vehiclePriceTo != -1 && vehicleModel != "-1")
            {
                vehicles = _unitOfWork.Vehicles.Find(vehicle => vehicle.PricePerHour >= vehiclePriceFrom &&
                                                                vehicle.PricePerHour <= vehiclePriceTo &&
                                                                vehicle.Model == vehicleModel
                                                                ).ToList();
            }
            else if (vehicleManufactor != "-1" && vehicleModel != "-1" && vehiclePriceTo != -1)
            {
                vehicles = _unitOfWork.Vehicles.Find(vehicle => vehicle.Manufactor == vehicleManufactor &&
                                                                vehicle.PricePerHour <= vehiclePriceTo &&
                                                                vehicle.Model == vehicleModel
                                                                ).ToList();
            }
            else if (vehicleTypeId != -1 && vehiclePriceFrom != -1)
            {
                vehicles = _unitOfWork.Vehicles.Find(vehicle => vehicle.VehicleType.Id == vehicleTypeId &&
                                                               vehicle.PricePerHour >= vehiclePriceFrom &&
                                                               vehicle.Model == vehicleModel
                                                               ).ToList();
            }
            else if (vehicleTypeId != -1 && vehiclePriceTo != -1)
            {
                vehicles = _unitOfWork.Vehicles.Find(vehicle => vehicle.VehicleType.Id == vehicleTypeId &&
                                                               vehicle.PricePerHour <= vehiclePriceTo
                                                               ).ToList();
            }
            else if (vehicleTypeId != -1 && vehicleManufactor != "-1")
            {
                vehicles = _unitOfWork.Vehicles.Find(vehicle => vehicle.VehicleType.Id == vehicleTypeId &&
                                                               vehicle.PricePerHour >= vehiclePriceFrom &&
                                                               vehicle.Model == vehicleModel
                                                               ).ToList();
            }
            else if (vehicleTypeId != -1 && vehicleModel != "-1")
            {
                vehicles = _unitOfWork.Vehicles.Find(vehicle => vehicle.VehicleType.Id == vehicleTypeId &&
                                                               vehicle.Model == vehicleModel
                                                               ).ToList();
            }
            else if (vehiclePriceFrom != -1 && vehiclePriceTo != -1)
            {
                vehicles = _unitOfWork.Vehicles.Find(vehicle => vehicle.PricePerHour >= vehiclePriceFrom &&
                                                               vehicle.PricePerHour <= vehiclePriceTo
                                                               ).ToList();
            }
            else if (vehiclePriceFrom != -1 && vehicleManufactor != "-1")
            {
                vehicles = _unitOfWork.Vehicles.Find(vehicle => vehicle.PricePerHour >= vehiclePriceFrom &&
                                                               vehicle.Manufactor == vehicleManufactor
                                                               ).ToList();
            }
            else if (vehiclePriceFrom != -1 && vehicleModel != "-1")
            {
                vehicles = _unitOfWork.Vehicles.Find(vehicle => vehicle.PricePerHour >= vehiclePriceFrom &&
                                                               vehicle.Model == vehicleModel
                                                               ).ToList();
            }
            else if (vehiclePriceTo != -1 && vehicleManufactor != "-1")
            {
                vehicles = _unitOfWork.Vehicles.Find(vehicle => vehicle.PricePerHour <= vehiclePriceTo &&
                                                               vehicle.Manufactor == vehicleManufactor
                                                               ).ToList();
            }
            else if (vehiclePriceTo != -1 && vehicleModel != "-1")
            {
                vehicles = _unitOfWork.Vehicles.Find(vehicle => vehicle.PricePerHour <= vehiclePriceTo &&
                                                               vehicle.Model == vehicleModel
                                                               ).ToList();
            }
            else if (vehicleManufactor != "-1" && vehicleModel != "-1")
            {
                vehicles = _unitOfWork.Vehicles.Find(vehicle => vehicle.Manufactor == vehicleManufactor &&
                                                               vehicle.Model == vehicleModel
                                                               ).ToList();
            }
            else if (vehicleTypeId != -1)
            {
                vehicles = _unitOfWork.Vehicles.Find(vehicle => vehicle.VehicleType.Id == vehicleTypeId).ToList();
            }
            else if (vehiclePriceFrom != -1)
            {
                vehicles = _unitOfWork.Vehicles.Find(vehicle => vehicle.PricePerHour >= vehiclePriceFrom).ToList();
            }
            else if (vehiclePriceTo != -1)
            {
                vehicles = _unitOfWork.Vehicles.Find(vehicle => vehicle.PricePerHour <= vehiclePriceTo).ToList();
            }
            else if (vehicleManufactor != "-1")
            {
                vehicles = _unitOfWork.Vehicles.Find(vehicle => vehicle.Manufactor == vehicleManufactor).ToList();
            }
            else if (vehicleModel != "-1")
            {
                vehicles = _unitOfWork.Vehicles.Find(vehicle => vehicle.Model == vehicleModel).ToList();
            }
            else
            {
                vehicles = _unitOfWork.Vehicles.GetAll().ToList();
            }

            return vehicles;
        }
    }
}