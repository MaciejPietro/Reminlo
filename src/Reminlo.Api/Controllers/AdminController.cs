using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reminlo.Api.Abstractions;
using Reminlo.Application.Features.Identity.Users;
using Reminlo.Application.Features.Workspace;

namespace Reminlo.Api.Controllers;

[Authorize(Roles = "admin,developer")]
public class AdminController(IMediator mediator) : ApiController(mediator)
{
    [HttpGet("workspaces")]
    public async Task<IActionResult> GetAllWorkspaces(GetAllWorkspacesQuery query, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(query, cancellationToken);

        return Ok(response);
    }
    
    /// <summary>
    /// Retrieves all users.
    /// </summary>
    [HttpGet("users")]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetAllUserQuery(), cancellationToken);
        
        return Ok(result);
    }

}