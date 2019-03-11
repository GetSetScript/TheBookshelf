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
            var pageSize = 8;
            
            var books = PaginatedList<Book>.ReturnPaginated(_bookRepository.GetAll(), Id ?? 1, pageSize);

            _logger.LogDebug("Returning Index View with @{books} resources, with page size of @{pageSize} and requested page index of @{Id}", books, pageSize, Id);
            
            return View(books);
        }

        //[HttpGet("detail")]
        //public IActionResult Detail()
        //{
        //    return View();
        //}

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
