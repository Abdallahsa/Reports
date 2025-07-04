using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reports.Api.Data;
using Reports.Api.Domain.Entities;
using Reports.Api.Services.CurrentUser;
using Reports.Common.Abstractions.Mediator;
using Reports.Common.Exceptions;
using Reports.Domain.Entities;
using Reports.Service.ApprovalService;

namespace Reports.Features.Approval.Commands.RejectReport
{
    public class RejectReportCommand : ICommand
    {
        public required int ReportId { get; set; }
    }

    public class RejectReportCommandHandler(
        AppDbContext _context,
        IReportApprovalService _reportApprovalService,
        ICurrentUserService _currentUserService
    ) : ICommandHandler<RejectReportCommand>
    {
        public async Task Handle(RejectReportCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(_currentUserService.UserId)
                ?? throw new NotFoundException(nameof(User), _currentUserService.UserId);

            // Get report
            var report = await _context.Reports
                .Include(r => r.Approvals)
                .FirstOrDefaultAsync(r => r.Id == request.ReportId, cancellationToken)
                ?? throw new NotFoundException(nameof(Report), request.ReportId);

            // Check: has user already approved this report at this level → if so, can reject only if still in same level
            var alreadyApproved = report.Approvals.Any(a =>
                a.UserId == _currentUserService.UserId && a.IsApproved && a.Geha == user.Geha);

            if (alreadyApproved && report.CurrentApprovalLevel != user.Level)
            {
                throw new BadRequestException("You have already approved this report at this level; you can't reject after moving to higher level.");
            }

            // 👍 Everything is valid → reject
            await _reportApprovalService.RejectReportAsync(request.ReportId, _currentUserService.UserId);
        }
    }

    public class RejectReportCommandValidator : AbstractValidator<RejectReportCommand>
    {
        public RejectReportCommandValidator(
            AppDbContext _context,
            ICurrentUserService _currentUserService
        )
        {
            RuleFor(x => x.ReportId)
                .GreaterThan(0).WithMessage("Report ID must be greater than 0.");

            // Check if report exists
            RuleFor(x => x.ReportId)
                .MustAsync(async (reportId, cancellation) =>
                {
                    return await _context.Reports.AnyAsync(r => r.Id == reportId, cancellation);
                })
                .WithMessage("Report not found.");

            // Check if RA already approved → cannot reject anymore
            RuleFor(x => x.ReportId)
                .MustAsync(async (reportId, cancellation) =>
                {
                    var report = await _context.Reports
                        .FirstOrDefaultAsync(r => r.Id == reportId, cancellation);
                    return report != null && !report.IsApprovedByRA;
                })
                .WithMessage("You cannot reject this report; it is already approved by RA.");

            // Check if user's level matches report current approval level
            RuleFor(x => x.ReportId)
                .MustAsync(async (reportId, cancellation) =>
                {
                    var report = await _context.Reports
                        .FirstOrDefaultAsync(r => r.Id == reportId, cancellation)
                        ?? throw new NotFoundException(nameof(Report), reportId);

                    return _currentUserService.Role != null &&
                           _currentUserService.Role.Contains(report.CurrentApprovalLevel.ToString());
                })
                .WithMessage("You are not allowed to reject this report based on your level.");
        }
    }


}
