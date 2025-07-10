namespace Reports.Features.Auth.Models
{
    public class GetUsersStatisticsModel
    {
        public int TotalUsers { get; set; }
        public int ActiveUsers { get; set; }
        public int InactiveUsers { get; set; }
        public int TotalAdmins { get; set; }
        public int TotalLevelZeroUsers { get; set; }
        public int TotalLevelOneUsers { get; set; }
        public int TotalLevelTwoUsers { get; set; }
        public int TotalLevelThreeUsers { get; set; }
        public int TotalLevelFourUsers { get; set; }
    }
}
