namespace Reports.Service.LoggingService
{
    public interface ILoggingService
    {
        Task LogInformation(string messageTemplate, object? properties = null);
        Task LogInformation(string messageTemplate, params object[] propertyValues);

        Task LogWarning(string messageTemplate, object? properties = null);
        Task LogWarning(string messageTemplate, params object[] propertyValues);

        Task LogError(string messageTemplate, Exception? exception = null, object? properties = null);
        Task LogError(string messageTemplate, Exception? exception = null, params object[] propertyValues);
    }


}
