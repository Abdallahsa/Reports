namespace Reports.Api.Auth.Models
{
    public class UserInfoModel
    {
        public required string Email { get; set; }
        public List<string> Roles { get; set; } = new();
        public Dictionary<string, string> Claims { get; set; } = new();
    }

}
