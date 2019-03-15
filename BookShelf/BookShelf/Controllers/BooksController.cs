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
using Microsoft.Extensions.Configuration;
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
        private readonly IBookImageService _imageService;
        private readonly IConfiguration _config;
        
        private readonly int PageSize;
        private readonly int BookImageSize;

        /// <summary>
        /// Creates a new instance of <see cref="BooksController"/>
        /// </summary>
        /// <param name="mapper">Used for mapping objects</param>
        /// <param name="bookRepository">Used for manipulating and accessing <see cref="Book"/> resources</param>
        /// <param name="logger">Used for logging</param>
        /// <param name="imageService">Used for saving and deleting <see cref="Book"/> Images</param>
        /// <param name="config">Used to access application configuration</param>
        public BooksController(IMapper mapper, IBookRepositoryService bookRepository, 
                               ILogger<BooksController> logger, IBookImageService imageService,
                               IConfiguration config)
        {
            _mapper = mapper;
            _bookRepository = bookRepository;
            _logger = logger;
            _imageService = imageService;
            _config = config;

            int.TryParse(_config["Books:PageSize"], out PageSize);
            int.TryParse(_config["Books:ImageSize"], out BookImageSize);
        }

        /// <summary>
        /// Serves the <see cref="BookViewModel{TContent}"/> Index View
        /// </summary>
        /// <returns>The View that displays a paginated list of <see cref="BookViewModel{TContent}"/></returns>
        [HttpGet]
        public IActionResult Index(int? Id)
        {
            var sortedBooks = _bookRepository.GetAll().OrderByDescending(b => b.DateCreated);

            var books = PaginatedList<Book>.ReturnPaginated(sortedBooks, Id ?? 1, PageSize);
            
            var bookView = new BookViewModel<PaginatedList<Book>>()
            {
                Content = books,
                NoImagePath = _config["Books:NoImagePath"]
            };

            _logger.LogDebug("Returning Index View with @{bookView} resources, with page size of @{pageSize} and requested page index of @{Id}", bookView, PageSize, Id);
            
            return View(bookView);
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

            var bookView = new BookViewModel<Book>()
            {
                Content = book,
                NoImagePath = _config["Books:NoImagePath"]
            };

            _logger.LogDebug("Returning Detail View with @{bookView} resource", bookView);

            return View(bookView);
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

            var bookView = new BookViewModel<Book>()
            {
                Content = book,
                NoImagePath = _config["Books:NoImagePath"]
            };

            _logger.LogDebug("Returning Delete View with @{bookView} resource", bookView);

            return View(bookView);
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

            if (book.ImagePath != null)
            {
                if (_imageService.TryDeleteImage(book))
                {
                    _logger.LogDebug("Successfully deleted the @{book.ImagePath} file for @{book}", book.ImagePath, book);
                }
                else
                {
                    _logger.LogError("Failed to delete the @{book.ImagePath} file for the @{book} resource", book.ImagePath, book);
                }
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
                ImagePath = null,
                DateCreated = DateTime.Now
            };
            
            if (model.Image != null)
            {
                var extension = Path.GetExtension(Path.GetFileName(model.Image.FileName));

                book.ImagePath = _imageService.GenerateImagePath(extension);
            }

            try
            {
                if (book.ImagePath != null)
                {
                    if (_imageService.TrySaveAndResizeImage(model.Image, BookImageSize, book))
                    {
                        _logger.LogDebug("The @{model.Image} for @{book} was successfully saved and resized", model.Image, book);
                    }
                    else
                    {
                        _logger.LogError("Failed to create @{book} because the @{model.Image} was not created", book, model.Image);
                        ViewBag.CreateImageError("We were not able to create the book image due to an error, please try again or submit your book entry without the image");
                        return View();
                    }
                }

                await _bookRepository.CreateAsync(book);
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
