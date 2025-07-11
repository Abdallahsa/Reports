using FluentValidation;
using Reports.Api.Data;
using Reports.Api.Services.CurrentUser;
using Reports.Common.Abstractions.Mediator;
using Reports.Common.Exceptions;
using Reports.Domain.Entities;
using Reports.Service.LoggingService;
using Reports.Service.SaveReport;


namespace Reports.Features.Reportss.Commands.LockReport
{
    public class LockReportCommand : ICommand<string>
    {
        public int ReportId { get; set; }
    }

    public class LockReportCommandHandler(
        AppDbContext _context,
        ITemplateReportService _templateReportService,
        ICurrentUserService _currentUserService,
        ILoggingService _loggingService
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
                await _loggingService.LogInformation("Report with ID {ReportId} locked by user {UserId}", request.ReportId, _currentUserService.UserId);

                return "File locked successfully.";
            }
            catch (Exception ex)
            {
                // Log the error
                await _loggingService.LogError(
                                       "Error locking report with ID {ReportId}: {Message}", ex, request.ReportId, ex.Message);

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
