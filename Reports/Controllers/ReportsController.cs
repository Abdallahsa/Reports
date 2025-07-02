using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reports.Api.Controllers;
using Reports.Api.Domain.Constants;
using Reports.Features.Reportss.Commands.CreateDailyDeputyReport;
using Reports.Service.SaveReport;

namespace Reports.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController(ITemplateReportService templateReportService) : BaseController
    {

        [HttpGet("create-daily-deputy-report")]
        [Authorize(Roles = RoleConstants.LevelZero)]
        public async Task<IActionResult> CreateDailyDeputyReport()
        {
            return Ok(await _mediator.Send(new CreateDailyDeputyReportCommand { }));

        }

        //// endpoint to return all reports 
        //[HttpGet("all-reports")]
        //[Authorize(Roles = RoleConstants.LevelZero)]
        //public async Task<IActionResult> GetAllReports()
        //{

        //    return Ok("This endpoint will return all reports.");
        //}

        [Authorize]
        [HttpGet("preview-report/{fileName}")]
        public IActionResult PreviewReport(string fileName)
        {
            var decryptedBytes = templateReportService.GetDecryptedFile(fileName);

            var contentType = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";

            // نطلب من المتصفح إنه يعرض الملف inline بدل download
            Response.Headers.Add("Content-Disposition", $"inline; filename=\"{fileName}\"");

            return File(decryptedBytes, contentType);
        }

    }

}
