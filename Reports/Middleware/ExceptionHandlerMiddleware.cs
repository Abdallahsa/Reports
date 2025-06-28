using Reports.Common.Exceptions;
using System.Net;
using System.Text.Json;
namespace Reports.Api.Middleware
{
    public class ExceptionHandlerMiddleware
    {
        private readonly RequestDelegate next;
        private readonly ILogger<ExceptionHandlerMiddleware> logger;

        public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
        {
            this.next = next;
            this.logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await next(httpContext);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, ex.Message, httpContext.Request, "");
                await HandleExceptionAsync(httpContext, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
        {
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError;
            object problem;
            switch (ex)
            {
                case BadRequestException badRequestException:
                    statusCode = HttpStatusCode.BadRequest;
                    problem = new
                    {
                        Title = badRequestException.Message,
                        Status = (int)statusCode,
                        Detail = badRequestException.InnerException?.Message,
                        Type = nameof(BadRequestException),
                        Errors = badRequestException.Errors,
                    };
                    break;

                case NotFoundException notFound:
                    statusCode = HttpStatusCode.NotFound;
                    problem = new
                    {
                        Title = notFound.Message,
                        Status = (int)statusCode,
                        Type = nameof(NotFoundException),
                    };
                    break;

                case IntegrationBadResponseException integrationFailure:
                    statusCode = HttpStatusCode.BadGateway;
                    problem = new
                    {
                        Title = integrationFailure.Message,
                        statusCode = (int)statusCode,
                        Type = nameof(IntegrationBadResponseException),
                    };
                    break;

                case ServiceUnavailableException serviceUnavailable:
                    statusCode = HttpStatusCode.ServiceUnavailable;
                    problem = new
                    {
                        Title = serviceUnavailable.Message,
                        statusCode = (int)statusCode,
                        Type = nameof(ServiceUnavailableException),
                    };
                    break;

                case UnauthorizedAccessException unauthorized:
                    statusCode = HttpStatusCode.Unauthorized;
                    problem = new
                    {
                        Title = unauthorized.Message,
                        Status = (int)statusCode,
                        Type = nameof(HttpStatusCode.Unauthorized),
                    };
                    break;

                case ArgumentException argumentException:
                    statusCode = HttpStatusCode.BadRequest;
                    problem = new
                    {
                        Title = argumentException.Message,
                        Status = (int)statusCode,
                        Type = nameof(HttpStatusCode.BadRequest),
                    };
                    break;

                default:
                    problem = new
                    {
                        Title = ex.Message,
                        Status = (int)statusCode,
                        Type = nameof(HttpStatusCode.InternalServerError),
                        Detail = ex.StackTrace,
                    };
                    break;
            }
            httpContext.Response.StatusCode = (int)statusCode;
            var logMessage = JsonSerializer.Serialize(problem);
            await httpContext.Response.WriteAsJsonAsync(problem);
        }
    }
}
