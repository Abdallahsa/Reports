using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reports.Api.Common.Abstractions.Collections;
using Reports.Api.Controllers;
using Reports.Features.Notifications.Commands.SendNotificationToAll;
using Reports.Features.Notifications.Models;
using Reports.Features.Notifications.Queries.GetMyNotifications;
using Reports.Features.Notifications.Queries.GetNotificationById;

namespace Reports.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : BaseController
    {
        [HttpPost("send-to-all")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SendNotificationToAll([FromForm] SendNotificationToAllCommand command)
        {
            await _mediator.Send(command);
            return Ok(new { Message = "Notifications sent successfully to all users." });
        }

        [HttpPost("my")]
        [Authorize]
        public async Task<ActionResult<PagedList<GetMyNotificationsModel>>> GetMyNotifications(GetMyNotificationsQuery query)
        {
            return Ok(await _mediator.Send(query));
        }

        // endpoint return notification by id
        [HttpGet("notification/{id}")]
        [Authorize]
        public async Task<ActionResult<GetNotificationByIdModel>> GetNotificationById(int id)
        {
            return Ok(await _mediator.Send(new GetNotificationByIdQuery { Id = id }));
        }


    }
}
