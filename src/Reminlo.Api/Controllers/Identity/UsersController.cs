using Reminlo.Application.Features.Identity.Auth;
using Reminlo.Application.Features.Identity.Users;
using MediatR;
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
    public async Task<IActionResult> Create(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(GetAll), response);
    }
    
    /// <summary>
    /// Retrieves logged user.
    /// </summary>
    [HttpGet("me")]
    public async Task<IActionResult> GetMe(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetLoggedUserQuery(), cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Retrieves all users.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetAllUserQuery(), cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Updates an existing user.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var command = request with { Id = id };
        var response = await _mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Deletes a user by their identifier.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteByIdUserCommand(id);
        await _mediator.Send(command, cancellationToken);
        return NoContent();
    }

    /// <summary>
    /// Assigns a role to a user.
    /// </summary>
    [HttpPost("{userId}/roles")]
    public async Task<IActionResult> AssignRole(Guid userId, [FromBody] AssignRoleRequest request, CancellationToken cancellationToken)
    {
        var command = new AssignRoleToUserCommand(request.RoleId, userId);
        var response = await _mediator.Send(command, cancellationToken);
        return Ok(response);
    }
}
