namespace Reports.Api.Features.Common.Models
{
    public class StuffUserDto
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? UserType { get; set; }
        public bool IsConfirmed { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? CreatedBy { get; set; }
        public int BranchId { get; set; }
        public string? BranchName { get; set; }
        public bool IsDeleted { get; set; }
    }
}
