using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookShelf.Core.DataAnnotations
{
    /// <summary>
    /// Represents an attribute for validating file extensions
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FileExtensionValidationAttribute : ValidationAttribute
    {
        private readonly List<string> _allowedExtensions;

        /// <summary>
        /// Creates a new instance of the <see cref="FileExtensionValidationAttribute"/>
        /// </summary>
        /// <param name="fileExtensions">The allowed file extensions</param>
        public FileExtensionValidationAttribute(string fileExtensions)
        {
            _allowedExtensions = fileExtensions.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
        }

        /// <summary>
        /// Checks the passed in object for valid file extensions
        /// </summary>
        /// <param name="value">The object to be validated</param>
        /// <returns>Returns true if the object has the correct file extension</returns>
        public override bool IsValid(object value)
        {
            if (value is IFormFile file)
            {
                var fileName = file.FileName;

                return _allowedExtensions.Any(e => fileName.EndsWith(e));
            }

            return true;
        }
    }
}
