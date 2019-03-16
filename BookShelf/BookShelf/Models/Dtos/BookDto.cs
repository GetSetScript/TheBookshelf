using BookShelf.Core.DataAnnotations;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace BookShelf.Models.Dtos
{
    /// <summary>
    /// Represents a class for transfering data to a <see cref="Book"/> when adding a resource
    /// </summary>
    public class BookDto
    {
        /// <summary>
        /// The title of the book
        /// </summary>
        [Required(ErrorMessage = "The title of the book is required.")]
        [MaxLength(55, ErrorMessage = "The title of the book cannot be longer than 55 characters.")]
        public string Title { get; set; }

        /// <summary>
        /// The author of the book
        /// </summary>
        [Required(ErrorMessage = "The author of the book is required.")]
        [MaxLength(55, ErrorMessage = "The name of the author cannot be longer than 55 characters.")]
        public string Author { get; set; }

        /// <summary>
        /// The description of the book
        /// </summary>
        [Required(ErrorMessage = "The description of the book is required.")]
        [MinLength(10, ErrorMessage = "The description of the book should be at least 10 characters long.")]
        [MaxLength(7000, ErrorMessage = "The description of the book cannot be more than 7000 characters long.")]
        public string Description { get; set; }

        /// <summary>
        /// The date the book was read
        /// </summary>
        [DataType(DataType.Date, ErrorMessage = "The \"Date Read\" value must be a valid date.")]
        public DateTime? DateRead { get; set; }

        /// <summary>
        /// The 5 star rating for the book
        /// </summary>
        [Range(1, 5, ErrorMessage = "The rating for the book must be between 1 and 5.")]
        public int? Rating { get; set; }

        /// <summary>
        /// The image for the book
        /// </summary>
        [DataType(DataType.Upload, ErrorMessage = "The upload must be a file.")]
        [MaxFileSizeValidation(2000000, ErrorMessage = "The image must be smaller than 2mb")]
        [FileExtensionValidation(".jpg,.jpeg,.png,.gif", ErrorMessage = "The file type must be one of the following - .jpg, .jpeg, .png, .gif")]
        public IFormFile Image { get; set; }
    }
}

