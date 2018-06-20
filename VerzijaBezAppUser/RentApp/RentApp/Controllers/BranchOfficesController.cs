using System.Data.Entity.Infrastructure;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using RentApp.Models.Entities;
using RentApp.Persistance.UnitOfWork;
using System.Collections.Generic;
using static RentApp.Models.BranchOfficeBindingModel;
using System.Web;
using System;
using System.Net.Http;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Http.Headers;
using System.Web.Hosting;

namespace RentApp.Controllers
{
    public class BranchOfficesController : ApiController
    {
        private string validationErrorMessage = "";
        private readonly IUnitOfWork unitOfWork;

        public BranchOfficesController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        // GET: api/BranchOffices
        public IEnumerable<BranchOffice> GetBranchOffices()
        {
            return unitOfWork.BranchOffices.GetAll();
        }

        // GET: api/BranchOffices/5
        [ResponseType(typeof(BranchOffice))]
        public IHttpActionResult GetBranchOffice(int id)
        {
            BranchOffice branchOffice = unitOfWork.BranchOffices.Get(id);
            if (branchOffice == null)
            {
                return NotFound();
            }

            return Ok(branchOffice);
        }

        // GET: api/BranchOffices
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

        // PUT: api/BranchOffices/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutBranchOffice(int id, BranchOffice branchOffice)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != branchOffice.Id)
            {
                return BadRequest();
            }

            try
            {
                unitOfWork.BranchOffices.Update(branchOffice);
                unitOfWork.Complete();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BranchOfficeExists(id))
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

        // POST: api/BranchOffices
        [ResponseType(typeof(BranchOffice))]
        public IHttpActionResult PostBranchOffice(CreateBranchOfficeBindingModel model)
        {
            HttpRequest httpRequest = HttpContext.Current.Request;

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!ValidateImage(httpRequest.Files[0]))
            {
                return BadRequest(validationErrorMessage);
            }

            BranchOffice branchOffice = new BranchOffice
            {
                Address = model.Address,
                Latitude = model.Latitude,
                Longitude = model.Longitude,
                Image = SaveImageToServer(httpRequest.Files[0])
            };

            unitOfWork.BranchOffices.Add(branchOffice);
            unitOfWork.Complete();

            return Ok("BranchOffice successfully created");
        }

        // DELETE: api/BranchOffices/5
        [ResponseType(typeof(BranchOffice))]
        public IHttpActionResult DeleteBranchOffice(int id)
        {
            BranchOffice branchOffice = unitOfWork.BranchOffices.Get(id);
            if (branchOffice == null)
            {
                return NotFound();
            }

            unitOfWork.BranchOffices.Remove(branchOffice);
            unitOfWork.Complete();

            return Ok(branchOffice);
        }

        private bool BranchOfficeExists(int id)
        {
            return unitOfWork.BranchOffices.Get(id) != null;
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