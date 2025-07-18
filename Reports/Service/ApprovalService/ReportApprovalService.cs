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

                // Send notification to new level users
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

                // Log the approval action
                await _loggingService.LogInformation("Report {ReportId} approved by user {UserId} at level {Level}",
                    new
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
                await _loggingService.LogError("Error approving report {ReportId} by user {UserId}: {Message}", ex,
                    new
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

        ////private void InsertUserSignature(string docxPath, string gehaAltText, string userSignaturePath)
        //{
        //    using var wordDoc = WordprocessingDocument.Open(docxPath, true);

        //    var mainPart = wordDoc.MainDocumentPart;
        //    if (mainPart == null || mainPart.Document?.Body == null)
        //        throw new InvalidOperationException("Invalid Word document structure.");

        //    // Find drawings with the specified AltText (gehaAltText)
        //    var drawings = mainPart.Document.Body
        //        .Descendants<Wp.Drawing>()
        //        .ToList();

        //    foreach (var drawing in drawings)
        //    {
        //        var docPr = drawing.Inline?.DocProperties;
        //        if (docPr == null || docPr.Description?.Value != gehaAltText)
        //            continue;

        //        var blip = drawing.Descendants<A.Blip>().FirstOrDefault();
        //        if (blip?.Embed == null)
        //            continue;

        //        string oldRelId = blip.Embed.Value;
        //        var oldImagePart = mainPart.GetPartById(oldRelId);
        //        mainPart.DeletePart(oldImagePart);

        //        // Add the new signature image
        //        var newImagePart = mainPart.AddImagePart(ImagePartType.Png);
        //        using var imageStream = File.OpenRead(userSignaturePath);
        //        newImagePart.FeedData(imageStream);

        //        // Update the relationship ID in the blip
        //        blip.Embed.Value = mainPart.GetIdOfPart(newImagePart);

        //        break; // Stop after replacing the first matching image
        //    }

        //    mainPart.Document.Save();
        //}
    }
}