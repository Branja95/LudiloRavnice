using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using System;
using System.IO;
using System.Threading.Tasks;

namespace RentVehicle.Helpers
{
    public class ImageHelper
    {
        public static Stream ReadImageFromServer(string root, string folderPath, string imageId)
        {
            IFileProvider provider = new PhysicalFileProvider(root);
            IFileInfo fileInfo = provider.GetFileInfo(folderPath + imageId);

            return fileInfo.CreateReadStream();
        }

        public async Task<string> UploadImageToServer(string root, string folderPath, IFormFile image)
        {
            string pathToFolder = System.IO.Path.Combine(root, folderPath);
            string uniqueName = Guid.NewGuid().ToString() + "_" + image.FileName;
            string filePath = Path.Combine(pathToFolder, uniqueName);

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return uniqueName;
        }

        public async Task DeleteImage(string root,string folderPath, string imageId)
        {
            IFileProvider provider = new PhysicalFileProvider(root);
            IFileInfo fileInfo = provider.GetFileInfo(folderPath + imageId);
            if (File.Exists(fileInfo.PhysicalPath))
            {
                File.Delete(fileInfo.PhysicalPath);
            }
        }

        public async Task DeleteImages(string root, string folderPath, string images)
        {
            string[] imagesforDelete = images.Split(new string[] { ";_;" }, StringSplitOptions.None);

            foreach (string imageId in imagesforDelete)
            {
                IFileProvider provider = new PhysicalFileProvider(root);
                IFileInfo fileInfo = provider.GetFileInfo(folderPath + imageId);
                if (File.Exists(fileInfo.PhysicalPath))
                {
                    File.Delete(fileInfo.PhysicalPath);
                }
            }
        }
    }
}
