using ImageMagick;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BookShelf.Services
{
    /// <summary>
    /// Represents a service for adjusting images and saving them 
    /// </summary>
    public class ImageAdjuster : IImageAdjuster
    {
        /// <summary>
        /// Resizes an image and saves it to a file path
        /// </summary>
        /// <param name="image">The image to save</param>
        /// <param name="width">The width to resize the image to, the aspect ratio is preserved</param>
        /// <param name="filePath">The fully qualified path to save the image to</param>
        public void ResizeAndSave(IFormFile image, int width, string filePath)
        {
            if (image.Length > 0)
            {
                using (MagickImage images = new MagickImage(image.OpenReadStream()))
                {
                    MagickGeometry size = new MagickGeometry(width, 0);

                    images.Resize(size);

                    images.Write(filePath);
                }
            }
        }

    }
}
