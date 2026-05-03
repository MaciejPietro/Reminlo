using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reminlo.Api.Abstractions;
using Reminlo.Application.Features.HangFire;

namespace Reminlo.Api.Controllers;

/// <summary>
/// Controller for managing Hangfire dashboard users.
/// </summary>
public class HangFireController : ApiController
{
    public HangFireController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Creates a new Hangfire dashboard user.
    /// </summary>
    [HttpPost("users")]
    public async Task<IActionResult> CreateUser(CreateUserRepositoryCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(CreateUser), response);
    }
}
