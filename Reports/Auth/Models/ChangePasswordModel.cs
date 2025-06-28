namespace Reports.Application.Auth.Models
{
    public class ChangePasswordModel
    {
        public required string CurrentPassword { get; set; }
        public required string NewPassword { get; set; }
    }

}
