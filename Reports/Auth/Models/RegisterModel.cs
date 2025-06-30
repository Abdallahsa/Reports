using Reports.Api.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace Reports.Api.Auth.Models
{
    public class RegisterModel
    {

        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public required string Password { get; set; }

        [Required]

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;

        public Level Level { get; set; } 

        public required Geha Geha { get; set; }

        public IFormFile ? Signature { get; set; } 

    }

}
