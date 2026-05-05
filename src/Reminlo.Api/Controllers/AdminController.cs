using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reminlo.Api.Abstractions;
using Reminlo.Application.Features.Workspace;

namespace Reminlo.Api.Controllers;

public class AdminController(IMediator mediator) : ApiController(mediator)
{
    [HttpGet("workspaces")]
    [Authorize(Roles = "admin,developer")]
    public async Task<IActionResult> GetAllWorkspaces(GetAllWorkspacesQuery query, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(query, cancellationToken);

        if (!response.IsSuccess)
            return BadRequest(response);

        return Ok(response);
    }

}