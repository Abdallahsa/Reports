namespace Reports.Api.Features.Common.Models
{
    public class UserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? SignaturePath { get; set; }
        public required string Geha { get; set; }
        public required string Level { get; set; }
    }
}
