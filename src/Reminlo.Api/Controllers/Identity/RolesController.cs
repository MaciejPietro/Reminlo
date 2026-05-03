using Reminlo.Application.Features.Identity.Roles;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reminlo.Api.Abstractions;

namespace Reminlo.Api.Controllers.Identity;

/// <summary>
/// Controller providing endpoints for managing application roles.
/// </summary>
public class RolesController : ApiController
{
    public RolesController(IMediator mediator) : base(mediator)
    {
    }

    /// <summary>
    /// Creates a new role.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> Create(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return CreatedAtAction(nameof(GetAll), response);
    }

    /// <summary>
    /// Retrieves all roles.
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(new GetAllRolesQuery(), cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Updates an existing role.
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var command = request with { Id = id };
        var response = await _mediator.Send(command, cancellationToken);
        return Ok(response);
    }

    /// <summary>
    /// Deletes a role by its identifier.
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteByIdRoleCommand(id);
        var response = await _mediator.Send(command, cancellationToken);
        return NoContent();
    }
}
