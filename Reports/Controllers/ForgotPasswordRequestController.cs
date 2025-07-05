using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reports.Api.Controllers;
using Reports.Api.Domain.Constants;
using Reports.Features.ForgotPasswordRequests.Commands.ChangePassword;
using Reports.Features.ForgotPasswordRequests.Commands.CreateForgotPasswordRequests;
using Reports.Features.ForgotPasswordRequests.Queries.GetAllForgotPasswordRequest;
using Reports.Features.ForgotPasswordRequests.Queries.GetForgotPasswordRequestById;

namespace Reports.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ForgotPasswordRequestController : BaseController
    {

        [HttpPost("request-forgot-password")]
        public async Task<IActionResult> RequestForgotPassword([FromBody] CreateForgotPasswordRequestCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new { success = result });
        }


        [HttpPost("get-all-forgot-password-requests")]
        [Authorize(Roles = RoleConstants.Admin)]
        public async Task<IActionResult> GetAllForgotPasswordRequests([FromBody] GetAllForgotPasswordRequestQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize(Roles = RoleConstants.Admin)]
        public async Task<IActionResult> GetById(int id)
        {
            var result = await _mediator.Send(new GetForgotPasswordRequestByIdQuery { Id = id });
            return Ok(result);
        }

        // Endpoint to change the password
        [HttpPost("change-password")]
        [Authorize(Roles = RoleConstants.Admin)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return Ok(new { success = result });
        }


    }
}
