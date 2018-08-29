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
using static RentApp.Models.VehicleBindingModel;
using System.Data;

namespace RentApp.Controllers
{
    [Authorize]
    [RoutePrefix("api/Vehicles")]
    public class VehiclesController : ApiController
    {
        private static object lockObjectForVehicles = new object();

        private readonly IUnitOfWork unitOfWork;

        public ApplicationUserManager UserManager { get; private set; }


        public VehiclesController(ApplicationUserManager userManager, 
            IUnitOfWork unitOfWork)
        {
            UserManager = userManager;
            this.unitOfWork = unitOfWork;
        }

        // GET: api/Vehicles/GetVehicles
        [HttpGet]
        [Route("GetVehicles")]
        [AllowAnonymous]
        public IEnumerable<Vehicle> GetVehicles()
        {
            return unitOfWork.Vehicles.GetAll();
        }

        // GET: api/Vehicles/GetVehicle/5
        [HttpGet]
        [Route("GetVehicle")]
        [AllowAnonymous]
        public IHttpActionResult GetVehicle([FromUri] int id)
        {
            Vehicle vehicle = unitOfWork.Vehicles.Get(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return Ok(vehicle);
        }

        // GET: api/Vehicles/SearchVehicles
        [HttpGet]
        [Route("SearchVehicles")]
        [AllowAnonymous]
        public IHttpActionResult SearchVehicles([FromUri] int vehicleTypeId, [FromUri] double vehiclePriceFrom, [FromUri] double vehiclePriceTo, [FromUri] string vehicleManufactor, [FromUri] string vehicleModel)
        {
            List<Vehicle> vehicles = FilterSearch(vehicleTypeId, vehiclePriceFrom, vehiclePriceTo, vehicleManufactor, vehicleModel);

            return Ok(vehicles);
        }

        // GET: api/Vehicles/SearchNumberOfVehicles
        [HttpGet]
        [Route("SearchNumberOfVehicles")]
        [AllowAnonymous]
        public IHttpActionResult SearchNumberOfVehicles([FromUri] int vehicleTypeId, [FromUri] double vehiclePriceFrom, [FromUri] double vehiclePriceTo, [FromUri] string vehicleManufactor, [FromUri] string vehicleModel)
        {
            List<Vehicle> vehicles = FilterSearch(vehicleTypeId, vehiclePriceFrom, vehiclePriceTo, vehicleManufactor, vehicleModel);
            
            return Ok(vehicles.Count);
        }

        // GET: api/Vehicles/GetNumberOfVehicles
        [HttpGet]
        [Route("GetNumberOfVehicles")]
        [AllowAnonymous]
        public IHttpActionResult GetNumberOfVehicles()
        {
            IEnumerable<Vehicle> vehicles = unitOfWork.Vehicles.GetAll();
            if (vehicles == null)
            {
                return NotFound();
            }

            List<Vehicle> vehiclesList = new List<Vehicle>(vehicles);

            return Ok(vehiclesList.Count);
        }


        // GET: api/Vehicles/GetSearchPagedVehicles
        [HttpGet]
        [Route("GetSearchPagedVehicles")]
        [AllowAnonymous]
        public IHttpActionResult GetSearchPagedVehicles([FromUri] int pageIndex, [FromUri] int pageSize, [FromUri] int vehicleTypeId, [FromUri] double vehiclePriceFrom, [FromUri] double vehiclePriceTo, [FromUri] string vehicleManufactor, [FromUri] string vehicleModel)
        {
            List<Vehicle> vehicles = FilterSearch(vehicleTypeId, vehiclePriceFrom, vehiclePriceTo, vehicleManufactor, vehicleModel);
            if (vehicles == null)
            {
                return NotFound();
            }

            List<Vehicle> vehiclesList = vehicles.OrderBy(vehicle => vehicle.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            
            return Ok(vehiclesList);
        }

        // GET: api/Vehicles/GetPagedVehicles
        [HttpGet]
        [Route("GetPagedVehicles")]
        [AllowAnonymous]
        public IHttpActionResult GetPagedVehicles([FromUri] int pageIndex, [FromUri] int pageSize)
        {
            IEnumerable<Vehicle> vehicles = unitOfWork.Vehicles.GetAll(pageIndex, pageSize);
            if (vehicles == null)
            {
                return NotFound();
            }

            List<Vehicle> vehiclesList = new List<Vehicle>(vehicles);

            return Ok(vehiclesList);
        }

        // GET: api/Vehicles/FilterVehicles
        [HttpGet]
        [Route("GetNumberOfVehicles")]
        [AllowAnonymous]
        public IHttpActionResult GetNumberOfVehicles([FromUri] int pageIndex, [FromUri] int pageSize)
        {
            IEnumerable<Vehicle> vehicles = unitOfWork.Vehicles.GetAll();
            if (vehicles == null)
            {
                return NotFound();
            }

            List<Vehicle> vehiclesList = new List<Vehicle>(vehicles);

            return Ok(vehiclesList.Count);
        }
        
        // GET: api/Vehicles/LoadImage
        [HttpGet]
        [Route("LoadImage")]
        [AllowAnonymous]
        public HttpResponseMessage LoadImage([FromUri] string imageId)
        {
            return ImageHelper.LoadImage(imageId);
        }

        // PUT: api/Vehicles/ChangeAvailability/5
        [HttpPut]
        [Route("ChangeAvailability")]
        [Authorize(Roles = "Admin, Manager")]
        public IHttpActionResult ChangeAvailability(VehicleIdBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (UserManager.IsInRole(User.Identity.GetUserId(), "Manager") && !UserManager.IsInRole(User.Identity.GetUserId(), "Admin"))
            {
                IEnumerable<Service> services = unitOfWork.Services.GetAll();

                foreach (Service service in services)
                {
                    if (service.Vehicles.Find(v => v.Id == model.VehicleId) != null)
                    {
                        if (service.Creator != User.Identity.GetUserId())
                        {
                            return BadRequest("U can't edit vehicle at this service.");
                        }
                    }
                }
            }

            try
            {
                lock (lockObjectForVehicles)
                {
                    Vehicle vehicle = unitOfWork.Vehicles.Get(model.VehicleId);
                    vehicle.IsAvailable = !vehicle.IsAvailable;
                    unitOfWork.Vehicles.Update(vehicle);
                    unitOfWork.Complete();
                }
            }
            catch(DBConcurrencyException)
            {
                return NotFound();
            }

            return Ok("Availability changed successfully.");
        }


        // PUT: api/Vehicles/PutVehicle/5
        [HttpPut]
        [Route("PutVehicle")]
        [Authorize(Roles = "Admin, Manager")]
        public IHttpActionResult PutVehicle([FromUri] int id, EditVehicleBindingModel model)
        {
            HttpRequest httpRequest = HttpContext.Current.Request;

            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (httpRequest.Files == null)
            {
                return BadRequest("Images cannot be left blank\n");
            }

            Vehicle vehicle = null;

            try
            {
                vehicle = unitOfWork.Vehicles.Get(id);
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

            if(vehicle == null)
            {
                return BadRequest();
            }

            if (UserManager.IsInRole(User.Identity.GetUserId(), "Manager") && !UserManager.IsInRole(User.Identity.GetUserId(), "Admin"))
            {
                IEnumerable<Service> services = unitOfWork.Services.GetAll();

                foreach (Service service in services)
                {
                    if (service.Vehicles.Find(v => v.Id == vehicle.Id) != null)
                    {
                        if (service.Creator != User.Identity.GetUserId())
                        {
                            return BadRequest("U can't edit vehicle at this service.");
                        }
                    }
                }
            }

            string[] oldImageUris = vehicle.Images.Split(new string[] { ";_;" }, StringSplitOptions.None);

            foreach(string imageId in oldImageUris)
            {
                ImageHelper.DeleteImage(imageId);
            }

            string imageUris = string.Empty;
            int count = 0;

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

            VehicleType vehicleType = unitOfWork.VehicleTypes.Get(model.VehicleTypeId);

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
                    unitOfWork.Vehicles.Update(vehicle);
                    unitOfWork.Complete();
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
        [Authorize(Roles = "Admin, Manager")]
        public IHttpActionResult PostVehicle(CreateVehicleBindingModel model)
        {
            HttpRequest httpRequest = HttpContext.Current.Request;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (httpRequest.Files == null)
            {
                return BadRequest("Images cannot be left blank\n");
            }

            Service service = unitOfWork.Services.Get(model.ServiceId);

            if (UserManager.IsInRole(User.Identity.GetUserId(), "Manager") && !UserManager.IsInRole(User.Identity.GetUserId(), "Admin"))
            {
                if (service.Creator != User.Identity.GetUserId())
                {
                    return BadRequest("U can't edit vehicle at this service.");
                }
            }

            string imageUris = string.Empty;
            int count = 0;

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

            VehicleType vehicleType = unitOfWork.VehicleTypes.Get(model.VehicleTypeId);

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
                    unitOfWork.Services.Update(service);
                    unitOfWork.Vehicles.Add(vehicle);
                    unitOfWork.Complete();
                }
            }
            catch(DBConcurrencyException)
            {
                return NotFound();
            }

            return Ok("Vehicle successfully created.");

        }

        // DELETE: api/Vehicles/DeleteVehicle/5
        [HttpDelete]
        [Route("DeleteVehicle")]
        [Authorize(Roles = "Admin, Manager")]
        public IHttpActionResult DeleteVehicle([FromUri] int id)
        {

            Vehicle vehicle = unitOfWork.Vehicles.Get(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            if (UserManager.IsInRole(User.Identity.GetUserId(), "Manager") && !UserManager.IsInRole(User.Identity.GetUserId(), "Admin"))
            {
                IEnumerable<Service> services = unitOfWork.Services.GetAll();

                foreach (Service service in services)
                {
                    if (service.Vehicles.Find(v => v.Id == vehicle.Id) != null)
                    {
                        if (service.Creator != User.Identity.GetUserId())
                        {
                            return BadRequest("U can't edit vehicle at this service.");
                        }
                    }
                }
            }

            try
            {
                lock (lockObjectForVehicles)
                {
                    unitOfWork.Vehicles.Remove(vehicle);
                    unitOfWork.Complete();
                }
            }
            catch(DBConcurrencyException)
            {
                return NotFound();
            }

            return Ok($"Vehicle with id { vehicle.Id } successfully deleted.");
        }

        private bool VehicleExists(int id)
        {
            return unitOfWork.Vehicles.Get(id) != null;
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
                vehicles = unitOfWork.Vehicles.Find(vehicle => vehicle.VehicleType.Id == vehicleTypeId &&
                                                                vehicle.PricePerHour >= vehiclePriceFrom &&
                                                                vehicle.PricePerHour <= vehiclePriceTo &&
                                                                vehicle.Manufactor == vehicleManufactor &&
                                                                vehicle.Model == vehicleModel
                                                                ).ToList();
            }
            else if (vehicleTypeId != -1 && vehiclePriceFrom  != -1 && vehiclePriceTo != -1 && vehicleManufactor != "-1")
            {
                vehicles = unitOfWork.Vehicles.Find(vehicle => vehicle.VehicleType.Id == vehicleTypeId &&
                                                                vehicle.PricePerHour >= vehiclePriceFrom &&
                                                                vehicle.PricePerHour <= vehiclePriceTo &&
                                                                vehicle.Manufactor == vehicleManufactor
                                                                ).ToList();
            }
            else if (vehicleTypeId != -1 && vehiclePriceFrom != -1 && vehiclePriceTo != -1 && vehicleManufactor != "-1")
            {
                vehicles = unitOfWork.Vehicles.Find(vehicle => vehicle.VehicleType.Id == vehicleTypeId &&
                                                                vehicle.PricePerHour >= vehiclePriceFrom &&
                                                                vehicle.PricePerHour <= vehiclePriceTo &&
                                                                vehicle.Manufactor == vehicleManufactor
                                                                ).ToList();
            }
            else if (vehiclePriceFrom != -1 && vehiclePriceTo != -1 && vehicleModel != "-1" && vehicleManufactor != "-1")
            {
                vehicles = unitOfWork.Vehicles.Find(vehicle => vehicle.VehicleType.Id == vehicleTypeId &&
                                                                vehicle.PricePerHour >= vehiclePriceFrom &&
                                                                vehicle.PricePerHour <= vehiclePriceTo &&
                                                                vehicle.Model == vehicleModel
                                                                ).ToList();
            }
            else if (vehicleTypeId != -1 && vehicleManufactor != "-1" && vehicleModel != "-1")
            {
                vehicles = unitOfWork.Vehicles.Find(vehicle => vehicle.VehicleType.Id == vehicleTypeId &&
                                                                vehicle.Manufactor == vehicleManufactor &&
                                                                vehicle.Model == vehicleModel
                                                                ).ToList();
            }
            else if (vehiclePriceFrom != -1 && vehiclePriceTo != -1 && vehicleManufactor != "-1" && vehicleModel != "-1")
            {
                vehicles = unitOfWork.Vehicles.Find(vehicle => vehicle.PricePerHour >= vehiclePriceFrom &&
                                                                vehicle.PricePerHour <= vehiclePriceTo &&
                                                                vehicle.Manufactor == vehicleManufactor &&
                                                                vehicle.Model == vehicleModel
                                                                ).ToList();
            }
            else if (vehicleTypeId != -1 && vehiclePriceFrom != -1 && vehiclePriceTo != -1)
            {
                vehicles = unitOfWork.Vehicles.Find(vehicle => vehicle.VehicleType.Id == vehicleTypeId &&
                                                                vehicle.PricePerHour >= vehiclePriceFrom &&
                                                                vehicle.PricePerHour <= vehiclePriceTo 
                                                                ).ToList();
            }
            else if (vehicleTypeId != -1 && vehicleManufactor != "-1" && vehiclePriceTo != -1)
            {
                vehicles = unitOfWork.Vehicles.Find(vehicle => vehicle.VehicleType.Id == vehicleTypeId &&
                                                                vehicle.Manufactor == vehicleManufactor &&
                                                                vehicle.PricePerHour <= vehiclePriceTo
                                                                ).ToList();
            }
            else if (vehicleTypeId != -1 && vehicleModel != "-1" && vehiclePriceTo != -1)
            {
                vehicles = unitOfWork.Vehicles.Find(vehicle => vehicle.VehicleType.Id == vehicleTypeId &&
                                                               vehicle.Model == vehicleModel &&
                                                               vehicle.PricePerHour <= vehiclePriceTo
                                                               ).ToList();
            }
            else if (vehiclePriceFrom != -1 && vehiclePriceTo != -1 && vehicleManufactor != "-1")
            {
                vehicles = unitOfWork.Vehicles.Find(vehicle => vehicle.PricePerHour >= vehiclePriceFrom &&
                                                               vehicle.PricePerHour <= vehiclePriceTo &&
                                                                vehicle.Manufactor == vehicleManufactor
                                                                ).ToList();
            }
            else if (vehiclePriceFrom != -1 && vehiclePriceTo != -1 && vehicleModel != "-1")
            {
                vehicles = unitOfWork.Vehicles.Find(vehicle => vehicle.PricePerHour >= vehiclePriceFrom &&
                                                                vehicle.PricePerHour <= vehiclePriceTo &&
                                                                vehicle.Model == vehicleModel
                                                                ).ToList();
            }
            else if (vehicleManufactor != "-1" && vehicleModel != "-1" && vehiclePriceTo != -1)
            {
                vehicles = unitOfWork.Vehicles.Find(vehicle => vehicle.Manufactor == vehicleManufactor &&
                                                                vehicle.PricePerHour <= vehiclePriceTo &&
                                                                vehicle.Model == vehicleModel
                                                                ).ToList();
            }
            else if (vehicleTypeId != -1 && vehiclePriceFrom != -1)
            {
                vehicles = unitOfWork.Vehicles.Find(vehicle => vehicle.VehicleType.Id == vehicleTypeId &&
                                                               vehicle.PricePerHour >= vehiclePriceFrom &&
                                                               vehicle.Model == vehicleModel
                                                               ).ToList();
            }
            else if (vehicleTypeId != -1 && vehiclePriceTo != -1)
            {
                vehicles = unitOfWork.Vehicles.Find(vehicle => vehicle.VehicleType.Id == vehicleTypeId &&
                                                               vehicle.PricePerHour <= vehiclePriceTo 
                                                               ).ToList();
            }
            else if (vehicleTypeId != -1 && vehicleManufactor != "-1")
            {
                vehicles = unitOfWork.Vehicles.Find(vehicle => vehicle.VehicleType.Id == vehicleTypeId &&
                                                               vehicle.PricePerHour >= vehiclePriceFrom &&
                                                               vehicle.Model == vehicleModel
                                                               ).ToList();
            }
            else if (vehicleTypeId != -1 && vehicleModel != "-1")
            {
                vehicles = unitOfWork.Vehicles.Find(vehicle => vehicle.VehicleType.Id == vehicleTypeId &&
                                                               vehicle.Model == vehicleModel
                                                               ).ToList();
            }
            else if (vehiclePriceFrom != -1 && vehiclePriceTo != -1)
            {
                vehicles = unitOfWork.Vehicles.Find(vehicle => vehicle.PricePerHour >= vehiclePriceFrom &&
                                                               vehicle.PricePerHour <= vehiclePriceTo 
                                                               ).ToList();
            }
            else if (vehiclePriceFrom != -1 && vehicleManufactor != "-1")
            {
                vehicles = unitOfWork.Vehicles.Find(vehicle => vehicle.PricePerHour >= vehiclePriceFrom &&
                                                               vehicle.Manufactor == vehicleManufactor
                                                               ).ToList();
            }
            else if (vehiclePriceFrom != -1 && vehicleModel != "-1")
            {
                vehicles = unitOfWork.Vehicles.Find(vehicle => vehicle.PricePerHour >= vehiclePriceFrom &&
                                                               vehicle.Model == vehicleModel
                                                               ).ToList();
            }
            else if (vehiclePriceTo != -1 && vehicleManufactor != "-1")
            {
                vehicles = unitOfWork.Vehicles.Find(vehicle => vehicle.PricePerHour <= vehiclePriceTo &&
                                                               vehicle.Manufactor == vehicleManufactor
                                                               ).ToList();
            }
            else if (vehiclePriceTo != -1 && vehicleModel != "-1")
            {
                vehicles = unitOfWork.Vehicles.Find(vehicle => vehicle.PricePerHour <= vehiclePriceTo &&
                                                               vehicle.Model == vehicleModel
                                                               ).ToList();
            }
            else if (vehicleManufactor != "-1" && vehicleModel != "-1")
            {
                vehicles = unitOfWork.Vehicles.Find(vehicle => vehicle.Manufactor == vehicleManufactor &&
                                                               vehicle.Model == vehicleModel
                                                               ).ToList();
            }
            else if (vehicleTypeId != -1)
            {
                vehicles = unitOfWork.Vehicles.Find(vehicle => vehicle.VehicleType.Id == vehicleTypeId).ToList();
            }
            else if (vehiclePriceFrom != -1)
            {
                vehicles = unitOfWork.Vehicles.Find(vehicle => vehicle.PricePerHour >= vehiclePriceFrom).ToList();
            }
            else if (vehiclePriceTo != -1)
            {
                vehicles = unitOfWork.Vehicles.Find(vehicle => vehicle.PricePerHour <= vehiclePriceTo).ToList();
            }
            else if (vehicleManufactor != "-1")
            {
                vehicles = unitOfWork.Vehicles.Find(vehicle => vehicle.Manufactor == vehicleManufactor).ToList();
            }
            else if (vehicleModel != "-1")
            {
                vehicles = unitOfWork.Vehicles.Find(vehicle => vehicle.Model == vehicleModel).ToList();
            }
            else
            {
                vehicles = unitOfWork.Vehicles.GetAll().ToList();
            }

            return vehicles;
        }
    }
}