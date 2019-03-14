using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using BookShelf.Core;
using BookShelf.Models;
using BookShelf.Models.Dtos;
using BookShelf.Services;
using ImageMagick;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace BookShelf.Controllers
{
    /// <summary>
    /// Represents the controller that serves the home page and <see cref="Book"/> Views
    /// </summary>
    public class BooksController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IBookRepositoryService _bookRepository;
        private readonly ILogger<BooksController> _logger;
        private readonly IImageAdjuster _imageAdjuster;
        private readonly IHostingEnvironment _hosting;

        private const int PageSize = 9;

        /// <summary>
        /// Creates a new instance of <see cref="BooksController"/>
        /// </summary>
        /// <param name="mapper">Used for mapping objects</param>
        /// <param name="bookRepository">Used for manipulating and accessing book resources</param>
        /// <param name="logger">Used for logging</param>
        /// <param name="imageAdjuster">Used for adjusting and saving <see cref="IFormFile"/></param>
        /// <param name="hosting">Used to get information about the hosting environment</param>
        public BooksController(IMapper mapper, IBookRepositoryService bookRepository, 
                               ILogger<BooksController> logger, IImageAdjuster imageAdjuster,
                               IHostingEnvironment hosting)
        {
            _mapper = mapper;
            _bookRepository = bookRepository;
            _logger = logger;
            _imageAdjuster = imageAdjuster;
            _hosting = hosting;
        }

        /// <summary>
        /// Serves the <see cref="Book"/> Index View
        /// </summary>
        /// <returns>The View that matches this Action</returns>
        [HttpGet]
        public IActionResult Index(int? Id)
        {
            var books = PaginatedList<Book>.ReturnPaginated(_bookRepository.GetAll(), Id ?? 1, PageSize);

            _logger.LogDebug("Returning Index View with @{books} resources, with page size of @{pageSize} and requested page index of @{Id}", books, PageSize, Id);
            
            return View(books);
        }

        /// <summary>
        /// Serves the <see cref="Book"/> Detail View
        /// </summary>
        /// <param name="id">The id of the <see cref="Book"/> resource</param>
        /// <returns>The View that matches this Action</returns>
        /// <returns>The NotFound View if the <see cref="Book"/> resource is not found</returns>
        [HttpGet("detail")]
        public async Task<IActionResult> Detail(int id)
        {
            _logger.LogDebug("Attempting to serve Detail View with book Id of @{Id}", id);
            var book = await _bookRepository.GetByIdAsync(id);

            if (book == null)
            {
                _logger.LogDebug("The book with Id of @{Id} was not found", id);
                return View(nameof(NotFound));
            }

            _logger.LogDebug("Returning Detail View with @{book} resource", book);

            return View(book);
        }

        /// <summary>
        /// Serves the Delete <see cref="Book"/> View
        /// </summary>
        /// <param name="id">The Id of the <see cref="Book"/> to be deleted</param>
        /// <returns>The View that matches this Action</returns>
        /// <returns>The NotFound View if the <see cref="Book"/> resource is not found</returns>
        [HttpGet("delete")]
        public async Task<IActionResult> Delete(int id)
        {
            _logger.LogDebug("Attempting to serve Delete View with book Id of @{Id}", id);

            var book = await _bookRepository.GetByIdAsync(id);

            if (book == null)
            {
                _logger.LogError("Failed to serve Delete View because @{book} was null", book);
                return View(nameof(NotFound));
            }

            _logger.LogDebug("Returning Delete View with @{book} resource", book);

            return View(book);
        }

        /// <summary>
        /// Deletes a <see cref="Book"/> resource
        /// </summary>
        /// <param name="id">The Id of the <see cref="Book"/> resource to be deleted</param>
        /// <returns>The <see cref="Index(int?)"/> View if successful</returns>
        /// <returns>The NotFound View if the <see cref="Book"/> resource is not found</returns>
        /// <returns>The <see cref="ErrorsController.Error"/> View if something went wrong</returns>
        [HttpPost("delete")]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            _logger.LogDebug("Attempting to delete book resourece with Id of @{id}", id);
            
            var book = await _bookRepository.GetByIdAsync(id);

            if (book == null)
            {
                _logger.LogDebug("The book with Id of @{Id} was not found", id);
                return View(nameof(NotFound));
            }

            try
            {
                await _bookRepository.DeleteAsync(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete @{book}", book);
                return RedirectToAction("Error", "Errors");
            }

            _logger.LogDebug("Successfully deleted @{book} resource", book);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Serves the Create <see cref="Book"/> View
        /// </summary>
        /// <returns>The View that matches this action</returns>
        [HttpGet("create")]
        public IActionResult Create()
        {
            _logger.LogDebug("Serving the Create View");
            return View();
        }

        /// <summary>
        /// Creates a new <see cref="Book"/> resource
        /// </summary>
        /// <param name="model">The model to bind to</param>
        /// <returns>The <see cref="Index(int?)"/> View if successful</returns>
        /// <returns>The <see cref="Create()"/> View if unsuccessful</returns>
        /// <returns>The <see cref="ErrorsController.Error"/> View if something went wrong</returns>
        [HttpPost("create")]
        public async Task<IActionResult> Create(BookDto model)
        {
            _logger.LogDebug("Attempting to create book resource from @{model}", model);

            if (model == null)
            {
                _logger.LogError("Could not create book resource because the @{model} was null", model);
                return RedirectToAction("Error", "Errors");
            }
            
            if (!ModelState.IsValid)
            {
                return View();
            }

            var book = new Book()
            {
                Title = model.Title,
                Author = model.Author,
                Rating = model.Rating,
                DateRead = model.DateRead.Value,
                Description = model.Description,
                ImagePath = Path.GetFileName(model.Image.FileName)
            };

            try
            {
                await _bookRepository.CreateAsync(book);

                var filePath = Path.Combine(_hosting.WebRootPath, "images", book.ImagePath);
                _imageAdjuster.ResizeAndSave(model.Image, 600, filePath);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong while trying to create @{book} resource from @{model}", book, model);
                return RedirectToAction("Error", "Errors");
            }

            _logger.LogDebug("Successfully created @{book} resource created from @{model}", book, model);
            return RedirectToAction(nameof(Index));
        }

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
