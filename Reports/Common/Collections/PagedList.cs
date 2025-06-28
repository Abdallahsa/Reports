using Microsoft.EntityFrameworkCore;

namespace Reports.Api.Common.Abstractions.Collections
{
    public class PagedList<T>
    {
        public int PageNumber { get; init; }
        public int PageSize { get; init; }
        public int TotalCount { get; init; }
        public int TotalPages { get; init; }
        public bool HasPrevious => PageNumber > 1;
        public bool HasNext => PageNumber < TotalPages;
        public List<T> Items { get; init; } = new();

        private PagedList(IEnumerable<T> items, int count, int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
            TotalCount = count;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            Items.AddRange(items);
        }

        public static PagedList<T> Create(IEnumerable<T> source, int pageNumber, int pageSize)
        {
            var count = source.Count();
            var totalPages = (int)Math.Ceiling(count / (double)pageSize);

            // Ensure that the pageNumber does not exceed the totalPages
            if (pageNumber > totalPages)
            {
                pageNumber = totalPages;
            }

            // Handle the case where totalPages is 0 to avoid negative skip
            pageNumber = Math.Max(pageNumber, 1);

            var items = source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }

        public static async Task<PagedList<T>> CreateAsync(IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var count = await source.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling(count / (double)pageSize);

            // Ensure that the pageNumber does not exceed the totalPages
            if (pageNumber > totalPages)
            {
                pageNumber = totalPages;
            }

            // Handle the case where totalPages is 0 to avoid negative skip
            pageNumber = Math.Max(pageNumber, 1);

            var items = await source.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
            return new PagedList<T>(items, count, pageNumber, pageSize);
        }

    }
}
