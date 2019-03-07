using BookShelf.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShelf.Data
{
    /// <summary>
    /// Represents the Database Interface for manipulating book resources
    /// </summary>
    public class BooksDbContext : DbContext
    {
        /// <summary>
        /// An entity set that can be used to manipulate books in the database
        /// </summary>
        public DbSet<Book> Books { get; set; }

        /// <summary>
        /// Creates a new instance of <see cref="BooksDbContext"/>
        /// </summary>
        /// <param name="options">The Options for the DbContext using this <see cref="BooksDbContext"/> as the Context</param>
        public BooksDbContext(DbContextOptions<BooksDbContext> options) : base(options)
        {
            Database.Migrate();
        }
    }
}
