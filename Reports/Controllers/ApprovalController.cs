using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reports.Api.Common.Abstractions.Collections;
using Reports.Api.Controllers;
using Reports.Api.Domain.Constants;
using Reports.Features.Approval.Commands.ApproveReport;
using Reports.Features.Approval.Commands.RejectReport;
using Reports.Features.Approval.Models;
using Reports.Features.Approval.Queries.GetReportApprovalHistory;
using Reports.Features.Approval.Queries.GetTodayPendingApprovalReports;
using Reports.Features.Reportss.Model;
using Reports.Features.Reportss.Queries.GetMyApprovedReports;

namespace Reports.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApprovalController : BaseController
    {

        [HttpPost("approve-report/{reportId}")]
        [Authorize(Roles = $"{RoleConstants.LevelZero},{RoleConstants.LevelOne},{RoleConstants.LevelTwo},{RoleConstants.LevelThree},{RoleConstants.LevelFour}")]
        public async Task<IActionResult> ApproveReport(int reportId)
        {
            await _mediator.Send(new ApproveReportCommand { ReportId = reportId });
            return Ok("Report approved successfully");
        }

        [HttpPost("reject-report/{reportId}")]
        [Authorize(Roles = $"{RoleConstants.LevelZero},{RoleConstants.LevelOne},{RoleConstants.LevelTwo},{RoleConstants.LevelThree},{RoleConstants.LevelFour}")]
        public async Task<IActionResult> RejectReport(int reportId)
        {
            await _mediator.Send(new RejectReportCommand { ReportId = reportId });
            return Ok("Report rejected successfully");
        }

        [HttpPost("today-pending")]
        [Authorize(Roles = $"{RoleConstants.LevelZero},{RoleConstants.LevelOne},{RoleConstants.LevelTwo},{RoleConstants.LevelThree},{RoleConstants.LevelFour}")]
        public async Task<ActionResult<PagedList<GetAllReportModel>>> GetTodayPendingApprovalReports(GetTodayPendingApprovalReportsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }


        // endpoint to return all reports i marked approved
        [HttpPost("my-approved-reports")]
        [Authorize(Roles = $"{RoleConstants.LevelZero},{RoleConstants.LevelOne},{RoleConstants.LevelTwo},{RoleConstants.LevelThree},{RoleConstants.LevelFour}")]
        public async Task<ActionResult<PagedList<GetAllReportModel>>> GetMyApprovedReports(GetMyApprovedReportsQuery query)
        {
            return Ok(await _mediator.Send(query));

        }

        // endpoint to return all history of reports i approved
        [HttpPost("{reportId}/approval-history")]
        [Authorize(Roles = $"{RoleConstants.LevelZero},{RoleConstants.LevelOne},{RoleConstants.LevelTwo},{RoleConstants.LevelThree},{RoleConstants.LevelFour}")]
        public async Task<ActionResult<ReportApprovalHistoryModel>> GetReportApprovalHistory(int reportId)
        {

            return Ok(await _mediator.Send(new GetReportApprovalHistoryQuery { ReportId = reportId }));
        }

    }
}
