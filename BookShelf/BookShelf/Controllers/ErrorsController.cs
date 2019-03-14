using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace BookShelf.Controllers
{
    /// <summary>
    /// Represents a controller for serving error Views
    /// </summary>
    public class ErrorsController : Controller
    {
        /// <summary>
        /// Serves An Error View for the application
        /// </summary>
        /// <returns>The View that matches this Action</returns>
        public IActionResult Error()
        {
            return View();
        }
    }
}