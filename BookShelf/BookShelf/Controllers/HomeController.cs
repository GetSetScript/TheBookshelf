using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace BookShelf.Controllers
{
    /// <summary>
    /// Represents the main controller for serving up the home page
    /// </summary>
    public class HomeController : Controller
    {
        private readonly IMapper _mapper;

        /// <summary>
        /// Creates a new instance of <see cref="HomeController"/>
        /// </summary>
        /// <param name="mapper">Used for mapping objects</param>
        public HomeController(IMapper mapper)
        {
            _mapper = mapper;
        }

        /// <summary>
        /// Serves the homepage of the website
        /// </summary>
        /// <returns>The Index page view</returns>
        public IActionResult Index()
        {
            return View();
        }
    }
}
