using System.Data.Entity.Infrastructure;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using RentApp.Models.Entities;
using RentApp.Persistance.UnitOfWork;
using System.Collections.Generic;
using static RentApp.Models.VehicleBindingModel;
using System.Web;
using System;
using System.Net.Http;
using System.Web.Hosting;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Http.Headers;
using RentApp.Helpers;
using System.Threading.Tasks;
using System.Linq;

namespace RentApp.Controllers
{
    [Authorize]
    [RoutePrefix("api/Vehicles")]
    public class VehiclesController : ApiController
    {
        private readonly IUnitOfWork unitOfWork;

        public VehiclesController(IUnitOfWork unitOfWork)
        {
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
                unitOfWork.Vehicles.Update(vehicle);
                unitOfWork.Complete();
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

            Service service = unitOfWork.Services.Get(model.ServiceId);
            service.Vehicles.Add(vehicle);

            unitOfWork.Services.Update(service);
            unitOfWork.Vehicles.Add(vehicle);
            unitOfWork.Complete();

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

            unitOfWork.Vehicles.Remove(vehicle);
            unitOfWork.Complete();

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
    }
}