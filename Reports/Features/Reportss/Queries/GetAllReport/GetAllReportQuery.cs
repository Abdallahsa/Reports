using Reports.Api.Common.Abstractions.Collections;
using Reports.Api.Data;
using Reports.Api.Services;
using Reports.Api.Services.CurrentUser;
using Reports.Common.Abstractions.Collections;
using Reports.Common.Abstractions.Mediator;
using Reports.Features.Reportss.Model;

namespace Reports.Features.Reportss.Queries.GetAllReport
{
    public class GetAllReportQuery : HasTableViewWithDate, ICommand<PagedList<GetReportApprovalModel>>
    {
        public string ? Search { get; set; }
        public bool ? Archive { get; set; }

    }

    //Create handler for GetAllReportQuery

    //public class GetAllReportQueryHandler(
    //    AppDbContext context,
    //    ICurrentUserService currentUserService,
    //    IStorageService storageService

    //    ) : ICommandHandler<GetAllReportQuery, PagedList<GetReportApprovalModel>>
    //{
    //    public Task<PagedList<GetReportApprovalModel>> Handle(GetAllReportQuery request, CancellationToken cancellationToken)
    //    {
    //        try
    //        {

    //            var allowedFields = new List<string> { };
    //            var allowedSorting = new List<string> { };



    //        }
    //        catch ( Exception ex ) {

    //            throw new Exception(ex.Message);

           
    //    }
    //}





}
