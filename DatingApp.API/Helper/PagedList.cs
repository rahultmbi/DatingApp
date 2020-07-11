using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DatingApp.API.Helper
{
    public class PagedList<T>: List<T>
    {
        public int CurrentPage { get; set; }
        public int TotalPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

        public PagedList(List<T> items, int count, int pageNumber, int pageSize) 
        {
            TotalCount = count;
            PageSize = pageSize;
            TotalPage = (int)Math.Ceiling(count/ (double)pageSize);
            CurrentPage = pageNumber;
            this.AddRange(items);
        }

        public async static Task<PagedList<T>> CreateAsync(IQueryable<T> source,
        int pageNumber, int pageSize) {
            var count = await source.CountAsync();
            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }
    }
}