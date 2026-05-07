namespace Reminlo.Api.Models;

/// <summary>
/// Standard API response wrapper for successful responses.
/// </summary>
/// <typeparam name="T">The type of data being returned</typeparam>
public class ApiResponse<T>
{
    /// <summary>
    /// HTTP status code
    /// </summary>
    public int Status { get; set; }

    /// <summary>
    /// The actual response data
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// Optional message for additional context
    /// </summary>
    public string? Message { get; set; }

    public ApiResponse(int status, T? data, string? message = null)
    {
        Status = status;
        Data = data;
        Message = message;
    }
}

/// <summary>
/// Non-generic version for responses without data (e.g., 204 No Content)
/// </summary>
public class ApiResponse
{
    public int Status { get; set; }
    public string? Message { get; set; }

    public ApiResponse(int status, string? message = null)
    {
        Status = status;
        Message = message;
    }
}
