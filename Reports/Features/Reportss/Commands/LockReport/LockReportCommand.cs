using FluentValidation;
using Reports.Api.Data;
using Reports.Common.Abstractions.Mediator;
using Reports.Common.Exceptions;
using Reports.Domain.Entities;
using Reports.Service.SaveReport;

namespace Reports.Features.Reportss.Commands.LockReport
{
    public class LockReportCommand : ICommand<string>
    {
        public int ReportId { get; set; }
    }

    public class LockReportCommandHandler(
        AppDbContext _context,
        ITemplateReportService _templateReportService
        ) : ICommandHandler<LockReportCommand, string>
    {
        public async Task<string> Handle(LockReportCommand request, CancellationToken cancellationToken)
        {
            var report = await _context.Reports.FindAsync(request.ReportId)
                ?? throw new NotFoundException(nameof(Report), request.ReportId);

            if (report.Status == FileStatus.Locked)
                throw new InvalidOperationException(" the file is already locked. No action needed.");

            _templateReportService.EncryptFileInPlace(report.FilePath);

            report.Status = FileStatus.Locked;
            await _context.SaveChangesAsync(cancellationToken);

            return "File locked successfully.";
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
