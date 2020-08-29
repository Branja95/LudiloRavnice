using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using System;
using System.IO;
using System.Threading.Tasks;

namespace AccountManaging.Helpers
{
    public class ImageHelper
    {
        private static readonly string folderPath = @"App_Data\";

        public static Stream ReadImageFromServer(string root, string imageId)
        {
            IFileProvider provider = new PhysicalFileProvider(root);
            IFileInfo fileInfo = provider.GetFileInfo(folderPath + imageId);

            return fileInfo.CreateReadStream();
        }

        public async Task<string>  UploadImageToServer(string root, IFormFile image)
        {
            string pathToFolder = System.IO.Path.Combine(root, "App_Data");
            string uniqueName = Guid.NewGuid().ToString() + "_" + image.FileName;
            string filePath = Path.Combine(pathToFolder, uniqueName);

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                await image.CopyToAsync(fileStream);
            }

            return uniqueName;
        }

        public static void DeleteImage(string root, string imageId)
        {
            IFileProvider provider = new PhysicalFileProvider(root);
            IFileInfo fileInfo = provider.GetFileInfo(folderPath + imageId);
            if (File.Exists(fileInfo.PhysicalPath))
            {
                File.Delete(fileInfo.PhysicalPath);
            }
        }

        public static void DeleteImages(string root, string images)
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
