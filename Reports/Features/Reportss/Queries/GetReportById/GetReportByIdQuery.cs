using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reports.Api.Data;
using Reports.Api.Domain.Entities;
using Reports.Api.Services.CurrentUser;
using Reports.Common.Abstractions.Mediator;
using Reports.Common.Exceptions;
using Reports.Domain.Entities;
using Reports.Features.Reportss.Model;
using Reports.Service.LoggingService;


namespace Reports.Features.Reportss.Queries.GetReportById
{
    public class GetReportByIdQuery : ICommand<GetReportByIdModel>
    {
        public required int Id { get; set; }
    }

    // Handler for GetReportByIdQuery
    public class GetReportByIdQueryHandler
        (
        AppDbContext _context,
        ICurrentUserService _currentUserService,
        ILoggingService _loggingService
        ) : ICommandHandler<GetReportByIdQuery, GetReportByIdModel>
    {


        public async Task<GetReportByIdModel> Handle(GetReportByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Id == _currentUserService.UserId, cancellationToken)
                    ?? throw new NotFoundException(nameof(User), _currentUserService.UserId);

                var query = _context.Reports
                    .Where(r => r.Id == request.Id);


                if (user.Level == Level.LevelZero)
                {
                    query = query.Where(r => r.ReportType == ReportType.DailyDeputyReport || r.ReportType == ReportType.DailyOperationsReport);
                }

                var report = await query
                    .Select(r => new GetReportByIdModel
                    {
                        Id = r.Id,
                        GehaCode = r.GehaCode,
                        ShoabaName = r.ShoabaName,
                        Description = r.Description,
                        ReportType = r.ReportType.ToArabic(),
                        CreatedAt = r.CreatedAt,
                        IsRejected = r.IsRejected,
                        CurrentApprovalLevel = r.CurrentApprovalLevel.ToString(),
                        IsApprovedByRA = r.IsApprovedByRA,
                        FilePath = r.FilePath,
                        Approvals = r.Approvals.Select(a => new GetReportApprovalModel
                        {
                            Geha = a.Geha,
                            ApprovalStatus = a.ApprovalStatus.ToString(),
                            ApprovalDate = a.ApprovalDate
                        }).ToList()
                    })
                    .FirstOrDefaultAsync(cancellationToken)
                    ?? throw new NotFoundException("Report not found");

                // log

                await _loggingService.LogInformation("Fetched report by Id {ReportId} for user {UserId}", request.Id, _currentUserService.UserId);


                return report;
            }
            catch (Exception ex)
            {
                await _loggingService.LogError("Error fetching report by ID {ReportId}: {Message}", ex, request.Id, ex.Message);
                throw new BadRequestException(ex.Message);
            }



        }
    }

    // Query Validator
    public class GetReportByIdQueryValidator : AbstractValidator<GetReportByIdQuery>
    {
        public GetReportByIdQueryValidator(
            AppDbContext _context,
            ICurrentUserService _currentUserService
            )
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Report ID must be greater than 0.");

            RuleFor(x => x.Id)
                .MustAsync(async (id, cancellationToken) =>
                {
                    return await _context.Reports.AnyAsync(r => r.Id == id, cancellationToken);
                })
                .WithMessage("Report with the specified ID does not exist.");


        }
    }


}
