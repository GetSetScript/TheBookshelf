using Microsoft.AspNetCore.Http;

namespace BookShelf.Services
{
    /// <summary>
    /// Represents an interface for adjusting images and saving them 
    /// </summary>
    public interface IImageAdjuster
    {
        /// <summary>
        /// Resizes an image and saves it to a file path
        /// </summary>
        /// <param name="image">The image to save</param>
        /// <param name="width">The width to resize the image to (the aspect ratio is preserved)</param>
        /// <param name="filePath">The fully qualified path to save the image to</param>
        void ResizeAndSave(IFormFile image, int width, string filePath);
    }
}