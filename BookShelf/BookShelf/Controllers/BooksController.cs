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
            _logger.LogDebug("Attempting to serve Delete View with book Id of @{id}", id);

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
                if (_imageService.TryDeleteImage(book.ImagePath))
                {
                    _logger.LogDebug("Successfully deleted the @{book.ImagePath} image for @{book}", book.ImagePath, book);
                }
                else
                {
                    _logger.LogError("Failed to delete the @{book.ImagePath} image for the @{book} resource", book.ImagePath, book);
                }
            }

            _logger.LogDebug("Successfully deleted @{book} resource", book);
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Serves the Create <see cref="Book"/> View
        /// </summary>
        /// <returns>A View for creating <see cref="Book"/> resources</returns>
        [HttpGet("create")]
        public IActionResult Create()
        {
            _logger.LogDebug("Serving the Create View");
            return View();
        }

        /// <summary>
        /// Creates a new <see cref="Book"/> resource
        /// </summary>
        /// <param name="model">The Dto containing the information for the new <see cref="Book"/> resource</param>
        /// <returns>The <see cref="Index(int?)"/> View if successful</returns>
        /// <returns>The <see cref="Create()"/> View if unsuccessful</returns>
        /// <returns>The <see cref="ErrorsController.Error"/> View if something went wrong</returns>
        [HttpPost("create")]
        public async Task<IActionResult> Create(AddBookDto model)
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
                DateRead = model.DateRead,
                Description = model.Description,
                ImagePath = null,
                DateCreated = DateTime.Now
            };
            
            if (model.Image != null)
            {
                book.ImagePath = _imageService.GenerateImagePath(Path.GetFileName(model.Image.FileName));
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
                        _logger.LogError("Failed to create @{book} because the @{model.Image} image was not created", book, model.Image);
                        ViewBag.CreateImageError = "We were not able to create the book image due to an error, please try again or submit your book entry without the image";
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

        /// <summary>
        /// Serves the Edit Book View
        /// </summary>
        /// <param name="id">The Id of the <see cref="Book"/> to be deleted</param>
        /// <returns>The View that displays a <see cref="EditBookDto"/></returns>
        /// <returns>The NotFound View if the <see cref="Book"/> resource is not found</returns>
        [HttpGet("edit")]
        public async Task<IActionResult> Edit(int id)
        {
            _logger.LogDebug("Attempting to serve Edit View with book Id of @{id}", id);

            var book = await _bookRepository.GetByIdAsync(id);

            if (book == null)
            {
                _logger.LogError("Failed to serve Edit View because @{book} was null", book);
                return View(nameof(NotFound));
            }

            var editBookDto = new EditBookDto()
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                Rating = book.Rating,
                DateRead = book.DateRead,
                Description = book.Description,
                Image = null,
                ShouldChangeImage = false
            };

            _logger.LogDebug("Returning Edit View with @{editBookDto} resource", editBookDto);

            return View(editBookDto);
        }

        /// <summary>
        /// Creates a change to a <see cref="Book"/> resource
        /// </summary>
        /// <param name="model">The Dto containing the changes for the <see cref="Book"/> resource</param>
        /// <returns>The <see cref="Index(int?)"/> View if successful</returns>
        /// <returns>The <see cref="Edit(int)"/> View if unsuccessful</returns>
        /// <returns>The <see cref="ErrorsController.Error"/> View if something went wrong</returns>
        /// <returns>The NotFound View if the <see cref="Book"/> resource is not found</returns>
        [HttpPost("edit")]
        public async Task<IActionResult> Edit(EditBookDto model)
        {
            _logger.LogDebug("Attempting to edit book resource from @{model}", model);

            if (model == null)
            {
                _logger.LogError("Could not edit book resource because the @{model} was null", model);
                return RedirectToAction("Error", "Errors");
            }

            if (!ModelState.IsValid)
            {
                return View();
            }

            var book = await _bookRepository.GetByIdAsync(model.Id);

            if (book == null)
            {
                _logger.LogError("Failed to serve Edit View because @{book} was null", book);
                return View(nameof(NotFound));
            }

            book.Title = model.Title;
            book.Author = model.Author;
            book.Rating = model.Rating;
            book.Description = model.Description;
            book.DateRead = model.DateRead;

            if (model.ShouldChangeImage)
            {
                _logger.LogDebug("Attempting to change image for @{book}", book);
                var oldImagePath = book.ImagePath;

                if (model.Image != null)
                {
                    book.ImagePath = _imageService.GenerateImagePath(Path.GetFileName(model.Image.FileName));

                    if (_imageService.TrySaveAndResizeImage(model.Image, BookImageSize, book))
                    {
                        _logger.LogDebug("The @{model.Image} for @{book} was successfully saved and resized", model.Image, book);
                    }
                    else
                    {
                        _logger.LogError("Failed to create @{book} because the @{model.Image} image was not created", book, model.Image);
                        ViewBag.EditImageError = "We were not able to create the book image due to an error, please try again or submit your book entry without the image";
                        return View();
                    }
                }
                else
                {
                    book.ImagePath = null;
                    _logger.LogDebug("The image for @{book} was successfully changed to null", book);
                }

                if (oldImagePath != null)
                {
                    if (_imageService.TryDeleteImage(oldImagePath))
                    {
                        _logger.LogDebug("Successfully deleted the @{oldImagePath} image for @{book}", oldImagePath, book);
                    }
                    else
                    {
                        _logger.LogError("Failed to delete the @{oldImagePath} image for the @{book} resource", oldImagePath, book);
                    }
                }
            }

            try
            {
                await _bookRepository.UpdateAsync(book);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Something went wrong while trying to edit @{book} resource from @{model}", book, model);
                return RedirectToAction("Error", "Errors");
            }

            _logger.LogDebug("Successfully edited @{book} resource created from @{model}", book, model);
            return RedirectToAction(nameof(Index));
        }
    }
}
