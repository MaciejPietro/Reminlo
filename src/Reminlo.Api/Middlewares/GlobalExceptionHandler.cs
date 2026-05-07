using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace Reminlo.Api.Middlewares;

internal sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        Log.Error(exception, "Unhandled exception occurred. Path: {Path}, Message: {Message}",
            httpContext.Request.Path, exception.Message);

        var (statusCode, errorCode, title, shouldExposeDetails) = GetExceptionDetails(exception);

        // Create error description - hide sensitive details in production
        var description = shouldExposeDetails
            ? exception.Message
            : "An error occurred while processing your request.";

        var problemDetails = new ProblemDetails
        {
            Type = exception.GetType().Name,
            Status = statusCode,
            Title = title,
            Instance = httpContext.Request.Path,
            Detail = description,
            Extensions =
            {
                // Add error code to unified array format
                ["errors"] = new Dictionary<string, string[]>
                {
                    { errorCode, [description] }
                }
            }
        };

        // In development, add stack trace
        if (ShouldIncludeStackTrace())
        {
            problemDetails.Extensions["stackTrace"] = exception.StackTrace;
            problemDetails.Extensions["exceptionType"] = exception.GetType().Name;
        }

        httpContext.Response.StatusCode = statusCode;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = problemDetails
        });
    }

    private static bool ShouldIncludeStackTrace()
    {
        // Only include stack trace in Development environment
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        return environment == "Development";
    }

    /// <summary>
    /// Maps exception types to HTTP status codes and error details.
    /// Add custom exception mappings here.
    /// </summary>
    private static (int StatusCode, string ErrorCode, string Title, bool ShouldExposeDetails) GetExceptionDetails(
        Exception exception)
    {
        return exception switch
        {
            // Validation exceptions - 400 Bad Request
            System.ComponentModel.DataAnnotations.ValidationException =>
                (StatusCodes.Status400BadRequest,
                    "Validation.Failed",
                    "One or more validation errors occurred.",
                    true),

            ArgumentException or ArgumentNullException =>
                (StatusCodes.Status400BadRequest,
                    "Argument.Invalid",
                    "Invalid argument provided.",
                    true),

            // Unauthorized - 401
            UnauthorizedAccessException =>
                (StatusCodes.Status401Unauthorized,
                    "Auth.Unauthorized",
                    "Unauthorized access.",
                    true),

            // Not Found - 404
            KeyNotFoundException =>
                (StatusCodes.Status404NotFound,
                    "Resource.NotFound",
                    "The requested resource was not found.",
                    true),

            // Conflict - 409
            InvalidOperationException =>
                (StatusCodes.Status409Conflict,
                    "Operation.Invalid",
                    "The operation could not be completed.",
                    true),

            // Timeout - 504
            TimeoutException =>
                (StatusCodes.Status504GatewayTimeout,
                    "Request.Timeout",
                    "The request timed out.",
                    false),

            // Database/Infrastructure errors - 500 (hide details in production)
            _ when exception.GetType().Namespace?.StartsWith("Microsoft.EntityFrameworkCore") == true =>
                (StatusCodes.Status500InternalServerError,
                    "Database.Error",
                    "A database error occurred.",
                    false),

            // All other exceptions - 500 Internal Server Error
            _ =>
                (StatusCodes.Status500InternalServerError,
                    "Server.InternalError",
                    "An unexpected error occurred.",
                    false)
        };
    }
}