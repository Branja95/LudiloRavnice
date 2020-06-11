using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using System;
using System.IO;


namespace AccountManaging.Helpers
{
    public static class ImageHelper
    {
        private static readonly Random _randomNumber = new Random();
        private static readonly string folderPath = @"App_Data\";

        public static Stream ReadImageFromServer(string root, string imageId)
        {
            IFileProvider provider = new PhysicalFileProvider(root);
            IFileInfo fileInfo = provider.GetFileInfo(folderPath + imageId);

            return fileInfo.CreateReadStream();
        }

        public static string  UploadImageToServer(string root, IFormFile image)
        {
            string pathToFolder = System.IO.Path.Combine(root, "App_Data");
            string uniqueName = GetRandomNumber(1, 1000000) + image.FileName;
            string filePath = Path.Combine(pathToFolder, uniqueName);

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                image.CopyToAsync(fileStream);
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

        private static int GetRandomNumber(int min, int max)
        {
            lock (_randomNumber)
            {
                return _randomNumber.Next(min, max);
            }
        }
    }
}
