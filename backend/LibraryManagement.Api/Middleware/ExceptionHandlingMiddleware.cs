namespace LibraryManagement.API.Middleware;

using Application.Common.Exceptions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

/// <summary>
/// Global exception handling middleware
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        var response = new { error = "", statusCode = 500, details = "" };

        switch (exception)
        {
            case ValidationException validationException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                var errors = validationException.Errors.SelectMany(x => x.Value).ToList();
                response = new
                {
                    error = "Validation failed",
                    statusCode = StatusCodes.Status400BadRequest,
                    details = string.Join(", ", errors)
                };
                break;

            case NotFoundException notFoundException:
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                response = new
                {
                    error = "Not found",
                    statusCode = StatusCodes.Status404NotFound,
                    details = notFoundException.Message
                };
                break;

            case ConflictException conflictException:
                context.Response.StatusCode = StatusCodes.Status409Conflict;
                response = new
                {
                    error = "Conflict",
                    statusCode = StatusCodes.Status409Conflict,
                    details = conflictException.Message
                };
                break;

            case ForbiddenException forbiddenException:
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                response = new
                {
                    error = "Forbidden",
                    statusCode = StatusCodes.Status403Forbidden,
                    details = forbiddenException.Message
                };
                break;

            case UnauthorizedException unauthorizedException:
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                response = new
                {
                    error = "Unauthorized",
                    statusCode = StatusCodes.Status401Unauthorized,
                    details = unauthorizedException.Message
                };
                break;

            default:
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                _logger.LogError(exception, "An unhandled exception occurred: {Message}", exception.Message);
                response = new
                {
                    error = "Internal server error",
                    statusCode = StatusCodes.Status500InternalServerError,
                    details = "An unexpected error occurred. Please try again later."
                };
                break;
        }

        return context.Response.WriteAsJsonAsync(response);
    }
}