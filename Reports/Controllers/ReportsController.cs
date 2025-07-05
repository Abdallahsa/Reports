using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reports.Api.Common.Abstractions.Collections;
using Reports.Api.Controllers;
using Reports.Api.Domain.Constants;
using Reports.Api.Domain.Entities;
using Reports.Features.Reportss.Commands.CreateReport;
using Reports.Features.Reportss.Commands.LockReport;
using Reports.Features.Reportss.Commands.UnlockReport;
using Reports.Features.Reportss.Model;
using Reports.Features.Reportss.Queries.GetAllReport;
using Reports.Features.Reportss.Queries.GetAvailableReportTypes;
using Reports.Features.Reportss.Queries.GetReportById;

namespace Reports.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportsController() : BaseController
    {


        [HttpPost("create")]
        [Authorize(Roles = $"{RoleConstants.LevelZero},{RoleConstants.LevelOne},{RoleConstants.LevelTwo},{RoleConstants.LevelThree},{RoleConstants.LevelFour}")]
        public async Task<IActionResult> CreateReport([FromForm] CreateReportCommand command)
        {
            return Ok(await _mediator.Send(command));
        }



        [HttpPost("unlock-report/{reportId}")]
        [Authorize(Roles = $"{RoleConstants.LevelZero},{RoleConstants.LevelOne},{RoleConstants.LevelTwo},{RoleConstants.LevelThree},{RoleConstants.LevelFour}")]
        public async Task<IActionResult> UnlockReport(int reportId)
        {
            var result = await _mediator.Send(new UnlockReportCommand { ReportId = reportId });
            return Ok(result);
        }



        [HttpPost("lock-report/{reportId}")]
        [Authorize(Roles = $"{RoleConstants.LevelZero},{RoleConstants.LevelOne},{RoleConstants.LevelTwo},{RoleConstants.LevelThree},{RoleConstants.LevelFour}")]
        public async Task<IActionResult> LockReport(int reportId)
        {
            var result = await _mediator.Send(new LockReportCommand { ReportId = reportId });
            return Ok(result);
        }



        // endpoint to return all reports 
        [HttpPost("all-reports")]
        [Authorize(Roles = $"{RoleConstants.LevelZero},{RoleConstants.LevelOne},{RoleConstants.LevelTwo},{RoleConstants.LevelThree},{RoleConstants.LevelFour}")]
        public async Task<PagedList<GetAllReportModel>> GetAllReports(GetAllReportQuery query)
        {
            return await _mediator.Send(query);

        }


        // endpoint to return report by id

        [HttpGet("{id}")]
        [Authorize(Roles = $"{RoleConstants.LevelZero},{RoleConstants.LevelOne},{RoleConstants.LevelTwo},{RoleConstants.LevelThree},{RoleConstants.LevelFour}")]
        public async Task<IActionResult> GetReportById(int id)
        {
            var result = await _mediator.Send(new GetReportByIdQuery { Id = id });
            return Ok(result);
        }



        [Authorize]
        [HttpGet("my-available-reports")]
        public async Task<IActionResult> GetMyAvailableReports()
        {
            var result = await _mediator.Send(new GetAvailableReportTypesQuery());
            return Ok(result);
        }

        [HttpGet("Geha-list")]
        public ActionResult<ICollection<string>> GetGehaStatus()
        {
            var statuses = Enum.GetNames<Geha>();
            return Ok(statuses);
        }

        [HttpGet("Level-list")]
        public ActionResult<ICollection<string>> GetLevelStatus()
        {
            var statuses = Enum.GetNames<Level>();
            return Ok(statuses);
        }




    }

}
