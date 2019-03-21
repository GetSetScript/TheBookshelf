using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BookShelf.Core.DataAnnotations
{
    /// <summary>
    /// Represents an attribute for validating file size
    /// </summary>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MaxFileSizeValidationAttribute : ValidationAttribute
    {
        private readonly int? _maxBytes;

        /// <summary>
        /// Creatse a new instance of the <see cref="MaxFileSizeValidationAttribute"/>
        /// </summary>
        /// <param name="maxBytes">The maximum amount of bytes a file is allowed to be</param>
        public MaxFileSizeValidationAttribute(int maxBytes)
        {
            _maxBytes = maxBytes;
        }

        /// <summary>
        /// Checks the passed in object for a valid file size
        /// </summary>
        /// <param name="value">The object to be validated</param>
        /// <returns>Returns true if the file is under the maximum amount of bytes</returns>
        public override bool IsValid(object value)
        {
            if (value is IFormFile file)
            {
                if (_maxBytes.HasValue)
                {
                    return file.Length < _maxBytes.Value;
                }
            }

            return true;
        }

    }
}
