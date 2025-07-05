using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reports.Api.Data;
using Reports.Common.Abstractions.Mediator;
using Reports.Common.Exceptions;
using Reports.Features.Approval.Models;
using Reports.Features.Reportss.Model;

namespace Reports.Features.Approval.Queries.GetReportApprovalHistory
{
    public class GetReportApprovalHistoryQuery : ICommand<ReportApprovalHistoryModel>
    {
        public required int ReportId { get; set; }
    }

    // Handler GetReportApprovalHistoryQueryHandler
    public class GetReportApprovalHistoryQueryHandler
        (
          AppDbContext _context
        ) : ICommandHandler<GetReportApprovalHistoryQuery, ReportApprovalHistoryModel>
    {


        public async Task<ReportApprovalHistoryModel> Handle(GetReportApprovalHistoryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // تحقق من وجود التقرير
                var exists = await _context.Reports.AnyAsync(r => r.Id == request.ReportId, cancellationToken);
                if (!exists)
                    throw new NotFoundException("Report", request.ReportId);

                // استرجاع كل الموافقات للتقرير
                var approvals = await _context.Reports
                    .Include(r => r.Approvals)
                    .Where(r => r.Id == request.ReportId)
                    .Select(r => new ReportApprovalHistoryModel
                    {
                        Id = r.Id,
                        GehaCode = r.GehaCode,
                        ShoabaName = r.ShoabaName,
                        Description = r.Description,
                        ReportType = r.ReportType.ToArabic(),
                        CreatedAt = r.CreatedAt,
                        Approvals = r.Approvals.Select(a => new GetReportApprovalModel
                        {
                            Id = a.Id,
                            ApprovalDate = a.ApprovalDate,
                            ApprovalStatus = a.ApprovalStatus.ToString(),
                            Geha = a.Geha
                        })
                        .ToList()

                    })
                    .FirstAsync(cancellationToken)
                    ?? throw new NotFoundException("Report", request.ReportId);

                return approvals;


            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }

        }

    }

    // validation
    public class GetReportApprovalHistoryQueryValidator : AbstractValidator<GetReportApprovalHistoryQuery>
    {
        public GetReportApprovalHistoryQueryValidator(AppDbContext context)
        {
            RuleFor(x => x.ReportId)
                .GreaterThan(0).WithMessage("Report ID must be greater than 0.");

            //check if report exists
            RuleFor(x => x.ReportId)
                .MustAsync(async (id, cancellationToken) =>
                {

                    return await context.Reports.AnyAsync(r => r.Id == id, cancellationToken);
                })
                .WithMessage("Report not found.");

        }
    }

}

