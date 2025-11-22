using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ProgressTrackingService.Helper
{
    public class ImageHelper
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ImageHelper(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public   string UploadImageAsync(IFormFile file, string folder)
        {
            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, folder);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var uniqueFileName = Guid.NewGuid().ToString() + file.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                 file.CopyToAsync(fileStream);
            }
            return $"{folder}/{uniqueFileName}".Replace("\\", "/");
        }
        public Task DeleteImageAsync(string imageUrl)
        {
           
            if (string.IsNullOrEmpty(imageUrl))
                return Task.CompletedTask;
            var uri = new Uri(imageUrl, UriKind.RelativeOrAbsolute);
            string relativePath = imageUrl;
            if (uri.IsAbsoluteUri)
            {
                relativePath = uri.AbsolutePath.TrimStart('/');
            }
            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
            if (File.Exists(fullPath)) File.Delete(fullPath);
            return Task.CompletedTask;


        }
        public string GetImagePath(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
                return null;
            var uri = new Uri(imageUrl, UriKind.RelativeOrAbsolute);
            string relativePath = imageUrl;
            if (uri.IsAbsoluteUri)
            {
                relativePath = uri.AbsolutePath.TrimStart('/');
            }
            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));
            return fullPath;
        }
        public Task UpdateImageAsync(IFormFile newFile, string existingImageUrl, string folder)
        {
            if (!string.IsNullOrEmpty(existingImageUrl))
            {
                DeleteImageAsync(existingImageUrl); 
            }
            UploadImageAsync(newFile, folder);
            return Task.CompletedTask;
        }


    }
}
