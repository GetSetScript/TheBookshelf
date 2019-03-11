using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
        [Required(ErrorMessage = "The Title of the book is required")]
        public string Title { get; set; }

        /// <summary>
        /// The author of the book
        /// </summary>
        [Required(ErrorMessage = "The Author of the book is required")]
        public string Author { get; set; }

        /// <summary>
        /// The description of the book
        /// </summary>
        [Required(ErrorMessage = "The Description of the book is required")]
        [MinLength(10, ErrorMessage = "The description of the book should be at least 10 characters long")]
        public string Description { get; set; }

        /// <summary>
        /// The date the book was read
        /// </summary>
        [DataType(DataType.Date, ErrorMessage = "The Date Read value must be a valid Date")]
        public DateTime DateRead { get; set; }

        /// <summary>
        /// The 5 star rating for the book
        /// </summary>
        [Range(1, 5, ErrorMessage = "The Rating for the book must be between 1 and 5")]
        public int? Rating { get; set; }

        /// <summary>
        /// The image for the book
        /// </summary>
        [DataType(DataType.Upload, ErrorMessage = "The Image must be a file")]
        public IFormFile Image { get; set; }
    }
}

