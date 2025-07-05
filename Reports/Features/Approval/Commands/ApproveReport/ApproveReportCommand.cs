using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reports.Api.Data;
using Reports.Api.Domain.Entities;
using Reports.Api.Services.CurrentUser;
using Reports.Common.Abstractions.Mediator;
using Reports.Common.Exceptions;
using Reports.Domain.Entities;
using Reports.Service.ApprovalService;
using Reports.Service.GehaService;

namespace Reports.Features.Approval.Commands.ApproveReport
{
    public class ApproveReportCommand : ICommand
    {
        public required int ReportId { get; set; }
    }

    public class ApproveReportCommandHandler(
       AppDbContext _context,
       IReportApprovalService _reportApprovalService,
       ICurrentUserService _currentUserService
   ) : ICommandHandler<ApproveReportCommand>
    {
        public async Task Handle(ApproveReportCommand request, CancellationToken cancellationToken)
        {


            var user = await _context.Users.FindAsync(_currentUserService.UserId)
                ?? throw new NotFoundException(nameof(User), _currentUserService.UserId);

            // Get report
            var report = await _context.Reports
                .Include(r => r.Approvals)
                .FirstOrDefaultAsync(r => r.Id == request.ReportId, cancellationToken)
                ?? throw new NotFoundException(nameof(Report), request.ReportId);


            // 🔒 Check: has user already approved before for this report & level?
            var alreadyApprovedInCurrentLevel = report.Approvals.Any(a =>
                    a.UserId == _currentUserService.UserId
                    && a.Geha == user.Geha
                    && a.ApprovalStatus == ApprovalStatus.Approved
                    && report.CurrentApprovalLevel.ToString() == user.Level.ToString());


            if (alreadyApprovedInCurrentLevel)
            {
                throw new BadRequestException("You have already approved this report at this level.");
            }

            // 👍 Everything is valid → approve
            await _reportApprovalService.ApproveReportAsync(request.ReportId, _currentUserService.UserId);
        }
    }

    // Validation 
    public class ApproveReportCommandValidator : AbstractValidator<ApproveReportCommand>
    {
        public ApproveReportCommandValidator(AppDbContext _context,
            ICurrentUserService _currentUserService,
            IUserGehaService _userGehaService
            )
        {
            RuleFor(x => x.ReportId)
                .GreaterThan(0).WithMessage("Report ID must be greater than 0.");

            //check if report exists
            RuleFor(x => x.ReportId)
                .MustAsync(async (reportId, cancellation) =>
                {
                    return await _context.Reports.AnyAsync(r => r.Id == reportId, cancellation);
                })
                .WithMessage("Report not found.");

            // 🔒 Check: is user allowed to approve this report if IsApprovedByRA is false can approval based on level if true is not allowed to approve the higher level is approved by RA
            RuleFor(x => x.ReportId)
                .MustAsync(async (reportId, cancellation) =>
                {
                    // Get the report
                    var report = await _context.Reports
                        .Include(r => r.Approvals)
                        .FirstOrDefaultAsync(r => r.Id == reportId, cancellation);

                    // return if IsApprovedByRA is true return false
                    if (report == null || report.IsApprovedByRA)
                        return false;
                    return true;
                })
                .WithMessage("You are not allowed to approve this report the RA is make Approved .");

            //check if user is allowed to approve this report based on his level
            RuleFor(x => x.ReportId)
                .MustAsync(async (reportId, cancellation) =>
                {
                    // Get the report
                    var report = await _context.Reports
                        .FirstOrDefaultAsync(r => r.Id == reportId, cancellation)
                        ?? throw new NotFoundException(nameof(Report), reportId);

                    if (_currentUserService.Role == null)
                        return false;
                    // Check if user is allowed to approve this report based on his level
                    return _currentUserService.Role.Contains(report.CurrentApprovalLevel.ToString());
                })
                .WithMessage("You are not allowed to approve this report based on your level.");

            // check if this geha is not make approved before
            RuleFor(x => x.ReportId)
                .MustAsync(async (reportId, cancellation) =>
                {
                    // Get the report
                    var report = await _context.Reports
                        .Include(r => r.Approvals)
                        .FirstOrDefaultAsync(r => r.Id == reportId, cancellation)
                        ?? throw new NotFoundException(nameof(Report), reportId);

                    // Check if this geha is not make approved before
                    return !report.Approvals.Any(a => a.Geha == _currentUserService.Geha && a.ApprovalStatus == ApprovalStatus.Approved && report.CurrentApprovalLevel == a.User?.Level);

                })
                .WithMessage("This geha has already approved this report.");

        }
    }


}
