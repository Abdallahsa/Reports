using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reports.Api.Auth.Models;
using Reports.Api.Auth.Services;
using Reports.Api.Common.Abstractions.Collections;
using Reports.Api.Domain.Constants;
using Reports.Api.Features.Auth.Commands.RefreshToken;
using Reports.Api.Features.Auth.Commands.Register;
using Reports.Api.Features.Common.Models;
using Reports.Application.Auth.Models;
using Reports.Features.Admin.Commands.AddUser;
using Reports.Features.Auth.Commands.UpLoadSignature;
using Reports.Features.Auth.Queries.GetAllUsers;
using Reports.Features.Auth.Queries.GetMyProfile;
using Reports.Features.Auth.Queries.GetUsersStatistics;
using System.Security.Claims;

namespace Reports.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService) : BaseController
    {
        // method to test home
        [HttpGet("/")]
        public IActionResult Home()
        {
            return Ok("Welcome to Reports API.");
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponseModel>> Login([FromBody] LoginModel model)
        {
            try
            {
                var responseModel = await authService.LoginAsync(model);
                return Ok(responseModel);
            }
            catch (UnauthorizedAccessException e)
            {
                return Unauthorized(e.Message);
            }
        }

        //method to change password with old password
        [Authorize]
        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? throw new UnauthorizedAccessException("User not found.");
            var result = await authService.ChangePasswordAsync(userId, model);

            if (result.Succeeded)
                return Ok("Password changed successfully.");

            return BadRequest(result.Errors);
        }


        [HttpPost("refresh")]
        public async Task<ActionResult<LoginResponseModel>> Refresh([FromBody] RefreshTokenCommand command)
        {
            var res = await _mediator.Send(command);
            return Ok(res);
        }

        [Authorize]
        [HttpGet("get-roles")]
        public async Task<ActionResult<ICollection<string>>> GetRoles()
        {
            return Ok(await authService.GetRolesAsync());
        }

        // method tro add customer
        [HttpPost("add-user")]
        [Authorize(Roles = RoleConstants.Admin)]
        public async Task<ActionResult<ICollection<string>>> AddCustomer([FromForm] AddUserCommand command)
        {
            return Ok(await _mediator.Send(command));

        }
        [HttpPost("add-admin")]
        [Authorize(Roles = RoleConstants.Admin)]
        public async Task<IActionResult> Register([FromForm] RegisterCommand command)
        {
            await _mediator.Send(command);
            return Ok("User registered successfully.");
        }

        [HttpPost("upload-signature")]
        [Authorize]
        public async Task<IActionResult> UpLoadSignature([FromForm] UpLoadSignatureCommand command)
        {
            await _mediator.Send(command);
            return Ok("Upload Image is successful ");

        }

        [Authorize]
        [HttpGet("my-profile")]
        public async Task<ActionResult<UserDto>> GetMyProfile()
        {

            return Ok(await _mediator.Send(new GetMyProfileQuery()));
        }

        // endpoint return statistics of users
        [HttpGet("statistics")]
        [Authorize(Roles = RoleConstants.Admin)]
        public async Task<IActionResult> GetUsersStatistics()
        {
            var result = await _mediator.Send(new GetUsersStatisticsQuery());
            return Ok(result);
        }

        // endpoint return all users
        [HttpPost("all-users")]
        [Authorize(Roles = RoleConstants.Admin)]
        public async Task<ActionResult<PagedList<UserDto>>> GetAllUsers([FromBody] GetAllUsersQuery query)
        {
            return Ok(await _mediator.Send(query));
        }


    }

}
