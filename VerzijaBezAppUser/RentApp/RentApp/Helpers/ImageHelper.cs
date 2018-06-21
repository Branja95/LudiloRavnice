using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Hosting;

namespace RentApp.Helpers
{
    public class ImageHelper
    {
        public static bool ValidateImage(HttpPostedFile image, out string validationErrorMessage)
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

        public static string SaveImageToServer(HttpPostedFile image)
        {
            string imageId = Guid.NewGuid() + image.FileName;
            string fileLocationOnServer = HttpContext.Current.Server.MapPath("~/App_Data/" + imageId);
            image.SaveAs(fileLocationOnServer);

            return imageId;
        }

        public static HttpResponseMessage LoadImage(string imageId)
        {
            HttpResponseMessage result;
            
            string filePath = HostingEnvironment.MapPath("~/App_Data/" + imageId);

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

        public static void DeleteImage(string imageId)
        {
            string filePath = HostingEnvironment.MapPath("~/App_Data/" + imageId);
            File.Delete(filePath);
        }
    }
}