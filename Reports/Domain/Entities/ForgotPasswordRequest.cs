namespace Reports.Domain.Entities
{
    public class ForgotPasswordRequest
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsUsed { get; set; } = false;
    }
}
