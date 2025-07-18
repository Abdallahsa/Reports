using Newtonsoft.Json;
using Reports.Api.Data;
using Reports.Domain.Entities;
using Reports.Service.LoggingService;
using System.Text.RegularExpressions;

public class LoggingService : ILoggingService
{
    private readonly AppDbContext _context;

    public LoggingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task LogInformation(string messageTemplate, object? properties = null)
    {
        await Log("Information", messageTemplate, null, properties);
    }

    public async Task LogInformation(string messageTemplate, params object[] propertyValues)
    {
        var properties = BuildPropertiesFromTemplate(messageTemplate, propertyValues);
        await Log("Information", messageTemplate, null, properties);
    }

    public async Task LogWarning(string messageTemplate, object? properties = null)
    {
        await Log("Warning", messageTemplate, null, properties);
    }

    public async Task LogWarning(string messageTemplate, params object[] propertyValues)
    {
        var properties = BuildPropertiesFromTemplate(messageTemplate, propertyValues);
        await Log("Warning", messageTemplate, null, properties);
    }

    public async Task LogError(string messageTemplate, Exception? exception = null, object? properties = null)
    {
        await Log("Error", messageTemplate, exception, properties);
    }

    public async Task LogError(string messageTemplate, Exception? exception = null, params object[] propertyValues)
    {
        var properties = BuildPropertiesFromTemplate(messageTemplate, propertyValues);
        await Log("Error", messageTemplate, exception, properties);
    }

    private async Task Log(string level, string messageTemplate, Exception? exception, object? properties)
    {
        string renderedMessage = RenderMessage(messageTemplate, properties);
        string serializedProperties = properties != null ? JsonConvert.SerializeObject(properties) : string.Empty;

        var log = new SystemLog
        {
            MessageTemplate = messageTemplate,
            Message = renderedMessage,
            Level = level,
            TimeStamp = DateTime.UtcNow,
            Exception = exception?.ToString() ?? string.Empty,
            Properties = serializedProperties
        };

        _context.SystemLog.Add(log);
        await _context.SaveChangesAsync();
    }

    private string RenderMessage(string template, object? properties)
    {
        if (properties == null) return template;

        string message = template;

        if (properties is Dictionary<string, object> dict)
        {
            foreach (var pair in dict)
            {
                message = message.Replace($"{{{pair.Key}}}", pair.Value?.ToString() ?? "");
            }
        }
        else
        {
            foreach (var prop in properties.GetType().GetProperties())
            {
                var value = prop.GetValue(properties)?.ToString() ?? "";
                message = message.Replace($"{{{prop.Name}}}", value);
            }
        }

        return message;
    }

    private object BuildPropertiesFromTemplate(string template, object[] values)
    {
        var regex = new Regex(@"\{(\w+)\}");
        var matches = regex.Matches(template);
        var props = new Dictionary<string, object>();

        for (int i = 0; i < matches.Count && i < values.Length; i++)
        {
            string propName = matches[i].Groups[1].Value;
            props[propName] = values[i];
        }

        return props;
    }
}
