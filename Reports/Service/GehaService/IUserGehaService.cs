using Reports.Api.Domain.Entities;

namespace Reports.Service.GehaService
{
    public interface IUserGehaService
    {
        List<Geha> GetAllowedGehaByLevel(Level level);
        bool IsUserAllowedToApproveReport(string userLevel, Level currentApprovalLevel);

    }
}
