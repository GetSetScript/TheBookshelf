using BookShelf.Models;
using Microsoft.AspNetCore.Http;
using System;

namespace BookShelf.Services
{
    /// <summary>
    /// Represents a service for saving and deleting <see cref="Book"/> Images
    /// </summary>
    public interface IBookImageService
    {
        /// <summary>
        /// Deletes the image file for a <see cref="Book"/> resource
        /// </summary>
        /// <param name="imageName">The name of the image file that will be deleted</param>
        /// <returns>True if the image file was successfully deleted</returns>
        bool TryDeleteImage(string imageName);

        /// <summary>
        /// Resizes an <see cref="IFormFile"/> image and saves it to a file
        /// </summary>
        /// <param name="image">The image to save</param>
        /// <param name="width">The width to resize the image to, the aspect ratio is preserved</param>
        /// <param name="book">The book the image belongs with</param>
        /// <returns>True if the image was successfully saved and resized</returns>
        bool TrySaveAndResizeImage(IFormFile image, int width, Book book);

        /// <summary>
        /// Generates a unique string for use as an Image path
        /// </summary>
        /// <param name="imageName">The name of the image file</param>
        /// <returns>A <see cref="Guid"/> with the image file extension added on at the end</returns>
        string GenerateImagePath(string imageName);
    }
}