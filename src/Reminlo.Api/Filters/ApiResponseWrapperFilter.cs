using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Reminlo.Api.Models;

namespace Reminlo.Api.Filters;

/// <summary>
/// Action filter that wraps all successful API responses in a standard envelope.
/// Error responses (ProblemDetails) are not wrapped.
/// </summary>
public class ApiResponseWrapperFilter : IAlwaysRunResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        // Don't wrap error responses (ProblemDetails, ValidationProblemDetails)
        if (context.Result is ObjectResult { Value: ProblemDetails })
        {
            return;
        }

        // Wrap successful responses
        if (context.Result is ObjectResult objectResult)
        {
            var statusCode = objectResult.StatusCode ?? context.HttpContext.Response.StatusCode;

            // Get optional custom message from HttpContext
            var message = context.HttpContext.Items.TryGetValue("SuccessMessage", out var msg)
                ? msg?.ToString()
                : null;

            // Wrap the value in ApiResponse
            var wrappedResponse = new ApiResponse<object>(
                status: statusCode,
                data: objectResult.Value,
                message: message
            );

            objectResult.Value = wrappedResponse;
            objectResult.DeclaredType = typeof(ApiResponse<object>);
        }
        else if (context.Result is StatusCodeResult statusCodeResult)
        {
            // For status code results without data (e.g., NoContent)
            context.Result = new ObjectResult(new ApiResponse(
                status: statusCodeResult.StatusCode
            ))
            {
                StatusCode = statusCodeResult.StatusCode
            };
        }
        else if (context.Result is CreatedAtActionResult createdResult)
        {
            // Handle CreatedAtAction results
            var wrappedResponse = new ApiResponse<object>(
                status: StatusCodes.Status201Created,
                data: createdResult.Value
            );

            context.Result = new CreatedAtActionResult(
                createdResult.ActionName,
                createdResult.ControllerName,
                createdResult.RouteValues,
                wrappedResponse
            );
        }
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
        // No action needed after execution
    }
}
