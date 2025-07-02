using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reports.Api.Controllers;
using Reports.Api.Domain.Constants;
using Reports.Features.Reportss.Commands.CreateDailyDeputyReport;
using Reports.Features.Reportss.Commands.LockReport;
using Reports.Features.Reportss.Commands.UnlockReport;
using Reports.Features.Reportss.Queries.GetAvailableReportTypes;
using Reports.Service.SaveReport;

namespace Reports.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController() : BaseController
    {

        [HttpPost("create-daily-deputy-report")]
        [Authorize(Roles = RoleConstants.LevelZero)]
        public async Task<IActionResult> CreateDailyDeputyReport()
        {
            return Ok(await _mediator.Send(new CreateDailyDeputyReportCommand { }));

        }

        [Authorize]
        [HttpPost("unlock-report/{reportId}")]
        public async Task<IActionResult> UnlockReport(int reportId)
        {
            var result = await _mediator.Send(new UnlockReportCommand { ReportId = reportId });
            return Ok(result);
        }


        [Authorize]
        [HttpPost("lock-report/{reportId}")]
        public async Task<IActionResult> LockReport(int reportId)
        {
            var result = await _mediator.Send(new LockReportCommand { ReportId = reportId });
            return Ok(result);
        }



        // endpoint to return all reports 
        [HttpGet("all-reports")]
        [Authorize(Roles = RoleConstants.LevelZero)]
        public async Task<IActionResult> GetAllReports()
        {

            return Ok("This endpoint will return all reports.");
        }

        [Authorize]
        [HttpGet("my-available-reports")]
        public async Task<IActionResult> GetMyAvailableReports()
        {
            var result = await _mediator.Send(new GetAvailableReportTypesQuery());
            return Ok(result);
        }


        //[Authorize]
        //[HttpGet("preview-report/{fileName}")]
        //public IActionResult PreviewReport(string fileName)
        //{
        //    var decryptedBytes = templateReportService.GetDecryptedFile(fileName);

        //    var contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

        //    // نطلب من المتصفح إنه يعرض الملف inline بدل download
        //    Response.Headers.Add("Content-Disposition", $"inline; filename=\"{fileName}\"");

        //    return File(decryptedBytes, contentType);
        //}

    }

}
