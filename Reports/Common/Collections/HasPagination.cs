using System.ComponentModel.DataAnnotations;

namespace Reports.Common.Abstractions.Collections
{
    public abstract class HasPagination
    {
        [Range(1, (int.MaxValue / 100), ErrorMessage = "PageNumber must be at least 1.")]
        public int PageNumber { get; init; } = 1;

        [Range(1, 100, ErrorMessage = "PageSize must be between 1 and 100.")]
        public int PageSize { get; init; } = 20;
    }
}
