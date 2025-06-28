using Reports.Api.Features.Common.Validators;
using Reports.Common.Abstractions.Collections;

namespace Reports.Api.Common.Abstractions.Collections
{
    public class HasTableViewWithDate : HasTableView
    {

        public DateTime? StartRange { get; set; }


        [DateRangeValidation(nameof(StartRange))]
        public DateTime? EndRange { get; set; }

    }
}
