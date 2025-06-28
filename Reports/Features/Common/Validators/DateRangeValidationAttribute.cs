using System.ComponentModel.DataAnnotations;

namespace Reports.Api.Features.Common.Validators
{
    public class DateRangeValidationAttribute : ValidationAttribute
    {
        private readonly string _startDatePropertyName;

        public DateRangeValidationAttribute(string startDatePropertyName)
        {
            _startDatePropertyName = startDatePropertyName;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var endDate = value as DateTime?;

            // Get the start date property value
            var startDateProperty = validationContext.ObjectType.GetProperty(_startDatePropertyName);
            if (startDateProperty == null)
            {
                return new ValidationResult($"Unknown property: {_startDatePropertyName}");
            }

            var startDate = startDateProperty.GetValue(validationContext.ObjectInstance) as DateTime?;

            // Validate if both dates have values and ensure end date is greater than or equal to start date
            if (startDate.HasValue && endDate.HasValue && endDate < startDate)
            {
                return new ValidationResult(ErrorMessage ?? "End date must be greater than or equal to the start date.");
            }

            return ValidationResult.Success!;
        }
    }

}