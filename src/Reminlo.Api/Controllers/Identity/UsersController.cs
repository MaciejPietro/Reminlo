using Reminlo.Application.Features.Identity.Auth;
using Reminlo.Application.Features.Identity.Users;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reminlo.Api.Abstractions;

namespace Reminlo.Api.Controllers.Identity;

/// <summary>
/// Request DTO for assigning a role to a user.
/// </summary>
public record AssignRoleRequest(Guid RoleId);

/// <summary>
/// Controller providing endpoints for managing application users.
/// </summary>
public sealed class UsersController : ApiController
{
    public UsersController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> Create(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(request, cancellationToken);

        return Ok(result);
    }

    /// <summary>
    /// Retrieves logged user.
    /// </summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetLoggedUserQuery(), cancellationToken);
        
        return Ok(result);
    }

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var command = request with { Id = id };
        var result = await _mediator.Send(command, cancellationToken);
        
        return Ok(result);
    }

    /// <summary>
    /// Deletes a user by their identifier.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DeleteByIdUserCommand(id), cancellationToken);
        
        return Ok(result);
    }

    /// <summary>
    /// Assigns a role to a user.
    /// </summary>
    [HttpPost("{userId}/roles")]
    [Authorize(Roles = "admin,developer")]
    public async Task<IActionResult> AssignRole(Guid userId, [FromBody] AssignRoleRequest request, CancellationToken cancellationToken)
    {
        var command = new AssignRoleToUserCommand(request.RoleId, userId);
        var result = await _mediator.Send(command, cancellationToken);
        
        return Ok(result);
    }
}
