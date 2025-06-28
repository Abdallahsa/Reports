using System.ComponentModel.DataAnnotations;

namespace Reports.Api.Features.Common.Validators
{
    public class FutureDateAttribute(int days = 0) : ValidationAttribute
    {
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime deliveryDate)
            {
                if (deliveryDate > DateTime.Now.AddDays(days))
                {
                    return ValidationResult.Success!;
                }
                else
                {
                    var message = $"Delivery date must be in the future{(days > 0 ? " : after " + days + " days from now." : ".")}";
                    return new ValidationResult(message);
                }
            }
            return new ValidationResult("Invalid date format.");
        }
    }

}
