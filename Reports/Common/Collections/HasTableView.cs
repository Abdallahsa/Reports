using FluentValidation.Results;
using Reports.Common.Exceptions;

namespace Reports.Common.Abstractions.Collections
{
    public abstract class HasTableView : HasPagination
    {
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; }
        public Dictionary<string, string>? Filters { get; set; }

        public HasTableView()
        {
        }
        public bool ValidateFiltersAndSorting(List<string>? allowedFilters, List<string>? allowedSorting)
        {
            ValidateFilters(allowedFilters);
            ValidateSorting(allowedSorting);
            return true;
        }
        private void ValidateSorting(List<string>? allowed)
        {
            if (allowed == null || allowed.Count == 0 || string.IsNullOrWhiteSpace(SortBy) || SortBy.Length < 2)
                return;

            var smallLetterKey = char.ToUpper(SortBy[0]) + SortBy[1..];
            var capitalLetterKey = char.ToLower(SortBy[0]) + SortBy[1..];

            if (!string.IsNullOrEmpty(SortBy) && !allowed.Contains(smallLetterKey) && !allowed.Contains(capitalLetterKey))
            {
                List<ValidationFailure> validationFailures = [new ValidationFailure(nameof(SortBy), $"Invalid property name ({SortBy}). Only values [{string.Join(", ", allowed)}] are allowed.")];
                throw new BadRequestException(validationFailures);
            }
        }

        private void ValidateFilters(List<string>? allowed)
        {
            if (allowed == null || allowed.Count == 0)
                return;

            if (Filters != null && Filters.Count != 0)
            {
                foreach (var filter in Filters)
                {
                    if (string.IsNullOrWhiteSpace(filter.Key) || filter.Key.Length < 2)
                        continue;

                    var smallLetterKey = char.ToUpper(filter.Key[0]) + filter.Key[1..];
                    var capitalLetterKey = char.ToLower(filter.Key[0]) + filter.Key[1..];
                    if (!allowed.Contains(smallLetterKey) && !allowed.Contains(capitalLetterKey))
                    {
                        List<ValidationFailure> validationFailures = [new ValidationFailure(filter.Key, $"Invalid property name ({filter.Key}). Only values [{string.Join(", ", allowed)}] are allowed.")];
                        throw new BadRequestException(validationFailures);
                    }
                }
            }
        }
    }
}
