using FluentValidation;
using Reports.Api.Data;
using Reports.Api.Services.CurrentUser;
using Reports.Common.Abstractions.Mediator;
using Reports.Common.Exceptions;
using Reports.Domain.Entities;
using Reports.Service.SaveReport;
using Serilog;

namespace Reports.Features.Reportss.Commands.LockReport
{
    public class LockReportCommand : ICommand<string>
    {
        public int ReportId { get; set; }
    }

    public class LockReportCommandHandler(
        AppDbContext _context,
        ITemplateReportService _templateReportService,
        ICurrentUserService _currentUserService
        ) : ICommandHandler<LockReportCommand, string>
    {
        public async Task<string> Handle(LockReportCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var report = await _context.Reports.FindAsync(request.ReportId)
                ?? throw new NotFoundException(nameof(Report), request.ReportId);

                if (report.Status == FileStatus.Locked)
                    throw new InvalidOperationException(" the file is already locked. No action needed.");

                _templateReportService.EncryptFileInPlace(report.FilePath);

                report.Status = FileStatus.Locked;
                await _context.SaveChangesAsync(cancellationToken);

                // Log the lock action save user id
                Log.Information("Report {ReportId} locked successfully at {Time} by {user id}", report.Id, DateTime.UtcNow, _currentUserService.UserId);


                return "File locked successfully.";
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error unlocking report with ID {ReportId} by {User ID}", request.ReportId, _currentUserService.UserId);
                throw new BadRequestException(ex.Message);
            }
        }
    }

    public class LockReportCommandValidator : AbstractValidator<LockReportCommand>
    {
        public LockReportCommandValidator()
        {
            RuleFor(x => x.ReportId)
                .GreaterThan(0).WithMessage("ReportID is Required .");
        }
    }

}
