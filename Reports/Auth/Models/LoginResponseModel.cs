namespace Reports.Api.Auth.Models
{
    public class LoginResponseModel
    {
        public string Token { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public int TokenExpiryInMinutes { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public bool IsConfirmed { get; set; }
        public ICollection<string>? Roles { get; set; }
        public string UserType { get; set; } = string.Empty;
        public int UserId { get; set; }

    }
}
