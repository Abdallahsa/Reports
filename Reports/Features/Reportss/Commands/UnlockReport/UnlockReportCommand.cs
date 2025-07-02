using FluentValidation;
using Reports.Api.Data;
using Reports.Common.Abstractions.Mediator;
using Reports.Common.Exceptions;
using Reports.Domain.Entities;
using Reports.Service.SaveReport;

namespace Reports.Features.Reportss.Commands.UnlockReport
{
    public class UnlockReportCommand : ICommand<string>
    {
        public int ReportId { get; set; }
    }

    public class UnlockReportCommandHandler(
       AppDbContext _context,
       ITemplateReportService _templateReportService
       ) : ICommandHandler<UnlockReportCommand, string>
    {
        public async Task<string> Handle(UnlockReportCommand request, CancellationToken cancellationToken)
        {
            var report = await _context.Reports.FindAsync(request.ReportId)
                ?? throw new NotFoundException(nameof(Report), request.ReportId);

            if (report.Status == FileStatus.Unlocked)
                throw new InvalidOperationException("⚠️ الملف بالفعل مفتوح!");

            _templateReportService.DecryptFileInPlace(report.FilePath);

            report.Status = FileStatus.Unlocked;
            await _context.SaveChangesAsync(cancellationToken);

            return "✅ تم فك التشفير والملف أصبح مفتوح.";
        }
    }

    public class UnlockReportCommandValidator : AbstractValidator<UnlockReportCommand>
    {
        public UnlockReportCommandValidator()
        {
            RuleFor(x => x.ReportId)
                .GreaterThan(0).WithMessage("ReportId يجب أن يكون رقم صحيح أكبر من صفر");
        }
    }


}
