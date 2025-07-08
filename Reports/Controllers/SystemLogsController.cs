using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reports.Api.Common.Abstractions.Collections;
using Reports.Api.Controllers;
using Reports.Api.Domain.Constants;
using Reports.Features.SystemLogs.Models;
using Reports.Features.SystemLogs.Queries.GetAllLog;
using Reports.Features.SystemLogs.Queries.GetLogsStatistics;

namespace Reports.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SystemLogsController : BaseController
    {
        [HttpPost("statistics")]
        [Authorize(Roles = RoleConstants.Admin)]
        public async Task<IActionResult> GetStatistics([FromBody] GetLogsStatisticsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        // Endpoint return all Logs
        [HttpPost]
        [Authorize(Roles = RoleConstants.Admin)]
        public async Task<ActionResult<PagedList<GetAllLogModel>>> GetAllLogs([FromBody] GetAllLogQuery query)
        {
            return Ok(await _mediator.Send(query));
        }


    }
}
