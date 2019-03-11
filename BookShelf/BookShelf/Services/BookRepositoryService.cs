using BookShelf.Data;
using BookShelf.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShelf.Services
{
    /// <summary>
    /// Represents a service for accessing and manipulating the book resources
    /// </summary>
    public class BookRepositoryService : IBookRepositoryService
    {
        private readonly BooksDbContext _context;
        private readonly ILogger<BookRepositoryService> _logger;

        /// <summary>
        /// Creates a new instance of <see cref="BookRepositoryService"/>
        /// </summary>
        /// <param name="context">The DbContext for interfacing with the database</param>
        /// <param name="logger">Used for logging</param>
        public BookRepositoryService(BooksDbContext context, ILogger<BookRepositoryService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Gets all of the <see cref="Book"/> resources from the database
        /// </summary>
        /// <returns>Returns an IQueryable collection</returns>
        public IEnumerable<Book> GetAll()
        {
            _logger.LogDebug("Attempting to retreive all of the @{book} resources from @{_context}", nameof(Book), _context);
            return _context.Books.ToList();
        }

        /// <summary>
        /// Get a specific <see cref="Book"/> based on the passed in ID
        /// </summary>
        /// <param name="id">The Id of the book to be returned</param>
        /// <returns>A book resource</returns>
        public async Task<Book> GetByIdAsync(int id)
        {
            _logger.LogDebug("Attempting to retreive a @{book} resource with id of @{id} from @{_context}", nameof(Book), id, _context);
            return await _context.Books.FindAsync(id);
        }

        /// <summary>
        /// Creates a book resource
        /// </summary>
        /// <param name="book">The book resource to be created</param>
        /// <returns>A Task to be awaited</returns>
        public async Task CreateAsync(Book book)
        {
            _logger.LogDebug("Attempting to create a @{book} resource", book);

            if (book == null)
            {
                _logger.LogError("Failed to create @{book} resource because it was null", book);
                throw new ArgumentNullException(nameof(book));
            }

            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();

            _logger.LogDebug("Created the @{book} resource", book);
        }
        
        /// <summary>
        /// Deletes a book resource
        /// </summary>
        /// <param name="book">The book resource to be deleted</param>
        /// <returns>A Task to be awaited</returns>
        public async Task DeleteAsync(Book book)
        {
            _logger.LogDebug("Attempting to delete a @{book} resource", book);

            if (book == null)
            {
                _logger.LogError("Failed to delete @{book} resource because it was null", book);
                throw new ArgumentNullException(nameof(book));
            }

            if (await DoesBookExistAsync(book))
            {
                _context.Books.Remove(book);
                await _context.SaveChangesAsync();
                _logger.LogDebug("Deleted the @{book} resource", book);
            }
        }

        /// <summary>
        /// Updates a book resourece
        /// </summary>
        /// <param name="book">The book to be updated</param>
        /// <returns>A Task to await</returns>
        public async Task UpdateAsync(Book book)
        {
            _logger.LogDebug("Attempting to update a @{book} resource", book);

            if (book == null)
            {
                _logger.LogError("Failed to update @{book} resource because it was null", book);
                throw new ArgumentNullException(nameof(book));
            }

            if (await DoesBookExistAsync(book))
            {
                _context.Books.Update(book);
                await _context.SaveChangesAsync();
                _logger.LogDebug("Updated the @{book} resource", book);
            }
        }

        /// <summary>
        /// Check for the existance of a book resource
        /// </summary>
        /// <param name="book">The book to check</param>
        /// <returns>Returns true if the book exists</returns>
        public async Task<bool> DoesBookExistAsync(Book book)
        {
            _logger.LogDebug("Attempting to check if a @{book} resource exists", book);

            if (book == null)
            {
                _logger.LogError("Failed to check the existance of the @{book} resource because it was null", book);
                throw new ArgumentNullException(nameof(book));
            }

            _logger.LogDebug("The @{book} resource exists", book);
            return await _context.Books.AnyAsync(b => b.Id == book.Id);
        }
        
    }
}
