using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BookShelf.Core;
using BookShelf.Models;
using BookShelf.Models.Dtos;
using BookShelf.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BookShelf.Controllers
{
    /// <summary>
    /// Represents the main controller for serving up the home page
    /// </summary>
    public class BooksController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IBookRepositoryService _bookRepository;
        private readonly ILogger<BooksController> _logger;

        /// <summary>
        /// Creates a new instance of <see cref="BooksController"/>
        /// </summary>
        /// <param name="mapper">Used for mapping objects</param>
        /// <param name="bookRepository">Used for manipulating and accessing book resources</param>
        /// <param name="logger">Used for logging</param>
        public BooksController(IMapper mapper, IBookRepositoryService bookRepository, ILogger<BooksController> logger)
        {
            _mapper = mapper;
            _bookRepository = bookRepository;
            _logger = logger;
        }

        /// <summary>
        /// Serves the homepage of the website
        /// </summary>
        /// <returns>The Index page view</returns>
        [HttpGet()]
        public IActionResult Index(int? Id)
        {
            var pageSize = 9;
            
            var books = PaginatedList<Book>.ReturnPaginated(_bookRepository.GetAll(), Id ?? 1, pageSize);

            _logger.LogDebug("Returning Index View with @{books} resources, with page size of @{pageSize} and requested page index of @{Id}", books, pageSize, Id);
            
            return View(books);
        }

        /// <summary>
        /// Serves a detail view of a <see cref="Book"/> resource
        /// </summary>
        /// <param name="Id">The id of the <see cref="Book"/> resource</param>
        /// <returns>A Task to be awaited</returns>
        [HttpGet("detail")]
        public async Task<IActionResult> Detail(int Id)
        {
            _logger.LogDebug("Attempting to retrieve book with Id of @{Id}", Id);
            var book = await _bookRepository.GetByIdAsync(Id);

            if (book == null)
            {
                _logger.LogDebug("The book with Id of @{Id} was not found", Id);
                return View(nameof(NotFound));
            }

            _logger.LogDebug("Returning Detail View with @{book} resource", book);

            return View(book);
        }

        /// <summary>
        /// Serves a Delete View for a <see cref="Book"/> resource
        /// </summary>
        /// <param name="id">The Id of the book to be deleted</param>
        /// <param name="saveChangesError">If true will display an error message to the View</param>
        /// <returns>A Task to be awaited</returns>
        [HttpGet("delete")]
        public async Task<IActionResult> Delete(int id, bool? saveChangesError = false)
        {
            var book = await _bookRepository.GetByIdAsync(id);

            if (book == null)
            {
                _logger.LogError("Failed to serve Delete View because @{book} was null", book);
                return View(nameof(NotFound));
            }

            if (saveChangesError.GetValueOrDefault())
            {
                ViewData["ErrorMessage"] =
                    "Delete failed. Try again, and if the problem persists " +
                    "contact the web administrator.";
            }

            _logger.LogDebug("Returning Delete View with @{book} resource", book);

            return View(book);
        }

        /// <summary>
        /// Returns the Index View if successful, otherwise returns to the Delete View with an error
        /// </summary>
        /// <param name="id">The Id of the <see cref="Book"/> resource to be deleted</param>
        /// <returns>A Task to be awaited</returns>
        [HttpPost("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogDebug("Attempting to delete book resourece with Id of @{id}", id);
            
            var book = await _bookRepository.GetByIdAsync(id);

            if (book == null)
            {
                _logger.LogDebug("The book with Id of @{Id} was not found", id);
                return RedirectToAction(nameof(Delete), new { id = id, saveChangesError = true });
            }

            try
            {
                await _bookRepository.DeleteAsync(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete @{book}", book);
                return RedirectToAction(nameof(Delete), new { id = book.Id, saveChangesError = true });
            }

            _logger.LogDebug("Successfully deleted @{book} resource", book);
            return RedirectToAction(nameof(Index));
        }
        
        //[HttpGet("create")]
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public IActionResult Create(BookDto model)
        //{
        //    if (model == null)
        //    {
        //        //throw error
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        //throw error
        //    }

        //    return View();
        //}

        //[HttpGet("update")]
        //public IActionResult Update()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public IActionResult Update(BookDto model)
        //{
        //    if (model == null)
        //    {
        //        //throw error
        //    }

        //    if (!ModelState.IsValid)
        //    {
        //        //throw error
        //    }

        //    return View();
        //}
    }
}
