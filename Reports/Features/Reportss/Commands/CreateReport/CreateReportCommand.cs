using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reports.Api.Data;
using Reports.Api.Domain.Entities;
using Reports.Api.Services.CurrentUser;
using Reports.Api.Services.Notifications;
using Reports.Common.Abstractions.Mediator;
using Reports.Common.Exceptions;
using Reports.Domain.Entities;
using Reports.Service.GehaService;
using Reports.Service.ReportService;
using Reports.Service.SaveReport;

namespace Reports.Features.Reportss.Commands.CreateReport
{
    public class CreateReportCommand : ICommand<string>
    {
        public required ReportType ReportType { get; set; }
    }

    public class CreateReportCommandHandler(
        ICurrentUserService _currentUserService,
        AppDbContext _context,
        IUserGehaService _userGehaService,
        INotificationService _notificationService,
        ITemplateReportService _templateReportService
    ) : ICommandHandler<CreateReportCommand, string>
    {
        public async Task<string> Handle(CreateReportCommand request, CancellationToken cancellationToken)
        {
            try
            {

                var user = await _context.Set<User>().FindAsync(_currentUserService.UserId)
                    ?? throw new NotFoundException(nameof(User), _currentUserService.UserId);


                // Check if report of this type is already created today
                var today = DateTime.UtcNow.Date;
                var exists = await _context.Reports.AnyAsync(r =>
                    r.ReportType == request.ReportType && r.CreatedAt.Date == today, cancellationToken);
                if (exists)
                    throw new BadRequestException("Report of this type has already been created today");

                // Build file name and copy template
                var templateFileName = $"{request.ReportType}.docx";
                var newFileName = _templateReportService.CopyTemplateAndSave(
                    templateFileName: templateFileName,
                    reportType: request.ReportType.ToString(),
                    gehaCode: user.Geha.ToString()
                );

                var gehaEnum = Enum.TryParse<Geha>(user.Geha, out var parsedGeha) ? parsedGeha : Geha.None;

                var report = new Report
                {
                    GehaCode = user.Geha,
                    ReportType = request.ReportType,
                    ShoabaName = gehaEnum.ToArabic(),
                    Description = request.ReportType.ToArabic(),
                    FilePath = newFileName,
                    Status = FileStatus.Locked,
                    CurrentApprovalLevel = user.Level,
                    IsApprovedByRA = false,
                    IsRejected = false,

                };
                var requiredGehas = _userGehaService.GetAllowedGehaByLevel(report.CurrentApprovalLevel)
                    .Select(g => g.ToString())
                    .ToList();

                var targetUsers = await _context.Users
                    .Where(u => u.Level == report.CurrentApprovalLevel && requiredGehas.Contains(u.Geha))
                    .ToListAsync(cancellationToken);

                var title = "تقرير جديد بحاجة لموافقتك";
                var content = $"تقرير رقم {report.Id} تم إنشاؤه اليوم وهو بحاجة إلى موافقتك في المستوى {report.CurrentApprovalLevel}.";

                foreach (var targetUser in targetUsers)
                {
                    await _notificationService.SendNotificationAsync(
                        title,
                        content,
                        targetUser.Id,
                        NotificationType.Info,
                        senderId: _currentUserService.UserId,
                        cancellationToken
                    );
                }

                await _context.Reports.AddAsync(report, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                return newFileName;
            }
            catch (Exception ex)
            {
                throw new BadRequestException(ex.Message);
            }
        }
    }

    public class CreateReportCommandValidator : AbstractValidator<CreateReportCommand>
    {
        public CreateReportCommandValidator(IUserReportService _userReportService)
        {
            RuleFor(x => x.ReportType)
                .IsInEnum()
                .WithMessage("Invalid report type");

            // check if user level is allow create this type of report using service IUserReportService
            RuleFor(x => x)
                .Must((command, cancellationToken) =>
                {
                    var allowedReports = _userReportService.GetAllowedReportsForCurrentUser();
                    return allowedReports.Contains(command.ReportType);
                })
                .WithMessage("You are not authorized to create this type of report");


        }
    }

}
