using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Reports.Api.Common.Abstractions.Collections;
using Reports.Api.Controllers;
using Reports.Api.Domain.Constants;
using Reports.Features.ForgotPasswordRequests.Commands.ChangePassword;
using Reports.Features.ForgotPasswordRequests.Commands.CreateForgotPasswordRequests;
using Reports.Features.ForgotPasswordRequests.Models;
using Reports.Features.ForgotPasswordRequests.Queries.GetAllForgotPasswordRequest;
using Reports.Features.ForgotPasswordRequests.Queries.GetForgotPasswordRequestById;

namespace Reports.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class ForgotPasswordRequestController : BaseController
    {

        [HttpPost("request-forgot-password")]
        public async Task<ActionResult<string>> RequestForgotPassword([FromBody] CreateForgotPasswordRequestCommand command)
        {
            return Ok(await _mediator.Send(command));
        }


        [HttpPost("get-all-forgot-password-requests")]
        [Authorize(Roles = RoleConstants.Admin)]
        public async Task<ActionResult<PagedList<ForgotPasswordRequestModel>>> GetAllForgotPasswordRequests([FromBody] GetAllForgotPasswordRequestQuery query)
        {
            return Ok(await _mediator.Send(query));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = RoleConstants.Admin)]
        public async Task<ActionResult<ForgotPasswordRequestModel>> GetById(int id)
        {
            return Ok(await _mediator.Send(new GetForgotPasswordRequestByIdQuery { Id = id }));
        }

        // Endpoint to change the password
        [HttpPost("change-password")]
        [Authorize(Roles = RoleConstants.Admin)]
        public async Task<ActionResult<string>> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            return Ok(await _mediator.Send(command));
        }


    }
}
