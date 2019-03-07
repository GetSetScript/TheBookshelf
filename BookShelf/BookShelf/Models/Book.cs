using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookShelf.Models
{
    /// <summary>
    /// Represent a book
    /// </summary>
    public class Book
    {
        /// <summary>
        /// The database Id for the book
        /// </summary>
        [Key]
        public int Id { get; set; }

        /// <summary>
        /// The title of the book
        /// </summary>
        [Required]
        public string Title { get; set; }

        /// <summary>
        /// The author of the book
        /// </summary>
        [Required]
        public string Author { get; set; }

        /// <summary>
        /// The description of the book
        /// </summary>
        [Required]
        [MinLength(10)]
        public string Description { get; set; }

        /// <summary>
        /// The date the book was read
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime DateRead { get; set; }

        /// <summary>
        /// The 5 star rating for the book
        /// </summary>
        [Range(1, 5)]
        public int? Rating { get; set; }

        /// <summary>
        /// The image URL for the book
        /// </summary>
        [DataType(DataType.Url)]
        public string ImageUrl { get; set; }
    }
}
