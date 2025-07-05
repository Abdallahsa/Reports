using System.Linq.Dynamic.Core;

namespace TwoHO.Api.Extensions
{
    public static class IQueryableExtension
    {
        public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> query, Dictionary<string, string>? filters)
        {
            if (filters != null && filters.Count != 0)
            {
                foreach (var filter in filters)
                {
                    if (string.IsNullOrEmpty(filter.Value))
                        continue;

                    var normalizedKey = char.ToUpper(filter.Key[0]) + filter.Key[1..];
                    if (int.TryParse(filter.Value, out int intValue))
                    {
                        query = query.Where($"{normalizedKey} == @0", intValue);
                    }
                    else if (bool.TryParse(filter.Value, out bool boolValue))
                    {
                        query = query.Where($"{normalizedKey} == @0", boolValue);
                    }
                    else
                    {
                        query = query.Where($"{normalizedKey}.Contains(@0)", filter.Value);
                    }

                }
            }

            return query;
        }

        ////craete filter using arbic and english 
        //public static IQueryable<T> ApplyFilters<T>(this IQueryable<T> query, Dictionary<string, string>? filters, ICurrentUserService _currentUserService)
        //{
        //    var language = _currentUserService.Lang;
        //    if (filters != null && filters.Count != 0)
        //    {
        //        foreach (var filter in filters)
        //        {
        //            if (string.IsNullOrEmpty(filter.Value))
        //                continue;

        //            // Determine if Arabic or English should be used
        //            var normalizedKey = char.ToUpper(filter.Key[0]) + filter.Key[1..];
        //            var fieldKey = (language == "ar" && (normalizedKey == "Name" || normalizedKey == "Description"))
        //                ? $"{normalizedKey}_ar"
        //                : normalizedKey;

        //            //check if attribute is equal Price convert it to Price_USD or Price_SAR based on the currency
        //            if (normalizedKey == "Price")
        //            {
        //                fieldKey = (_currentUserService.Currency == AppCurrency.USD) ? "Price_USD" : "Price_SAR";
        //            }

        //            if (int.TryParse(filter.Value, out int intValue))
        //            {
        //                query = query.Where($"{fieldKey} == @0", intValue);
        //            }
        //            else if (bool.TryParse(filter.Value, out bool boolValue))
        //            {
        //                query = query.Where($"{fieldKey} == @0", boolValue);
        //            }
        //            else
        //            {
        //                query = query.Where($"{fieldKey}.Contains(@0)", filter.Value);
        //            }
        //        }
        //    }

        //    return query;

        //}




        public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, string? sortBy, string? sortDirection)
        {
            // If sortBy is provided, sort by the specified column
            if (!string.IsNullOrEmpty(sortBy))
            {
                var direction = string.IsNullOrEmpty(sortDirection) || string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase)
                                ? "descending"
                                : "ascending";

                var normalizedKey = char.ToUpper(sortBy[0]) + sortBy[1..];


                query = query.OrderBy($"{normalizedKey} {direction}");
            }

            return query;
        }

        //public static IQueryable<T> ApplySorting<T>(this IQueryable<T> query, string? sortBy, string? sortDirection, string? currency)
        //{
        //    // If sortBy is provided, sort by the specified column
        //    if (!string.IsNullOrEmpty(sortBy))
        //    {
        //        var direction = string.IsNullOrEmpty(sortDirection) || string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase)
        //                        ? "descending"
        //                        : "ascending";

        //        var normalizedKey = char.ToUpper(sortBy[0]) + sortBy[1..];

        //        //check if attribute is equal Price convert it to Price_USD or Price_SAR based on the currency
        //        if (normalizedKey == "Price")
        //        {
        //            normalizedKey = (currency == AppCurrency.USD) ? "Price_USD" : "Price_SAR";
        //        }



        //        query = query.OrderBy($"{normalizedKey} {direction}");
        //    }

        //    return query;
        //}

        public static IQueryable<T> ApplyDateRangeFilter<T>(this IQueryable<T> query, DateTime? startDate, DateTime? endDate, string dateProperty = "CreatedAt")
        {
            // If only StartDate is provided, filter all records from that date onward
            if (startDate.HasValue && !endDate.HasValue)
            {
                query = query.Where($"{dateProperty} >= @0", startDate.Value);
            }
            // If only EndDate is provided, filter all records up until that date
            else if (!startDate.HasValue && endDate.HasValue)
            {
                query = query.Where($"{dateProperty} <= @0", endDate.Value);
            }
            // If both StartDate and EndDate are provided, filter between the dates
            else if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where($"{dateProperty} >= @0 && {dateProperty} <= @1", startDate.Value, endDate.Value);
            }

            return query;
        }

        ////method filter price range 
        //public static IQueryable<T> ApplyPriceRangeFilter<T>(this IQueryable<T> query, decimal? minPrice, decimal? maxPrice)
        //{
        //    var priceProperty = "Price";

        //    if (minPrice == 0 && maxPrice == decimal.MaxValue)
        //    {
        //        return query;
        //    }

        //    // If only MinPrice is provided, filter all records from that price onward
        //    if (minPrice.HasValue && !maxPrice.HasValue)
        //    {
        //        query = query.Where($"{priceProperty} >= @0", minPrice.Value);
        //    }
        //    // If only MaxPrice is provided, filter all records up until that price
        //    else if (!minPrice.HasValue && maxPrice.HasValue)
        //    {
        //        query = query.Where($"{priceProperty} <= @0", maxPrice.Value);
        //    }
        //    // If both MinPrice and MaxPrice are provided, filter between the prices
        //    else if (minPrice.HasValue && maxPrice.HasValue)
        //    {
        //        query = query.Where($"{priceProperty} >= @0 && {priceProperty} <= @1", minPrice.Value, maxPrice.Value);
        //    }

        //    return query;
        //}

        ////create method to filter by maximum ratting
        //public static IQueryable<T> ApplyMaxRattingFilter<T>(this IQueryable<T> query, double? maxRatting)
        //{
        //    if (maxRatting.HasValue)
        //    {
        //        query = query.Where("Ratting <= @0", maxRatting.Value); // Remove the condition "Ratting > 0"
        //    }

        //    return query;
        //}


    }
}
