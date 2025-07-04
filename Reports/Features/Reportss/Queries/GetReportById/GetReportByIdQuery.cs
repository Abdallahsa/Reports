using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reports.Api.Data;
using Reports.Api.Services.CurrentUser;
using Reports.Common.Abstractions.Mediator;
using Reports.Common.Exceptions;
using Reports.Features.Reportss.Model;

namespace Reports.Features.Reportss.Queries.GetReportById
{
    public class GetReportByIdQuery : ICommand<GetReportByIdModel>
    {
        public required int Id { get; set; }
    }

    // Handler for GetReportByIdQuery
    public class GetReportByIdQueryHandler(AppDbContext _context) : ICommandHandler<GetReportByIdQuery, GetReportByIdModel>
    {


        public async Task<GetReportByIdModel> Handle(GetReportByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {

                var report = await _context.Reports
                    .Where(r => r.Id == request.Id)
                    .Select(r => new GetReportByIdModel
                    {
                        Id = r.Id,
                        GehaCode = r.GehaCode,
                        ShoabaName = r.ShoabaName,
                        Description = r.Description,
                        ReportType = r.ReportType.ToArabic(),
                        CreatedAt = r.CreatedAt,
                        IsRejected = r.IsRejected,
                        CurrentApprovalLevel = r.CurrentApprovalLevel.ToString(),
                        IsApprovedByRA = r.IsApprovedByRA,
                        FilePath = r.FilePath,
                        Approvals = r.Approvals.Select(a => new GetReportApprovalModel
                        {
                            Geha = a.Geha,
                            ApprovalStatus = a.ApprovalStatus.ToString(),
                            ApprovalDate = a.ApprovalDate
                        }).ToList()
                    })
                    .FirstOrDefaultAsync(cancellationToken)
                    ?? throw new NotFoundException("Report not found");

                return report;
            }
            catch (Exception ex)
            {
                throw new NotFoundException("Report not found", ex);
            }


        }
    }

    // Query Validator
    public class GetReportByIdQueryValidator : AbstractValidator<GetReportByIdQuery>
    {
        public GetReportByIdQueryValidator(
            AppDbContext _context,
            ICurrentUserService _currentUserService
            )
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Report ID must be greater than 0.");

            RuleFor(x => x.Id)
                .MustAsync(async (id, cancellationToken) =>
                {
                    return await _context.Reports.AnyAsync(r => r.Id == id, cancellationToken);
                })
                .WithMessage("Report with the specified ID does not exist.");


        }
    }


}
