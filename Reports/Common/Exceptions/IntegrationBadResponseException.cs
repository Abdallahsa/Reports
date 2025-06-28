namespace Reports.Common.Exceptions
{
    public class IntegrationBadResponseException : Exception
    {
        public IntegrationBadResponseException() : base($"Bad Response was receive from integration service.") { }

        public IntegrationBadResponseException(string message) : base(message) { }
    }
}
