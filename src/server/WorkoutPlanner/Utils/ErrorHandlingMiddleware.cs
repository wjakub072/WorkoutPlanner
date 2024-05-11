using FluentValidation;
using Microsoft.Extensions.Hosting.Internal;
using WorkoutPlanner.Exceptions;

namespace WorkoutPlanner.Api.Utils;

public class ErrorHandlingMiddleware : IMiddleware
{
    private readonly ILogger<ErrorHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment  _enviroment;

    public ErrorHandlingMiddleware(ILogger<ErrorHandlingMiddleware> logger, IWebHostEnvironment  enviroment)
    {
        _logger = logger;
        _enviroment = enviroment;
    }
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next.Invoke(context);
        }
        // Add hard typed excpetions catches to error handling
        catch (ValidationException validationException)
        {
            _logger.LogError(validationException, validationException.Message);

            context.Response.StatusCode = 500;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync(validationException.Message);
        }
        catch (EndUserException exception)
        {
            _logger.LogError(exception, exception.Message);

            context.Response.StatusCode = 500;
            context.Response.ContentType = "text/plain";
            await context.Response.WriteAsync(exception.Message);
        }
        catch (Exception exception)
        {
            _logger.LogError(exception, exception.Message);

            context.Response.StatusCode = 500;
            context.Response.ContentType = "text/plain";

            if(_enviroment.IsDevelopment()){
                await context.Response.WriteAsync(exception.Message);
            } else {
                await context.Response.WriteAsync("Something went wrong.");
            }
        }
    }
}
