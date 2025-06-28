namespace Reports.Common.Exceptions
{
    public class ServiceUnavailableException : Exception
    {
        public ServiceUnavailableException() : base("External service is not available.") { }

        public ServiceUnavailableException(string message) : base(message) { }
    }
}
