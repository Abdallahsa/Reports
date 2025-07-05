using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reports.Api.Controllers;
using Reports.Features.Notifications.Commands.SendNotificationToAll;
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
        public async Task<IActionResult> GetMyNotifications(GetMyNotificationsQuery query)
        {
            var notifications = await _mediator.Send(query);
            return Ok(notifications);
        }

        // endpoint return notification by id
        [HttpGet("notification/{id}")]
        [Authorize]
        public async Task<IActionResult> GetNotificationById(int id)
        {
            var result = await _mediator.Send(new GetNotificationByIdQuery { Id = id });
            return Ok(result);
        }


    }
}
