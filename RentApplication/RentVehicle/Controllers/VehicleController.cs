using System;
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
using Microsoft.AspNetCore.Http;
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

        private static readonly string folderPath = @"App_Data\vehicle\";

        private readonly string _isUserInRoleEndpoint;
        private readonly string _findUserEndpoint;

        private readonly IUnitOfWork _unitOfWork;
        private readonly IHostingEnvironment _environment;
        private readonly IConfiguration _configuration;

        public VehicleController(IUnitOfWork unitOfWork,
            IHostingEnvironment environment,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _environment = environment;
            _configuration = configuration;

            _isUserInRoleEndpoint = configuration["AccountService:IsUserInRoleEndpoint"];
            _findUserEndpoint = configuration["AccountService:FindUserEndpoint"];
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
        public IActionResult GetVehicle([FromQuery] int id)
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
        public IActionResult SearchVehicles([FromQuery] int vehicleTypeId, [FromQuery] double vehiclePriceFrom, [FromQuery] double vehiclePriceTo, [FromQuery] string vehicleManufactor, [FromQuery] string vehicleModel)
        {
            List<Vehicle> vehicles = FilterSearch(vehicleTypeId, vehiclePriceFrom, vehiclePriceTo, vehicleManufactor, vehicleModel);
            return Ok(vehicles);
        }


        [HttpGet]
        [Route("SearchNumberOfVehicles")]
        [AllowAnonymous]
        public IActionResult SearchNumberOfVehicles([FromQuery] int vehicleTypeId, [FromQuery] double vehiclePriceFrom, [FromQuery] double vehiclePriceTo, [FromQuery] string vehicleManufactor, [FromQuery] string vehicleModel)
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
        [Route("GetSearchPagedVehicles")]
        [AllowAnonymous]
        public IActionResult GetSearchPagedVehicles([FromQuery] int pageIndex, [FromQuery] int pageSize, [FromQuery] int vehicleTypeId, [FromQuery] double vehiclePriceFrom, [FromQuery] double vehiclePriceTo, [FromQuery] string vehicleManufactor, [FromQuery] string vehicleModel)
        {
            List<Vehicle> vehicles = FilterSearch(vehicleTypeId, vehiclePriceFrom, vehiclePriceTo, vehicleManufactor, vehicleModel);
            if (vehicles == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(vehicles.OrderBy(vehicle => vehicle.Id).Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList());
            }
        }


        [HttpGet]
        [Route("GetPagedVehicles")]
        [AllowAnonymous]
        public IActionResult GetPagedVehicles([FromQuery] int pageIndex, [FromQuery] int pageSize)
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


        [HttpPut]
        [Route("ChangeAvailability")]
        [Authorize(Roles = "Administrator, Manager")]
        public async Task<IActionResult> ChangeAvailability([FromForm] VehicleIdBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
               
                if (IsUserInRole("Manager").Result && !IsUserInRole("Administrator").Result)
                {
                    ApplicationUser user = await FindUser();
                    if(user == null)
                    {
                        return BadRequest();
                    }
                    else
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
                }

                try
                {
                    lock (lockObjectForVehicles)
                    {
                        Vehicle vehicle = _unitOfWork.Vehicles.Get(model.VehicleId);
                        vehicle.IsAvailable = !vehicle.IsAvailable;
                        _unitOfWork.Vehicles.Update(vehicle);
                        _unitOfWork.Complete();

                        return Ok(_unitOfWork.Vehicles.GetAll());
                    }
                }
                catch (DBConcurrencyException)
                {
                    return NotFound();
                }
            }
        }


        [HttpPost]
        [Route("PostVehicle")]
        [Authorize(Roles = "Administrator, Manager")]
        public async Task<IActionResult> PostVehicle([FromForm] CreateVehicleBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                Service service = _unitOfWork.Services.Get(model.ServiceId);
                ApplicationUser user = await FindUser();
                if(user == null)
                {
                    return BadRequest();
                }
                else
                {
                    if (IsUserInRole("Manager").Result && !IsUserInRole("Administrator").Result && service.Creator != user.Id)
                    {
                        return BadRequest();
                    }

                    VehicleType vehicleType = _unitOfWork.VehicleTypes.Get(model.VehicleTypeId);

                    string imageUris = string.Empty;
                    int counter = 0;
                    foreach (IFormFile file in model.Images)
                    {
                        counter++;
                        ImageHelper.UploadImageToServer(_environment.WebRootPath, folderPath, file);
                        imageUris += file.FileName;
                        if (counter < model.Images.Count())
                        {
                            imageUris += ";_;";
                        }
                    }

                    Vehicle vehicle = new Vehicle
                    {
                        Description = model.Description,
                        Model = model.Model,
                        Manufactor = model.Manufactor,
                        PricePerHour = model.PricePerHour,
                        YearMade = model.YearMade,
                        IsAvailable = model.IsAvailable.Equals("IsAvailable"),
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

                    return Ok();
                }
            }
        }


        [HttpPut]
        [Route("PutVehicle")]
        [Authorize(Roles = "Administrator, Manager")]
        public async Task<IActionResult> PutVehicle([FromForm] EditVehicleBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                Vehicle vehicle = _unitOfWork.Vehicles.Get(model.Id);
                if (vehicle == null)
                {
                    return BadRequest();
                }

                ApplicationUser user = await FindUser();
                if(user == null)
                {
                    return BadRequest();
                }

                if (IsUserInRole("Manager").Result && !IsUserInRole("Administrator").Result)
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
                    ImageHelper.DeleteImage(_environment.WebRootPath, folderPath, imageId);
                }

                string imageUris = string.Empty;
                int count = 0;
                foreach (IFormFile file in model.Images)
                {
                    count++;
                    ImageHelper.UploadImageToServer(_environment.WebRootPath, folderPath, file);
                    imageUris += file.FileName;
                    if (count < model.Images.Count())
                    {
                        imageUris += ";_;";
                    }
                }

                vehicle.Description = model.Description;
                vehicle.Model = model.Model;
                vehicle.Manufactor = model.Manufactor;
                vehicle.PricePerHour = model.PricePerHour;
                vehicle.YearMade = model.YearMade;
                vehicle.IsAvailable = model.IsAvailable.Equals("Available");
                vehicle.Images = imageUris;
                vehicle.VehicleType = _unitOfWork.VehicleTypes.Get(model.VehicleTypeId);

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
                    if(_unitOfWork.Vehicles.Get(model.Id) == null)
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


        [HttpDelete]
        [Route("DeleteVehicle")]
        [Authorize(Roles = "Administrator, Manager")]
        public async Task<IActionResult> DeleteVehicle([FromQuery] int id)
        {
            Vehicle vehicle = _unitOfWork.Vehicles.Get(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            else
            {
                ApplicationUser user = await FindUser();
                if(user == null)
                {
                    return BadRequest();
                }

                if (IsUserInRole("Manager").Result && !IsUserInRole("Administrator").Result)
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

                return Ok();
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