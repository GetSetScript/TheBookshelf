using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookShelf.Core
{
    /// <summary>
    /// Represents a paginated collection
    /// </summary>
    /// <typeparam name="T">The type for the list</typeparam>
    public class PaginatedList<T> : List<T>
    {
        /// <summary>
        /// The current page index (page number)
        /// </summary>
        public int PageIndex { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public int NextPageIndex => PageIndex + 1;

        /// <summary>
        /// 
        /// </summary>
        public int PreviousPageIndex => PageIndex - 1;

        /// <summary>
        /// the total amount of pages available in the given collection
        /// </summary>
        public int TotalPages { get; private set; }

        /// <summary>
        /// Returns true if the previous page index (page number) is available
        /// </summary>
        public bool HasPreviousPage => (PageIndex > 1);

        /// <summary>
        /// Returns true if next page index (page number) is available
        /// </summary>
        public bool HasNextPage => (PageIndex < TotalPages);


        /// <summary>
        /// Creates a new instance of the <see cref="PaginatedList{T}"/>
        /// </summary>
        /// <param name="items">The resources in the collection that will be paginated over</param>
        /// <param name="count">The total number of resources in the collection</param>
        /// <param name="pageIndex">The page index (page number)</param>
        /// <param name="pageSize">The amount of resources to return from the collection</param>
        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            this.AddRange(items);

            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }

        /// <summary>
        /// Returns a collection that has been paginated over
        /// </summary>
        /// <param name="source">The collection to be paginated over</param>
        /// <param name="pageIndex">The desired page index (page number)</param>
        /// <param name="pageSize">The desired amount of resources to be returned</param>
        /// <returns>Returns the desired amount of resources from the desired page index (page number) as a collection</returns>
        public static PaginatedList<T> ReturnPaginated(IEnumerable<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
        
    }
}
