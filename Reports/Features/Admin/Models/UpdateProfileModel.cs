namespace Reports.Features.Admin.Models
{
    public class UpdateProfileModel
    {
        public int UserId { get; set; }

        public string? Password { get; set; }
        public IFormFile? Signature { get; set; }
    }
}
