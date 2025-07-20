using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using DocumentFormat.OpenXml.Office.Drawing;
using DocumentFormat.OpenXml.Packaging;
using Microsoft.EntityFrameworkCore;
using Reports.Api.Data;
using Reports.Api.Domain.Entities;
using Reports.Api.Services.Notifications;
using Reports.Common.Exceptions;
using Reports.Domain.Entities;
using Reports.Service.GehaService;
using Reports.Service.LoggingService;

namespace Reports.Service.ApprovalService
{
    public class ReportApprovalService(
        AppDbContext _context,
        IUserGehaService _userGehaService,
        INotificationService _notificationService,
        ILoggingService _loggingService
    ) : IReportApprovalService
    {
        public async Task ApproveReportAsync(int reportId, int userId)
        {
            try
            {
                var report = await _context.Reports
                    .Include(r => r.Approvals)
                    .FirstOrDefaultAsync(r => r.Id == reportId)
                    ?? throw new NotFoundException(nameof(Report), reportId);

                var user = await _context.Users.FindAsync(userId)
                    ?? throw new NotFoundException(nameof(User), userId);

                // تسجيل الموافقة
                report.Approvals.Add(new ReportApproval
                {
                    ReportId = report.Id,
                    UserId = userId,
                    Geha = user.Geha,
                    ApprovalStatus = ApprovalStatus.Approved,
                    ApprovalDate = DateTime.UtcNow
                });

                if (user.Level == Level.LevelFour)
                    report.IsApprovedByRA = true;

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
                    report.IsRejected = false;
                }

                // ✅ استبدال التوقيع بناءً على AltText
                var signaturePath = System.IO.Path.Combine("wwwroot", "StaticFiles", "Signatures", $"{userId}.png");
                if (File.Exists(signaturePath))
                {
                    try
                    {
                        ReplaceImageByAltText(report.FilePath, user.Geha, signaturePath);
                    }
                    catch (Exception e)
                    {
                        await _loggingService.LogError("Failed to replace image for report {ReportId}: {Message}", e, new
                        {
                            ReportId = report.Id,
                            Message = e.Message
                        });
                    }
                }

                // 🔔 إشعارات للمشاركين الحاليين
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

                // 🔔 إشعارات للمستوى الجديد
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

                // 📝 تسجيل العملية
                await _loggingService.LogInformation("Report {ReportId} approved by user {UserId} at level {Level}", new
                {
                    ReportId = reportId,
                    UserId = userId,
                    Level = user.Level
                });

                _context.Reports.Update(report);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                await _loggingService.LogError("Error approving report {ReportId} by user {UserId}: {Message}", ex, new
                {
                    ReportId = reportId,
                    UserId = userId,
                    Message = ex.Message
                });
                throw new BadRequestException(ex.Message);
            }
        }

        public async Task RejectReportAsync(int reportId, int userId)
        {
            try
            {
                var report = await _context.Reports
                    .Include(r => r.Approvals)
                    .FirstOrDefaultAsync(r => r.Id == reportId)
                    ?? throw new NotFoundException(nameof(Report), reportId);

                var user = await _context.Users.FindAsync(userId)
                    ?? throw new NotFoundException(nameof(User), userId);

                // Cancel approvals at current level
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

                // Log the rejection action
                await _loggingService.LogInformation("Report {ReportId} rejected by user {UserId} at level {Level}",
                    new
                    {
                        ReportId = reportId,
                        UserId = userId,
                        Level = user.Level
                    });
            }
            catch (Exception ex)
            {
                await _loggingService.LogError("Error rejecting report {ReportId} by user {UserId}: {Message}", ex,
                    new
                    {
                        ReportId = reportId,
                        UserId = userId,
                        Message = ex.Message
                    });
                throw new BadRequestException(ex.Message);
            }
        }

        private Level GetNextLevel(Level current)
        {
            return current switch
            {
                Level.LevelZero => Level.LevelOne,
                Level.LevelOne => Level.LevelTwo,
                Level.LevelTwo => Level.LevelThree,
                Level.LevelThree => Level.LevelFour,
                Level.LevelFour => Level.LevelFour, // Highest level
                _ => current
            };
        }

        private void ReplaceImageByAltText(string docxPath, string altText, string signaturePath)
        {
            using var wordDoc = WordprocessingDocument.Open(docxPath, true);
            var mainPart = wordDoc.MainDocumentPart;
            if (mainPart?.Document?.Body == null)
                throw new InvalidOperationException("Document structure is invalid.");

            var drawings = mainPart.Document.Body.Descendants<Drawing>().ToList();
            foreach (var drawing in drawings)
            {
                var docPr = drawing.Descendants<DocProperties>().FirstOrDefault();
                if (docPr == null || string.IsNullOrWhiteSpace(docPr.Description) || docPr.Description != altText)
                    continue;

                var blip = drawing.Descendants<Blip>().FirstOrDefault();
                if (blip?.Embed == null)
                    continue;

                var oldPart = mainPart.GetPartById(blip.Embed);
                if (oldPart != null)
                    mainPart.DeletePart(oldPart);

                var newImagePart = mainPart.AddImagePart(ImagePartType.Png);
                using var fs = File.OpenRead(signaturePath);
                newImagePart.FeedData(fs);

                blip.Embed = mainPart.GetIdOfPart(newImagePart);
                break; // remove this if you want to replace all matching images
            }

            mainPart.Document.Save();
        }

    }
}