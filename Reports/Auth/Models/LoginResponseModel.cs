namespace Reports.Api.Auth.Models
{
    public class LoginResponseModel
    {
        public string Token { get; set; } = string.Empty;
        public string level { get; set; } = string.Empty;
        public int TokenExpiryInMinutes { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public bool IsConfirmed { get; set; }
        public ICollection<string>? Roles { get; set; }
        public int UserId { get; set; }

    }
}
