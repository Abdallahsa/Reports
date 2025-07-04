using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reports.Api.Common.Abstractions.Collections;
using Reports.Api.Controllers;
using Reports.Features.Approval.Commands.ApproveReport;
using Reports.Features.Approval.Commands.RejectReport;
using Reports.Features.Approval.Queries.GetTodayPendingApprovalReports;
using Reports.Features.Reportss.Model;
using Reports.Features.Reportss.Queries.GetMyApprovedReports;

namespace Reports.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApprovalController : BaseController
    {

        [Authorize]
        [HttpPost("approve-report/{reportId}")]
        public async Task<IActionResult> ApproveReport(int reportId)
        {
            await _mediator.Send(new ApproveReportCommand { ReportId = reportId });
            return Ok("Report approved successfully");
        }

        [Authorize]
        [HttpPost("reject-report/{reportId}")]
        public async Task<IActionResult> RejectReport(int reportId)
        {
            await _mediator.Send(new RejectReportCommand { ReportId = reportId });
            return Ok("Report rejected successfully");
        }

        [Authorize]
        [HttpPost("today-pending")]
        public async Task<IActionResult> GetTodayPendingApprovalReports(GetTodayPendingApprovalReportsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }


        // endpoint to return all reports i marked approved
        [Authorize]
        [HttpPost("my-approved-reports")]
        public async Task<PagedList<GetAllReportModel>> GetMyApprovedReports(GetMyApprovedReportsQuery query)
        {
            return await _mediator.Send(query);

        }

    }
}
