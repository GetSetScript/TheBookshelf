using System.Linq;
using System.Threading.Tasks;
using BookShelf.Models;

namespace BookShelf.Services
{
    /// <summary>
    /// Represents the interface for a service that can access and manipulate book resources
    /// </summary>
    public interface IBookRepositoryService
    {
        /// <summary>
        /// Creates a book resource
        /// </summary>
        /// <param name="book">The book resource to be created</param>
        /// <returns>A Task to be awaited</returns>
        Task CreateAsync(Book book);

        /// <summary>
        /// Deletes a book resource
        /// </summary>
        /// <param name="book">The book resource to be deleted</param>
        /// <returns>A Task to be awaited</returns>
        Task DeleteAsync(Book book);

        /// <summary>
        /// Check for the existance of a book resource
        /// </summary>
        /// <param name="book">The book to check</param>
        /// <returns>Returns true if the book exists</returns>
        Task<bool> DoesBookExistAsync(Book book);

        /// <summary>
        /// Gets all of the <see cref="Book"/> resources from the database
        /// </summary>
        /// <returns>Returns an IQueryable collection</returns>
        IQueryable<Book> GetAll();

        /// <summary>
        /// Get a specific <see cref="Book"/> based on the passed in ID
        /// </summary>
        /// <param name="id">The Id of the book to be returned</param>
        /// <returns>A book resource</returns>
        Task<Book> GetByIdAsync(int id);

        /// <summary>
        /// Updates a book resourece
        /// </summary>
        /// <param name="book">The book to be updated</param>
        /// <returns>A Task to await</returns>
        Task UpdateAsync(Book book);
    }
}