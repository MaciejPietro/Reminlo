using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reminlo.Api.Abstractions;
using Reminlo.Application.Features.Obligations;

namespace Reminlo.Api.Controllers.Obligations;

/// <summary>
/// </summary>
public class ObligationCategoriesController(IMediator mediator) : ApiController(mediator)
{
    /// <summary>
    /// Creates a new Obligation Category.
    /// </summary>
    ///
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetObligationCategories(CancellationToken cancellationToken)
    {
        var request = new GetObligationCategoriesQuery();
        var response = await _mediator.Send(request, cancellationToken);

        return Ok(response);
    } 
    
    /// <summary>
    /// Creates a new Obligation Category.
    /// </summary>
    ///
    [HttpPost]
    [Authorize(Roles = "admin,developer")]
    public async Task<IActionResult> CreateObligationCategory(CreateObligationCategoryCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);

        return Ok(response);
    } 
    
    [HttpPut("{id}")]
    [Authorize(Roles = "admin,developer")]
    public async Task<IActionResult> UpdateObligationCategory(Guid id, [FromBody] UpdateObligationCategoryCommand request, CancellationToken cancellationToken)
    {
        var command = request with { Id = id };
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(response);
    } 
    
    [HttpDelete("{id}")]
    [Authorize(Roles = "admin,developer")]
    public async Task<IActionResult> DeleteObligationCategory(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeleteObligationCategoryCommand
        {
            Id = id
        };
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(response);
    } 
}