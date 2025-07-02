using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reports.Api.Data;
using Reports.Api.Domain.Constants;
using Reports.Api.Domain.Entities;
using Reports.Api.Services;
using Reports.Api.Services.CurrentUser;
using Reports.Common.Abstractions.Mediator;
using Reports.Common.Exceptions;
using Reports.Domain.Entities;
using Reports.Service.SaveReport;

namespace Reports.Features.Reportss.Commands.CreateDailyDeputyReport
{
    public class CreateDailyDeputyReportCommand : ICommand<string>
    {

    }


    public class CreateDailyDeputyReportCommandHandler(
        ICurrentUserService _currentUserService,
            ITemplateReportService _templateReportService,
            AppDbContext _context,
            IStorageService _storageService
            ) : ICommandHandler<CreateDailyDeputyReportCommand, string>
    {
        public async Task<string> Handle(CreateDailyDeputyReportCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _context.Set<User>()
                    .FindAsync(_currentUserService.UserId)
                    ?? throw new NotFoundException(nameof(User), _currentUserService.UserId);

                var newFileName = _templateReportService.CopyTemplateAndSave(
                    templateFileName: "DailyDeputyReport.docx",
                    reportType: ReportType.DailyDeputyReport.ToString(),
                    gehaCode: user.Geha.ToString()
                );

                var gehaEnum = Enum.TryParse<Geha>(user.Geha, out var parsedGeha) ? parsedGeha : Geha.None;

                var report = new Report
                {
                    GehaCode = user.Geha.ToString(),
                    ReportType = ReportType.DailyDeputyReport,
                    ShoabaName = gehaEnum.ToArabic(),
                    Description = "تقرير المنوبين اليومى",
                    FilePath = newFileName,
                };

                await _context.Set<Report>().AddAsync(report);
                await _context.SaveChangesAsync();

                // فك التشفير في الذاكرة
                var decryptedBytes = _templateReportService.GetDecryptedFile(newFileName);

                // رجّع الملف كـ base64 string
                var base64String = Convert.ToBase64String(decryptedBytes);

                //return base64String;

                return _storageService.GetFullPath(newFileName, true);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

    }

    public class CreateDailyDeputyReportCommandValidator : AbstractValidator<CreateDailyDeputyReportCommand>
    {
        public CreateDailyDeputyReportCommandValidator(ICurrentUserService _currentUserService, AppDbContext context)
        {
            // check if user role is level zero 

            RuleFor(x => x).Must((x, cancellationToken) =>
            {
                return _currentUserService.Level == RoleConstants.LevelZero;
            }).WithMessage("You are not authorized to Create Daily Deputy Report ");

            // check if not report type of Daily Deputy Report create in this day 
            RuleFor(x => x)
               .MustAsync(async (command, cancellationToken) =>
               {
                   var today = DateTime.UtcNow.Date;

                   return !await context.Reports.AnyAsync(r =>
                       r.ReportType == ReportType.DailyDeputyReport &&
                       r.CreatedAt.Date == today,
                       cancellationToken);
               })
               .WithMessage("Daily Deputy Report has already been created today.");
        }
    }

}
