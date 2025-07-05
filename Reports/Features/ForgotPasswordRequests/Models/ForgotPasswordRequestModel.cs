namespace Reports.Features.ForgotPasswordRequests.Models
{
    public class ForgotPasswordRequestModel
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsUsed { get; set; }
        public string Phone { get; set; } = string.Empty;
    }
}
