﻿using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.FileProviders;
using System;
using System.IO;

namespace RentVehicle.Helpers
{
    public static class ImageHelper
    {

        public static Stream ReadImageFromServer(string root, string folderPath, string imageId)
        {
            IFileProvider provider = new PhysicalFileProvider(root);
            IFileInfo fileInfo = provider.GetFileInfo(folderPath + imageId);

            return fileInfo.CreateReadStream();
        }

        public static void UploadImageToServer(string root, string folderPath, IFormFile image)
        {
            string pathToFolder = System.IO.Path.Combine(root, folderPath);
            string filePath = Path.Combine(pathToFolder, image.FileName);

            using (FileStream fileStream = new FileStream(filePath, FileMode.Create))
            {
                image.CopyToAsync(fileStream);
            }
        }

        public static void DeleteImage(string root,string folderPath, string imageId)
        {
            IFileProvider provider = new PhysicalFileProvider(root);
            IFileInfo fileInfo = provider.GetFileInfo(folderPath + imageId);
            if (File.Exists(fileInfo.PhysicalPath))
            {
                File.Delete(fileInfo.PhysicalPath);
            }
        }

        public static void DeleteImages(string root, string folderPath, string images)
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
