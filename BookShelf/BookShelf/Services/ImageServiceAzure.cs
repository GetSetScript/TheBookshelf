using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BookShelf.Models;
using Microsoft.AspNetCore.Http;

namespace BookShelf.Services
{
    public class ImageServiceAzure : IBookImageService
    {
        public string GenerateImagePath(string filePath)
        {
            throw new NotImplementedException();
        }

        public bool TryDeleteImage(string imagePath)
        {
            throw new NotImplementedException();
        }

        public bool TrySaveAndResizeImage(IFormFile image, int width, Book book)
        {
            throw new NotImplementedException();
        }
    }
}
