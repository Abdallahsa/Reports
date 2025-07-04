using Microsoft.EntityFrameworkCore;
using Reports.Api.Data;
using Reports.Api.Domain.Entities;
using Reports.Common.Exceptions;
using Reports.Domain.Entities;
using Reports.Service.GehaService;

namespace Reports.Service.ApprovalService
{
    public class ReportApprovalService(
        AppDbContext _context,
        IUserGehaService _userGehaService
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

            // أضف الموافقة
            report.Approvals.Add(new ReportApproval
            {
                ReportId = report.Id,
                UserId = userId,
                Geha = user.Geha,
                IsApproved = true,
                ApprovalDate = DateTime.UtcNow
            });

            // 👈 لو المستخدم مستواه هو LevelFour (RA)
            if (user.Level == Level.LevelFour)
            {
                report.IsApprovedByRA = true;
            }

            // الجهات المطلوبة للمستوى الحالي
            var requiredGehas = _userGehaService.GetAllowedGehaByLevel(report.CurrentApprovalLevel)
                .Select(g => g.ToString())
                .ToList();

            // الجهات اللي وافقت بالفعل في المستوى الحالي
            var approvedGehas = report.Approvals
                .Where(a => a.IsApproved)
                .Select(a => a.Geha)
                .Distinct()
                .ToList();

            // لو كل الجهات المطلوبة وافقت → نطلع للمستوى اللي بعده
            if (requiredGehas.All(required => approvedGehas.Contains(required)))
            {
                report.CurrentApprovalLevel = GetNextLevel(report.CurrentApprovalLevel);
                report.IsRejected = false; // reset الرفض
            }

            _context.Reports.Update(report);

            await _context.SaveChangesAsync();
        }


        public async Task RejectReportAsync(int reportId, int userId)
        {
            var report = await _context.Reports
                .FirstOrDefaultAsync(r => r.Id == reportId)
                ?? throw new NotFoundException(nameof(Report), reportId);

            var user = await _context.Users.FindAsync(userId)
                ?? throw new NotFoundException(nameof(User), userId);

            // أضف الرفض
            report.Approvals.Add(new ReportApproval
            {
                ReportId = report.Id,
                UserId = userId,
                Geha = user.Geha,
                IsApproved = false,
                ApprovalDate = DateTime.UtcNow
            });

            // 👈 لو المستخدم مستواه هو LevelFour (RA)
            if (user.Level == Level.LevelFour)
            {
                report.IsApprovedByRA = false;
            }

            // رجّع لأول مستوى وعلّم انه مرفوض
            report.CurrentApprovalLevel = Level.LevelZero;
            report.IsRejected = true;

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
