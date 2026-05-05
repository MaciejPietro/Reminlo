using MediatR;
using Microsoft.AspNetCore.Mvc;
using Reminlo.Api.Abstractions;
using Reminlo.Application.Features.Identity.Users;
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
    
    [HttpGet]
    public async Task<IActionResult> GetMyWorkspaces(GetWorkspacesQuery query, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(query, cancellationToken);

        if (!response.IsSuccess)
            return BadRequest(response);

        return Ok(response);
    }
    
    [HttpPost("invitations")]
    public async Task<IActionResult> CreateInvitation([FromBody] CreateWorkspaceInvitationCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);

        if (!response.IsSuccess)
            return BadRequest(response);

        return CreatedAtAction(nameof(CreateInvitation), response);
    }
    
    /// <summary>
    /// Accept Workspace invitation.
    /// </summary>
    [HttpPatch("invitations/accept")]
    public async Task<IActionResult> AcceptInvitation([FromBody] AcceptWorkspaceInvitationCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);

        if (!response.IsSuccess)
            return BadRequest(response);

        return CreatedAtAction(nameof(AcceptInvitation), response);
    }
    
    /// <summary>
    /// Rejects Workspace invitation.
    /// </summary>
    [HttpPatch("invitations/reject")]
    public async Task<IActionResult> RejectInvitation([FromBody] RejectWorkspaceInvitationCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);

        if (!response.IsSuccess)
            return BadRequest(response);

        return CreatedAtAction(nameof(AcceptInvitation), response);
    }

}