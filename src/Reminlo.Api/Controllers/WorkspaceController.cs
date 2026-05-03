using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reminlo.Api.Abstractions;
using Reminlo.Application.Features.Workspace;

namespace Reminlo.Api.Controllers;

/// <summary>
/// Controller for workspaces management.
/// </summary>
public class WorkspaceController(IMediator mediator) : ApiController(mediator)
{
    /// <summary>
    /// Creates a new Workspace.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateWorkspace(CreateWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);

        if (!response.IsSuccess)
            return BadRequest(response);

        return CreatedAtAction(nameof(CreateWorkspace), response);
    }
}