using Reports.Domain.Entities;

namespace Reports.Service.ReportService
{
    public interface IUserReportService
    {
        /// <summary>
        /// Get list of allowed report types for current user based on their level.
        /// </summary>
        /// <returns>List of ReportType enums</returns>
        List<ReportType> GetAllowedReportsForCurrentUser();
    }
}
