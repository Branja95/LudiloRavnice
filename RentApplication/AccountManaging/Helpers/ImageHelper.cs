﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using System;
using System.IO;


namespace AccountManaging.Helpers
{
    public static class ImageHelper
    {
        private static readonly string folderPath = @"App_Data\";

        public static Stream ReadImageFromServer(string root, string imageId)
        {
            IFileProvider provider = new PhysicalFileProvider(root);
            IFileInfo fileInfo = provider.GetFileInfo(folderPath + imageId);

            return fileInfo.CreateReadStream();
        }

        public static void  UploadImageToServer(string root, IFormFile image)
        {
            string pathToFolder = System.IO.Path.Combine(root, "App_Data");
            string filePath = Path.Combine(pathToFolder, image.FileName);

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                image.CopyToAsync(fileStream);
            }
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
