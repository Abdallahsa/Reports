using FluentValidation;
using Microsoft.EntityFrameworkCore;
using Reports.Api.Common.Abstractions.Collections;
using Reports.Api.Data;
using Reports.Api.Domain.Entities;
using Reports.Api.Services.CurrentUser;
using Reports.Common.Abstractions.Collections;
using Reports.Common.Abstractions.Mediator;
using Reports.Common.Exceptions;
using Reports.Features.Reportss.Model;
using Reports.Service.GehaService;

namespace Reports.Features.Approval.Queries.GetTodayPendingApprovalReports
{
    public class GetTodayPendingApprovalReportsQuery : HasTableView, ICommand<PagedList<GetAllReportModel>>
    {
    }

    // handler for the query

    public class GetTodayPendingApprovalReportsQueryHandler
        (
        AppDbContext _context,
            ICurrentUserService _currentUserService,
            IUserGehaService _userGehaService
        ) : ICommandHandler<GetTodayPendingApprovalReportsQuery, PagedList<GetAllReportModel>>
    {
        public async Task<PagedList<GetAllReportModel>> Handle(GetTodayPendingApprovalReportsQuery request, CancellationToken cancellationToken)
        {

            try
            {
                var user = await _context.Users.
                FindAsync(_currentUserService.UserId, cancellationToken)
                ?? throw new NotFoundException(nameof(User), _currentUserService.UserId);

                var userLevel = user.Level;
                var userGeha = user.Geha;

                // Get reports created on the requested date (or today if not provided)
                var targetDate = DateTime.UtcNow.Date;

                var query = _context.Reports
                    .Include(r => r.Approvals)
                    .Where(r => r.CreatedAt.Date == targetDate && r.CurrentApprovalLevel == userLevel)
                    .AsQueryable();


                //filter to return only reports that the user's geha has not yet approved
                var filtered = new List<GetAllReportModel>();

                var requiredGehas = _userGehaService.GetAllowedGehaByLevel(userLevel)
                    .Select(g => g.ToString())
                    .ToList();

                await foreach (var report in query.AsAsyncEnumerable().WithCancellation(cancellationToken))
                {
                    var approvedGehas = report.Approvals
                        .Where(a => a.IsApproved)
                        .Select(a => a.Geha)
                        .Distinct()
                        .ToList();

                    var missingGehas = requiredGehas.Except(approvedGehas);

                    if (missingGehas.Contains(userGeha))
                    {
                        filtered.Add(new GetAllReportModel
                        {
                            Id = report.Id,
                            Description = report.Description,
                            CreatedAt = report.CreatedAt,
                            ReportType = report.ReportType.ToString(),
                            CurrentApprovalLevel = report.CurrentApprovalLevel.ToString(),
                            GehaCode = report.GehaCode,
                            ShoabaName = report.ShoabaName,
                            FilePath = report.FilePath,
                            IsRejected = report.IsRejected,
                            IsApprovedByRA = report.IsApprovedByRA
                        });
                    }
                }


                return PagedList<GetAllReportModel>.Create(filtered.AsQueryable(), request.PageNumber, request.PageSize);
            }
            catch (Exception ex)
            {
                throw new NotFoundException("Reports not found", ex);
            }

        }
    }

    // Query Validator

    public class GetTodayPendingApprovalReportsQueryValidator : AbstractValidator<GetTodayPendingApprovalReportsQuery>
    {
        public GetTodayPendingApprovalReportsQueryValidator(ICurrentUserService _currentUserService)
        {
            RuleFor(_ => _)
                .Must(_ => _currentUserService.IsAuthenticated)
                .WithMessage("You must be logged in to get pending approvals.");
        }
    }

}
