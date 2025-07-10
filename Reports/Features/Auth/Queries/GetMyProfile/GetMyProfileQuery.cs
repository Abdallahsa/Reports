using Microsoft.EntityFrameworkCore;
using Reports.Api.Data;
using Reports.Api.Domain.Entities;
using Reports.Api.Features.Common.Models;
using Reports.Api.Services;
using Reports.Api.Services.CurrentUser;
using Reports.Common.Abstractions.Mediator;
using Reports.Common.Exceptions;

namespace Reports.Features.Auth.Queries.GetMyProfile
{
    public class GetMyProfileQuery : ICommand<UserDto>
    {

    }

    public class GetMyProfileQueryHandler(
       AppDbContext context,
       ICurrentUserService currentUserService,
       IStorageService storageService
       ) : ICommandHandler<GetMyProfileQuery, UserDto>
    {
        public async Task<UserDto> Handle(GetMyProfileQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await context.Set<User>()
                    .Select(r => new UserDto()
                    {
                        Id = r.Id,
                        Email = r.Email != null ? r.Email : string.Empty,
                        IsActive = r.EmailConfirmed,
                        UserName = r.UserName != null ? r.UserName : string.Empty,
                        SignaturePath = r.SignaturePath != null ? storageService.GetFullPath(r.SignaturePath, false) : string.Empty,
                        Geha = r.Geha,
                        Level = r.Level.ToString()

                    })
                    .FirstOrDefaultAsync(x => x.Id == currentUserService.UserId, cancellationToken)
                     ?? throw new Exception("User not found");
                return user;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }


        }
    }

}
