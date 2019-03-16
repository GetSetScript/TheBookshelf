using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShelf.Models.Dtos
{
    /// <summary>
    /// Represents a generic class to be used for displaying models to the <see cref="ViewResult"/>
    /// </summary>
    /// <typeparam name="TContent">The model resource to be displayed on the <see cref="ViewResult"/></typeparam>
    public class BookViewModel<TContent> where TContent : class
    {
        /// <summary>
        /// The model resource to be displayed on the <see cref="ViewResult"/>
        /// </summary>
        public TContent Content { get; set; }

        /// <summary>
        /// An Object for retrieving the application configuration values
        /// </summary>
        public string NoImagePath { get; set; }
    }
}
