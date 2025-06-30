using Microsoft.AspNetCore.Http.Metadata;
using Reports.Api.Domain.Entities;

namespace Reports.Api.Features.Auth.Models
{
    public class RegisterCustomerModel
    {
        public required string FirstName { get; set; } = string.Empty;
        public required string LastName { get; set; } = string.Empty;
        public required string Email { get; set; } = string.Empty;
        public required string Password { get; set; } = string.Empty;
        public required string ConfirmPassword { get; set; } = string.Empty;
        public bool IsConfirmed { get; set; } = false;
        public required Geha Geha { get; set; }
        public required Level Level { get; set; }

        public IFormFile? Signature { get; set; }


    }
}
