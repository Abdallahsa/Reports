using FluentValidation.Results;

namespace Reports.Common.Exceptions
{
    public class BadRequestException : Exception
    {
        public IDictionary<string, string[]> Errors { get; }
        public BadRequestException() : base("One or more validation failures have occurred.!!!")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public BadRequestException(string message) : base(message)
        {
            Errors = new Dictionary<string, string[]>();
        }

        public BadRequestException(IEnumerable<ValidationFailure> failures) : this()
        {
            Errors = failures
                .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                .ToDictionary(failureGroup => failureGroup.Key, failureGroup => failureGroup.ToArray());
        }
    }
}
