using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Reminlo.Api.Abstractions;

/// <summary>
/// Base controller for all API endpoints.
/// ErrorOr results are automatically converted to proper IActionResult by ErrorOrResultFilter.
/// Success responses are automatically wrapped by ApiResponseWrapperFilter.
/// Controllers can simply return ErrorOr<T> results directly.
/// </summary>
[Authorize]
[ApiController]
[EnableRateLimiting("fixed")]
[Route("api/[controller]")]
public abstract class ApiController(IMediator mediator) : ControllerBase
{
    protected readonly IMediator _mediator = mediator;
}
