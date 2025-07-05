using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reports.Api.Data;
using Reports.Common.Abstractions.Mediator;
using Reports.Common.Exceptions;
using Reports.Domain.Entities;
using Reports.Features.ForgotPasswordRequests.Models;

namespace Reports.Features.ForgotPasswordRequests.Queries.GetForgotPasswordRequestById
{
    public class GetForgotPasswordRequestByIdQuery : ICommand<ForgotPasswordRequestModel>
    {
        public int Id { get; set; }
    }

    public class GetForgotPasswordRequestByIdQueryHandler(
               AppDbContext _context
           ) : ICommandHandler<GetForgotPasswordRequestByIdQuery, ForgotPasswordRequestModel>
    {
        public async Task<ForgotPasswordRequestModel> Handle(GetForgotPasswordRequestByIdQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var forgotPasswordRequest = await _context.Set<ForgotPasswordRequest>()
               .Where(x => x.Id == request.Id)
               .Select(x => new ForgotPasswordRequestModel
               {
                   Id = x.Id,
                   Email = x.Email,
                   CreatedAt = x.CreatedAt,
                   IsUsed = x.IsUsed,
                   Phone = x.Phone
               })
               .FirstOrDefaultAsync(cancellationToken)
               ?? throw new NotFoundException($"Forgot password request with ID {request.Id} not found.");


                return forgotPasswordRequest;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
        }
    }

    public class GetForgotPasswordRequestByIdQueryValidator : AbstractValidator<GetForgotPasswordRequestByIdQuery>
    {
        public GetForgotPasswordRequestByIdQueryValidator(AppDbContext _context)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Id is required")
                .GreaterThan(0).WithMessage("Id must be greater than 0");

            // check if the Id exists in the database
            RuleFor(x => x.Id)
                .MustAsync(async (id, cancellation) =>
                {
                    return await _context.Set<ForgotPasswordRequest>().AnyAsync(x => x.Id == id, cancellation);
                }).WithMessage("Forgot password request with the specified ID does not exist.");

        }
    }

}
