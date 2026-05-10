using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reminlo.Api.Abstractions;
using Reminlo.Application.Features.Obligations;
using Reminlo.Domain.Common;

namespace Reminlo.Api.Controllers.Obligations;

/// <summary>
/// </summary>
public class ObligationsController(IMediator mediator) : ApiWorkspaceController(mediator)
{
    /// <summary>
    /// Creates a new Obligation Category.
    /// </summary>
    ///
    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateObligation(WorkspaceId workspaceId, [FromBody] CreateObligationCommand request, CancellationToken cancellationToken)
    {
        
        var command = request with {  WorkspaceId = workspaceId };
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(response);
    } 
    
}