using OrderService.API.Errors;
using OrderService.Application.Exceptions;
using System.Net;
using System.Text.Json;

namespace OrderService.API.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _hostEnvironment;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger, IHostEnvironment hostEnvironment)
        {
            _next = next;
            _logger = logger;
            _hostEnvironment = hostEnvironment;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,ex.Message);
                context.Response.ContentType = "application/json";
                var statusCode = (int)HttpStatusCode.InternalServerError;
                var result = string.Empty;

                switch (ex)
                {
                    case NotFoundException notFoundException:
                        statusCode = (int)HttpStatusCode.NotFound;
                        break;
                    case ValidationException validationException:
                        statusCode = (int)HttpStatusCode.BadRequest;
                        var validationJson = JsonSerializer.Serialize(validationException.Errors);
                        result = JsonSerializer.Serialize(new CodeErrorException(statusCode,ex.Message,validationJson));
                        break;
                    //case BadRequestException badRequestException:
                    //    statusCode = (int)HttpStatusCode.BadRequest;
                    //    break;
                    default:
                        break;
                }

                if(string.IsNullOrEmpty(result)) result = JsonSerializer.Serialize(new CodeErrorException(statusCode,ex.Message,ex.StackTrace));

                context.Response.StatusCode = statusCode;

                await context.Response.WriteAsync(result);
            }
        }
    }
}
