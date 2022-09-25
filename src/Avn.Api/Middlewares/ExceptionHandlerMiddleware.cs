#nullable disable
using Microsoft.IdentityModel.Tokens;
using System.Net;
using System.Text.Json;

namespace Avn.Api.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;

    public ExceptionHandlerMiddleware(RequestDelegate next,
        IWebHostEnvironment env,
        ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _env = env;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    
    {
        string message = string.Empty;
        HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;
        ActionResponseStatusCode apiStatusCode = ActionResponseStatusCode.ServerError;

        try
        {
            await _next(context);
        }
        catch (SecurityTokenExpiredException exception)
        {
            _logger.LogError(exception, exception.Message);
            SetUnAuthorizeResponse(exception);
            await WriteToResponseAsync();
        }
        catch (UnauthorizedAccessException exception)
        {
            _logger.LogError(exception, exception.Message);
            SetUnAuthorizeResponse(exception);
            await WriteToResponseAsync();
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, exception.Message);

            if (_env.IsDevelopment())
            {
                Dictionary<string, string> dic = new()
                {
                    ["Exception"] = exception.Message,
                    ["StackTrace"] = exception.StackTrace,
                };
                message = JsonSerializer.Serialize(dic);
            }
            await WriteToResponseAsync();
        }

        async Task WriteToResponseAsync()
        {
            if (context.Response.HasStarted)
                throw new InvalidOperationException("The response has already started, the http status code middleware will not be executed.");

            ActionResponse result = new(apiStatusCode, message);
            var json = JsonSerializer.Serialize(result);

            context.Response.StatusCode = (int)httpStatusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(json);
        }
        void SetUnAuthorizeResponse(Exception exception)
        {
            httpStatusCode = HttpStatusCode.Unauthorized;
            apiStatusCode = ActionResponseStatusCode.UnAuthorized;

            if (_env.IsDevelopment())
            {
                Dictionary<string, string> dic = new()
                {
                    ["Exception"] = exception.Message,
                    ["StackTrace"] = exception.StackTrace
                };
                if (exception is SecurityTokenExpiredException tokenException)
                    dic.Add("Expires", tokenException.Expires.ToString());

                message = JsonSerializer.Serialize(dic);
            }
        }
    }
}
