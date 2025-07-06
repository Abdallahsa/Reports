using Microsoft.EntityFrameworkCore;
using Reports.Api.Data;
using Reports.Api.Domain.Entities;
using Reports.Api.Services.Notifications;
using Reports.Common.Exceptions;
using Reports.Domain.Entities;
using Reports.Service.GehaService;

namespace Reports.Service.ApprovalService
{
    public class ReportApprovalService(
        AppDbContext _context,
        IUserGehaService _userGehaService,
        INotificationService _notificationService
    ) : IReportApprovalService
    {
        public async Task ApproveReportAsync(int reportId, int userId)
        {
            var report = await _context.Reports
                .Include(r => r.Approvals)
                .FirstOrDefaultAsync(r => r.Id == reportId)
                ?? throw new NotFoundException(nameof(Report), reportId);

            var user = await _context.Users.FindAsync(userId)
                ?? throw new NotFoundException(nameof(User), userId);

            report.Approvals.Add(new ReportApproval
            {
                ReportId = report.Id,
                UserId = userId,
                Geha = user.Geha,
                ApprovalStatus = ApprovalStatus.Approved,
                ApprovalDate = DateTime.UtcNow
            });

            if (user.Level == Level.LevelFour)
            {
                report.IsApprovedByRA = true;
            }

            var requiredGehas = _userGehaService.GetAllowedGehaByLevel(report.CurrentApprovalLevel)
                .Select(g => g.ToString())
                .ToList();

            var approvedGehas = report.Approvals
                .Where(a => a.ApprovalStatus == ApprovalStatus.Approved)
                .Select(a => a.Geha)
                .Distinct()
                .ToList();

            if (requiredGehas.All(required => approvedGehas.Contains(required)))
            {
                report.CurrentApprovalLevel = GetNextLevel(report.CurrentApprovalLevel);
                report.IsRejected = false; // reset
            }

            // Send notification to all participants
            var participantUserIds = report.Approvals
                .Where(a => a.UserId != userId)
                .Select(a => a.UserId)
                .Distinct()
                .ToList();

            var title = "تنبيه موافقة تقرير";
            var content = $"تمت الموافقة على التقرير رقم {report.Id} وانتقل إلى المستوى {report.CurrentApprovalLevel}.";

            foreach (var participantId in participantUserIds)
            {
                await _notificationService.SendNotificationAsync(
                    title, content, participantId, NotificationType.Success, userId);
            }

            // ✅ Send notification to new level users
            var newLevelUsers = await _context.Users
                .Where(u => u.Level == report.CurrentApprovalLevel)
                .ToListAsync();

            var notifyTitle = "تقرير جديد منتظر الموافقة";
            var notifyContent = $"هناك تقرير رقم {report.Id} بحاجة إلى موافقتك في المستوى {report.CurrentApprovalLevel}.";

            foreach (var u in newLevelUsers)
            {
                await _notificationService.SendNotificationAsync(
                    notifyTitle, notifyContent, u.Id, NotificationType.Info, userId);
            }

            _context.Reports.Update(report);
            await _context.SaveChangesAsync();
        }

        public async Task RejectReportAsync(int reportId, int userId)
        {
            var report = await _context.Reports
                .Include(r => r.Approvals)
                .FirstOrDefaultAsync(r => r.Id == reportId)
                ?? throw new NotFoundException(nameof(Report), reportId);

            var user = await _context.Users.FindAsync(userId)
                ?? throw new NotFoundException(nameof(User), userId);

            // 👈 Cancel approvals at current level
            var approvalsAtCurrentLevel = report.Approvals
                .Where(a => a.ApprovalStatus == ApprovalStatus.Approved)
                .ToList();

            foreach (var approval in approvalsAtCurrentLevel)
            {
                approval.ApprovalStatus = ApprovalStatus.Cancelled;
            }

            // Add new rejection
            report.Approvals.Add(new ReportApproval
            {
                ReportId = report.Id,
                UserId = userId,
                Geha = user.Geha,
                ApprovalStatus = ApprovalStatus.Rejected,
                ApprovalDate = DateTime.UtcNow
            });

            if (user.Level == Level.LevelFour)
            {
                report.IsApprovedByRA = false;
            }

            report.CurrentApprovalLevel = Level.LevelZero;
            report.IsRejected = true;

            // Send notification to all who approved
            var participantUserIds = report.Approvals
                .Where(a => a.UserId != userId)
                .Select(a => a.UserId)
                .Distinct()
                .ToList();

            var title = "تنبيه رفض تقرير";
            var content = $"تم رفض التقرير رقم {report.Id} من المستوى {user.Level} ({user.Geha}).";

            foreach (var participantId in participantUserIds)
            {
                await _notificationService.SendNotificationAsync(
                    title, content, participantId, NotificationType.Warning, userId);
            }

            _context.Reports.Update(report);
            await _context.SaveChangesAsync();
        }

        private Level GetNextLevel(Level current)
        {
            return current switch
            {
                Level.LevelZero => Level.LevelOne,
                Level.LevelOne => Level.LevelTwo,
                Level.LevelTwo => Level.LevelThree,
                Level.LevelThree => Level.LevelFour,
                Level.LevelFour => Level.LevelFour, // أعلى مستوى
                _ => current
            };
        }
    }
}
