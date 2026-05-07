using ErrorOr;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Reminlo.Api.Filters;

/// <summary>
/// Action filter that automatically converts ErrorOr results to appropriate IActionResult.
/// Handles both success and error cases, mapping ErrorOr error types to proper HTTP status codes.
/// This filter runs BEFORE ApiResponseWrapperFilter, so successful results get wrapped.
/// </summary>
public class ErrorOrResultFilter : IAlwaysRunResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        // Check if the result is an ObjectResult containing an ErrorOr<T>
        if (context.Result is not ObjectResult objectResult)
        {
            return;
        }

        var value = objectResult.Value;
        if (value == null)
        {
            return;
        }

        // Check if it's an ErrorOr type (generic)
        var valueType = value.GetType();
        if (!valueType.IsGenericType || valueType.GetGenericTypeDefinition() != typeof(ErrorOr<>))
        {
            return;
        }

        // Get IsError property
        var isErrorProperty = valueType.GetProperty("IsError");
        if (isErrorProperty == null)
        {
            return;
        }

        var isError = (bool)isErrorProperty.GetValue(value)!;

        if (isError)
        {
            // Handle error case
            var errorsProperty = valueType.GetProperty("Errors");
            var errors = (List<Error>)errorsProperty!.GetValue(value)!;

            context.Result = ConvertErrorsToProblemDetails(errors, context.HttpContext);
        }
        else
        {
            // Handle success case - extract the value
            var valueProperty = valueType.GetProperty("Value");
            var actualValue = valueProperty!.GetValue(value);

            // Replace ErrorOr<T> with actual T value
            objectResult.Value = actualValue;
            objectResult.DeclaredType = actualValue?.GetType();
        }
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
        // No action needed after execution
    }

    /// <summary>
    /// Converts ErrorOr errors to Problem Details with proper HTTP status codes.
    /// Returns a consistent format with errors always as an array.
    /// </summary>
    private static IActionResult ConvertErrorsToProblemDetails(List<Error> errors, HttpContext httpContext)
    {
        if (errors.Count == 0)
        {
            // Fallback for empty errors list
            return new ObjectResult(new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "An error occurred.",
                Type = "https://tools.ietf.org/html/rfc9110#section-15.6.1"
            })
            {
                StatusCode = StatusCodes.Status500InternalServerError
            };
        }

        // Get status code from first error
        var firstError = errors[0];
        var statusCode = GetStatusCode(firstError.Type);

        // Build errors dictionary - group by code, collect descriptions as arrays
        var errorsDictionary = errors
            .GroupBy(e => e.Code)
            .ToDictionary(
                g => g.Key,
                g => g.Select(e => e.Description).ToArray()
            );

        var problemDetails = new ValidationProblemDetails(errorsDictionary)
        {
            Status = statusCode,
            Title = GetTitle(firstError.Type),
            Type = GetTypeUrl(statusCode),
        };

        return new ObjectResult(problemDetails)
        {
            StatusCode = statusCode
        };
    }

    private static int GetStatusCode(ErrorType errorType) => errorType switch
    {
        ErrorType.Conflict => StatusCodes.Status409Conflict,
        ErrorType.Validation => StatusCodes.Status400BadRequest,
        ErrorType.NotFound => StatusCodes.Status404NotFound,
        ErrorType.Unauthorized => StatusCodes.Status401Unauthorized,
        ErrorType.Forbidden => StatusCodes.Status403Forbidden,
        ErrorType.Failure => StatusCodes.Status500InternalServerError,
        _ => StatusCodes.Status500InternalServerError,
    };

    private static string GetTitle(ErrorType errorType) => errorType switch
    {
        ErrorType.Conflict => "A conflict occurred.",
        ErrorType.Validation => "One or more validation errors occurred.",
        ErrorType.NotFound => "The requested resource was not found.",
        ErrorType.Unauthorized => "Unauthorized access.",
        ErrorType.Forbidden => "Forbidden access.",
        ErrorType.Failure => "An error occurred while processing your request.",
        _ => "An error occurred while processing your request.",
    };

    private static string GetTypeUrl(int statusCode) =>
        $"https://tools.ietf.org/html/rfc9110#section-15.{statusCode / 100}.{statusCode % 100}";
}
