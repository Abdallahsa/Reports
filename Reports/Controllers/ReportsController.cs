using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reports.Api.Controllers;
using Reports.Api.Domain.Constants;
using Reports.Features.Reportss.Commands.CreateDailyDeputyReport;

namespace Reports.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController : BaseController
    {

        [HttpGet("create-daily-deputy-report")]
        [Authorize(Roles = RoleConstants.LevelZero)]
        public async Task<IActionResult> CreateDailyDeputyReport()
        {
            return Ok(await _mediator.Send(new CreateDailyDeputyReportCommand { }));

        }

        // endpoint to return all reports 
        [HttpGet("all-reports")]
        [Authorize(Roles = RoleConstants.LevelZero)]
        public async Task<IActionResult> GetAllReports()
        {
            
            return Ok("This endpoint will return all reports.");
        }

    }

}
