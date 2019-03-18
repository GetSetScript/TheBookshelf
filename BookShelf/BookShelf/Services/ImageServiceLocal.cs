using BookShelf.Models;
using ImageMagick;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BookShelf.Services
{
    /// <summary>
    /// Represents a service for saving and deleting <see cref="Book"/> Images
    /// </summary>
    public class ImageServiceLocal : IBookImageService
    {
        private readonly ILogger<ImageServiceLocal> _logger;
        private readonly IHostingEnvironment _hosting;

        private readonly string _localFilePath;

        /// <summary>
        /// Creates a new instace of the <see cref="ImageServiceLocal"/>
        /// </summary>
        /// <param name="logger">Used for logging</param>
        /// <param name="hosting">Used for retrieving information about the application</param>
        public ImageServiceLocal(ILogger<ImageServiceLocal> logger, IHostingEnvironment hosting)
        {
            _logger = logger;
            _hosting = hosting;

            _localFilePath = Path.Combine(_hosting.WebRootPath, "images");
        }

        /// <summary>
        /// Resizes an <see cref="IFormFile"/> image and saves it to a file
        /// </summary>
        /// <param name="image">The image to save</param>
        /// <param name="width">The width to resize the image to, the aspect ratio is preserved</param>
        /// <param name="book">The book the image belongs with</param>
        /// <returns>True if the image was successfully saved and resized</returns>
        public bool TrySaveAndResizeImage(IFormFile image, int width, Book book)
        {
            if (image == null)
            {
                _logger.LogError("Failed to save @{image}, the image was null", image);
                return false;
            }

            if (image.Length <= 0)
            {
                _logger.LogError("Failed to save @{image}, the byte length of the image was @{image.Length}", image, image.Length);
                return false;
            }

            if (book == null)
            {
                _logger.LogError("Failed to save @{image}, the @{book} was null", image, book);
                return false;
            }

            if (book.ImagePath == null)
            {
                _logger.LogError("Failed to save @{image}, the @{book.ImagePath} was null", image, book.ImagePath);
                return false;
            }

            if (width <= 0)
            {
                _logger.LogError("Failed to save @{image}, The @{width} can't be 0 or negative", image, width);
                return false;
            }
            
            using (MagickImage images = new MagickImage(image.OpenReadStream()))
            {
                MagickGeometry size = new MagickGeometry(width, 0);

                images.Resize(size);

                var filePath = Path.Combine(_localFilePath, book.ImagePath);
                images.Write(filePath);

                _logger.LogDebug("Successfully resized the @{image} to @{width} and saved at @{filePath}", image, width, filePath);
                return true;
            }
        }

        /// <summary>
        /// Deletes the image file for a <see cref="Book"/> resource
        /// </summary>
        /// <param name="imageName">The name of the image file that will be deleted</param>
        /// <returns>True if the image file was successfully deleted</returns>
        public bool TryDeleteImage(string imageName)
        {
            if (imageName == null)
            {
                _logger.LogError("The @{imageName} was null", imageName);
                return false;
            }
            
            var filePath = Path.Combine(_localFilePath, imageName);

            if (!File.Exists(filePath))
            {
                _logger.LogError("The image file @{imageName} does not exist}", imageName);
                return false;
            }

            File.Delete(filePath);
            _logger.LogDebug("Successfully deleted the image file @{imageName} at @{filePath}", imageName, filePath);
            return true;
        }

        /// <summary>
        /// Generates a unique string for use as an Image path
        /// </summary>
        /// <param name="imageName">The name of the image file</param>
        /// <returns>A <see cref="Guid"/> with the image file extension added on at the end</returns>
        public string GenerateImagePath(string imageName)
        {
            var extension = Path.GetExtension(imageName);
            var generatedGuid = Guid.NewGuid();

            return generatedGuid.ToString() + extension;
        }

    }
}
