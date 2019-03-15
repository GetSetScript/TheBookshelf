using BookShelf.Data;
using BookShelf.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BookShelf.Services
{
    /// <summary>
    /// Represents a Class that adds default book data to the <see cref="BooksDbContext"/>
    /// </summary>
    public class BookDataSeeder
    {
        private readonly IHostingEnvironment _hosting;
        private readonly BooksDbContext _context;
        private readonly ILogger<BookDataSeeder> _logger;
        private readonly IBookImageService _bookImageService;
        private readonly List<Book> _books = new List<Book>();

        /// <summary>
        /// Creats a new instance of <see cref="BookDataSeeder"/>
        /// </summary>
        /// <param name="hosting">Used for getting the Json data from the content root of the application</param>
        /// <param name="context">Used to add the seed data to the database</param>
        /// <param name="logger">Used for logging</param>
        /// <param name="bookImageService">Used for seeding <see cref="Book"/> Images</param>
        public BookDataSeeder(IHostingEnvironment hosting, BooksDbContext context, 
                              ILogger<BookDataSeeder> logger, IBookImageService bookImageService)
        {
            _hosting = hosting;
            _context = context;
            _logger = logger;
            _bookImageService = bookImageService;
        }

        /// <summary>
        /// Seeds the database with default book data
        /// </summary>
        public void Seed()
        {
            _logger.LogDebug("Attempting to seed database");

            if (_context.Books.Any())
            {
                _logger.LogDebug("The database already contains data and does not need to be seeded");
                return;
            }

            var filePath = Path.Combine(_hosting.ContentRootPath, "Data", "BookData.json");
            var books = File.ReadAllText(filePath);

            var bookImports = JsonConvert.DeserializeObject<List<Book>>(books);
            
            for (int i = 0; i < bookImports.Count; i++)
            {
                var imageFileName = bookImports[i].ImagePath;
                var sourcePath = Path.Combine(_hosting.WebRootPath, "Images", "AppResources", imageFileName);

                var imageFileNameExtension = Path.GetExtension(imageFileName);
                var targetFileName = _bookImageService.GenerateImagePath(imageFileNameExtension);
                var targetPath = Path.Combine(_hosting.WebRootPath, "Images", targetFileName);

                File.Copy(sourcePath, targetPath); // this copies file to new location and renames?

                bookImports[i].ImagePath = targetFileName;
            }
            
            _context.Books.AddRange(bookImports);
            _context.SaveChanges();

            if (bookImports.Count() != _context.Books.Count())
            {
                throw new InvalidOperationException("Could not seed the database with all of the Seed Data");
            }

            _logger.LogDebug("Seeded the database with default book data from @{filePath}", filePath);
        }
    }
}
