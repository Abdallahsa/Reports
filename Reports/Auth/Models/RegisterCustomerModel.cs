using Reports.Api.Auth.Models;
using System.ComponentModel.DataAnnotations;

namespace Reports.Api.Auth.Models
{
    public class RegisterUerModel : RegisterModel
    {

        [Required]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public required string ConfirmPassword { get; set; }

    }
}
