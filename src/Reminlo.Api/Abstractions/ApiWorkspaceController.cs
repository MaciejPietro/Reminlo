using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Reminlo.Api.Abstractions;

[Route("api/workspaces/{workspaceId}/[controller]")]
public class ApiWorkspaceController(IMediator mediator) : ApiController(mediator)
{

}