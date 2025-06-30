
using Reports.Api.Data;
using Reports.Api.Domain.Entities;
using Reports.Api.Services;
using Reports.Api.Services.CurrentUser;
using Reports.Common.Abstractions.Mediator;
using Reports.Common.Exceptions;

namespace Reports.Features.Auth.Commands.UpLoadSignature
{
    public class UpLoadSignatureCommand :ICommand
    {

        public required  IFormFile Signature { get; set; }

    }

    // Create Handler for UpLoadSignatureCommand

    public class UpLoadSignatureCommandHandler(ICurrentUserService currentUserService,
        AppDbContext context ,
        IStorageService storageService
        ) : ICommandHandler<UpLoadSignatureCommand>
    {
        public async Task Handle(UpLoadSignatureCommand request, CancellationToken cancellationToken)
        {  
            //using make transactional start
            using var transaction = context.Database.BeginTransaction();
            try
            {
                var user = await context.Set<User>()
                    .FindAsync(currentUserService.UserId, cancellationToken)
                     ?? throw new NotFoundException(nameof(User), currentUserService.UserId);

                user.SignaturePath = await storageService.SaveFileAsync(request.Signature);

                context.Set<User>().Update(user);

                await context.SaveChangesAsync();

               await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
               await transaction.RollbackAsync();
                throw new BadRequestException(ex.Message);
            }


        }
    }
}
