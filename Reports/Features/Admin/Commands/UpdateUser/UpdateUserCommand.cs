using Reports.Api.Auth.Services;
using Reports.Api.Services.CurrentUser;
using Reports.Common.Abstractions.Mediator;
using Reports.Features.Admin.Models;
using Reports.Service.LoggingService;

namespace Reports.Features.Admin.Commands.UpdateUser
{
    public class UpdateUserCommand : ICommand<int>
    {
        public required int UserId { get; set; }
        public string? Password { get; set; }
        public IFormFile? Signature { get; set; }
    }

    //  handler of UpdateUserCommand
    public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, int>
    {
        private readonly IAuthService authService;
        private readonly ICurrentUserService currentUserService;
        private readonly ILoggingService _loggingService;

        public UpdateUserCommandHandler(IAuthService authService, ICurrentUserService currentUserService, ILoggingService loggingService)
        {
            this.authService = authService;
            this.currentUserService = currentUserService;
            _loggingService = loggingService;
        }

        public async Task<int> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var updateResult = await authService.UpdateProfileAsync
                    (new UpdateProfileModel
                    {
                        UserId = request.UserId,
                        Password = request.Password,
                        Signature = request.Signature
                    });

                // log
                await _loggingService.LogInformation("User with ID {UserId} updated successfully by user {CurrentUserId} at {Time}",
                                                           request.UserId, currentUserService.UserId, DateTime.UtcNow);

                return updateResult;
            }
            catch (Exception ex)
            {
                await _loggingService.LogError("Error updating user with ID {UserId}: {Message}", ex, request.UserId, ex.Message);
                throw new Exception($"Failed to update User: {ex.Message}", ex);
            }
        }
    }


}
