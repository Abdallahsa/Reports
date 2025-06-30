
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Mvc;
using Reports.Api.Auth.Models;
using Reports.Api.Auth.Services;
using Reports.Api.Domain.Constants;
using Reports.Api.Features.Auth.Commands.RefreshToken;
using Reports.Application.Auth.Models;
using Reports.Features.Admin.Commands.AddUser;
using Reports.Features.Auth.Commands.UpLoadSignature;
using System.Security.Claims;
using Reports.Api.Features.Auth.Commands.Register;
using Reports.Api.Domain.Entities;
using Reports.Features.Auth.Queries.GetMyProfile;

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
        public async Task<IActionResult> GetRoles()
        {
            return Ok(await authService.GetRolesAsync());
        }

        // method tro add customer
        [HttpPost("add-user")]
        [Authorize(Roles = RoleConstants.Admin)]
        public async Task<IActionResult> AddCustomer([FromForm] AddUserCommand command)
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
           return Ok("Upload Image is Succesd ");
           
        }

        [Authorize]
        [HttpGet("my-profile")]
        public async Task<ActionResult<GetMyProfileQuery>> GetMyProfile()
        {
            var query = new GetMyProfileQuery();
            var res = await _mediator.Send(query);
            return Ok(res);
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
