﻿using System.Data.Entity.Infrastructure;
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

namespace RentApp.Controllers
{
    public class VehiclesController : ApiController
    {
        private string validationErrorMessage;
        private readonly IUnitOfWork unitOfWork;

        public VehiclesController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: api/Vehicles
        public IEnumerable<Vehicle> GetVehicles()
        {
            return unitOfWork.Vehicles.GetAll();
        }

        // GET: api/Vehicles/5
        [ResponseType(typeof(Vehicle))]
        public IHttpActionResult GetVehicle(int id)
        {
            Vehicle vehicle = unitOfWork.Vehicles.Get(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return Ok(vehicle);
        }

        // GET: api/Vehicles
        [HttpGet]
        public HttpResponseMessage LoadImage(string imageId)
        {
            HttpResponseMessage result;

            String filePath = HostingEnvironment.MapPath("~/App_Data/" + imageId);

            if (File.Exists(filePath))
            {
                result = new HttpResponseMessage(HttpStatusCode.OK);
                FileStream fileStream = new FileStream(filePath, FileMode.Open);
                Image image = Image.FromStream(fileStream);
                MemoryStream memoryStream = new MemoryStream();
                image.Save(memoryStream, ImageFormat.Jpeg);
                result.Content = new ByteArrayContent(memoryStream.ToArray());
                result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
                fileStream.Close();
            }
            else
            {
                result = new HttpResponseMessage(HttpStatusCode.BadRequest);
            }

            return result;
        }

        // PUT: api/Vehicles/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutVehicle(int id, Vehicle vehicle)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != vehicle.Id)
            {
                return BadRequest();
            }

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

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/Vehicles
        [ResponseType(typeof(Vehicle))]
        public IHttpActionResult PostVehicle(CreateVehicleBindingModel model)
        {
            HttpRequest httpRequest = HttpContext.Current.Request;

            string imageUris = string.Empty;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (httpRequest.Files == null)
            {
                return BadRequest("Images cannot be left blank\n");
            }

            foreach (string file in httpRequest.Files)
            {
                HttpPostedFile uploadedImage = httpRequest.Files[file];

                if (ValidateImage(uploadedImage))
                {
                    imageUris += SaveImageToServer(uploadedImage) + ";_;";
                }
            }

            Vehicle vehicle = new Vehicle
            {
                Description = model.Description,
                Model = model.Model,
                Manufactor = model.Manufactor,
                PricePerHour = model.PricePerHour,
                YearMade = model.YearMade,
                VehicleType = GetVehicleType(model.VehicleType),
                IsAvailable = model.IsAvailable.Equals("IsAvailable") ? true : false,
            };

            unitOfWork.Vehicles.Add(vehicle);
            unitOfWork.Complete();

            return Ok("Vehicle successfully created");

        }

        // DELETE: api/Vehicles/5
        [ResponseType(typeof(Vehicle))]
        public IHttpActionResult DeleteVehicle(int id)
        {
            Vehicle vehicle = unitOfWork.Vehicles.Get(id);
            if (vehicle == null)
            {
                return NotFound();
            }

            unitOfWork.Vehicles.Remove(vehicle);
            unitOfWork.Complete();

            return Ok(vehicle);
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

        private bool ValidateImage(HttpPostedFile image)
        {
            bool isImageValid = true;

            int maximumImageSize = 1024 * 1024 * 1;

            IList<string> allowedImageExtensions = new List<string> { ".jpg", ".gif", ".png" };

            validationErrorMessage = string.Empty;

            string extension = image.FileName.Substring(image.FileName.LastIndexOf('.'));

            if (image == null)
            {
                validationErrorMessage += "Image cannot be left blank!\n";
                isImageValid = false;
            }
            if (!allowedImageExtensions.Contains(extension.ToLower()))
            {
                validationErrorMessage += "Image format is not supported!\n";
                isImageValid = false;
            }
            if (image.ContentLength > maximumImageSize)
            {
                validationErrorMessage += "Image size is too big!\n";
                isImageValid = false;
            }

            return isImageValid;
        }

        private string SaveImageToServer(HttpPostedFile image)
        {
            string imageId = Guid.NewGuid() + image.FileName;
            string fileLocationOnServer = HttpContext.Current.Server.MapPath("~/App_Data/" + imageId);
            image.SaveAs(fileLocationOnServer);

            return imageId;
        }
    }
}