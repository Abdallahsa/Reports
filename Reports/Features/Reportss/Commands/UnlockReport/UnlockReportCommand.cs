using FluentValidation;
using Reports.Api.Data;
using Reports.Api.Services.CurrentUser;
using Reports.Common.Abstractions.Mediator;
using Reports.Common.Exceptions;
using Reports.Domain.Entities;
using Reports.Service.LoggingService;
using Reports.Service.SaveReport;


namespace Reports.Features.Reportss.Commands.UnlockReport
{
    public class UnlockReportCommand : ICommand<string>
    {
        public int ReportId { get; set; }
    }

    public class UnlockReportCommandHandler(
       AppDbContext _context,
       ITemplateReportService _templateReportService,
       ICurrentUserService _currentUserService,
       ILoggingService _loggingService
       ) : ICommandHandler<UnlockReportCommand, string>
    {
        public async Task<string> Handle(UnlockReportCommand request, CancellationToken cancellationToken)
        {

            try
            {
                var report = await _context.Reports.FindAsync(request.ReportId)
                             ?? throw new NotFoundException(nameof(Report), request.ReportId);

                if (report.Status == FileStatus.Unlocked)
                    throw new InvalidOperationException("the file is already unlocked. No action needed.");

                _templateReportService.DecryptFileInPlace(report.FilePath);

                report.Status = FileStatus.Unlocked;
                await _context.SaveChangesAsync(cancellationToken);

                // Log the unlock action
                await _loggingService.LogInformation("Report with ID {ReportId} unlocked by user {UserId}", request.ReportId, _currentUserService.UserId);

                return "File unlocked successfully.";
            }
            catch (Exception ex)
            {
                // Log the error
                await _loggingService.LogError(
                    "Error unlocking report with ID {ReportId}: {Message}",
                    ex,
                    request.ReportId,
                    ex.Message
                );



                throw new BadRequestException(ex.Message);
            }

        }
    }

    public class UnlockReportCommandValidator : AbstractValidator<UnlockReportCommand>
    {
        public UnlockReportCommandValidator()
        {
            RuleFor(x => x.ReportId)
                .GreaterThan(0).WithMessage("Report ID must be greater than zero.");
        }
    }


}
