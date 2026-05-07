using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reminlo.Api.Abstractions;
using Reminlo.Application.Features.Identity.Users;
using Reminlo.Application.Features.Workspace;
using Reminlo.Domain.Common;

namespace Reminlo.Api.Controllers;

/// <summary>
/// Controller for workspaces management.
/// </summary>
public class WorkspacesController(IMediator mediator) : ApiController(mediator)
{
    /// <summary>
    /// Creates a new Workspace.
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateWorkspace(CreateWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);

        return Ok(response);
    }

    [HttpGet]
    public async Task<IActionResult> GetWorkspaces(GetWorkspacesQuery query, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(query, cancellationToken);

        return Ok(response);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> GetWorkspace(WorkspaceId id, [FromQuery] GetWorkspaceQuery query, CancellationToken cancellationToken)
    {
        var command = query with { Id = id };
        
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(response);
    }


    [HttpPost("invitations")]
    public async Task<IActionResult> CreateInvitation([FromBody] CreateWorkspaceInvitationCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        
        return Ok(response);
    }

    /// <summary>
    /// Accept Workspace invitation.
    /// </summary>
    [HttpPatch("invitations/accept")]
    public async Task<IActionResult> AcceptInvitation([FromBody] AcceptWorkspaceInvitationCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);

        return Ok(response);
    }

    /// <summary>
    /// Rejects Workspace invitation.
    /// </summary>
    [HttpPatch("invitations/reject")]
    public async Task<IActionResult> RejectInvitation([FromBody] RejectWorkspaceInvitationCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);

        return Ok(response);
    }

}