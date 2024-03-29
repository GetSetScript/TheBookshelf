﻿using BookShelf.Data;
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
        public bool TrySeed()
        {
            _logger.LogDebug("Attempting to seed database");

            if (_context.Books.Any())
            {
                _logger.LogDebug("The database already contains data and does not need to be seeded");
                return false;
            }

            var defaultDataFilePath = Path.Combine(_hosting.ContentRootPath, "Data", "BookData.json");
            var books = File.ReadAllText(defaultDataFilePath);

            var bookImports = JsonConvert.DeserializeObject<List<Book>>(books);

            var oldImageDirectory = Path.Combine(_hosting.WebRootPath, "images", "userResources");
            if (Directory.Exists(oldImageDirectory))
            {
                Directory.Delete(oldImageDirectory, true);
            }

            for (int i = 0; i < bookImports.Count; i++)
            {
                var imagePath = bookImports[i].ImagePath;
                var sourceImagePath = Path.Combine(_hosting.WebRootPath, "images", "appResources", imagePath);

                if (!Directory.Exists(Path.GetDirectoryName(sourceImagePath)))
                {
                    _logger.LogError("The Directory for the @{sourceImagePath} does not exist", sourceImagePath);
                    return false;
                }
                
                var generatedImagePath = _bookImageService.GenerateImagePath(imagePath);
                var targetImagePath = Path.Combine(_hosting.WebRootPath, "images", "userResources", generatedImagePath);

                Directory.CreateDirectory(Path.GetDirectoryName(targetImagePath));

                File.Copy(sourceImagePath, targetImagePath);
                _logger.LogDebug("File was copied from @{sourcePath} to @{targetPath}", sourceImagePath, targetImagePath);

                bookImports[i].ImagePath = generatedImagePath;
            }
            
            _context.Books.AddRange(bookImports);
            _context.SaveChanges();

            if (bookImports.Count() != _context.Books.Count())
            {
                _logger.LogError("Could not seed the database with all of the Seed Data");
                return false;
            }

            _logger.LogDebug("Seeded the database with default book data from @{filePath}", defaultDataFilePath);
            return true;
        }
    }
}
