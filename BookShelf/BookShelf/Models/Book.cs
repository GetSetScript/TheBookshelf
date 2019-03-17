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
        /// The Date the book was created and added to the database
        /// </summary>
        [Required]
        [DataType(DataType.Date)]
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// The title of the book
        /// </summary>
        [Required]
        [MaxLength(55)]
        public string Title { get; set; }

        /// <summary>
        /// The author of the book
        /// </summary>
        [Required]
        [MaxLength(55)]
        public string Author { get; set; }

        /// <summary>
        /// The description of the book
        /// </summary>
        [Required]
        [MinLength(10)]
        [MaxLength(7000)]
        public string Description { get; set; }

        /// <summary>
        /// The date the book was read
        /// </summary>
        [DataType(DataType.Date)]
        public DateTime? DateRead { get; set; }

        /// <summary>
        /// The 5 star rating for the book
        /// </summary>
        [Range(1, 5)]
        public int? Rating { get; set; }

        /// <summary>
        /// The image Path for the book
        /// </summary>
        public string ImagePath { get; set; }
    }
}
