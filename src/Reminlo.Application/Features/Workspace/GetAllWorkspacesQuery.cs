using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Reminlo.Application.Dto.Workspace;
using Reminlo.Application.Features.Identity.Users;
using Reminlo.Application.Features.Workspace.Extensions;
using Reminlo.Application.Repositories;
using Reminlo.Application.Services;
using Reminlo.Application.Services.Identity;
using Reminlo.Application.Services.Workspace;
using Reminlo.Domain.Entities.Identity;
using Reminlo.Domain.Enums;
using RepositoryKit.Core.Interfaces;
using ResultKit;

namespace Reminlo.Application.Features.Workspace;



public sealed record GetAllWorkspacesQuery() : IRequest<Result<List<WorkspaceListDto>>>;

internal sealed class GetAllWorkspacesQueryHandler(
    IWorkspaceRepository workspaceRepository,
    IUserService userService
) : IRequestHandler<GetAllWorkspacesQuery, Result<List<WorkspaceListDto>>>
{
    public async Task<Result<List<WorkspaceListDto>>> Handle(GetAllWorkspacesQuery request, CancellationToken cancellationToken)
    {
        var workspaces = await workspaceRepository.GetAllWithMembersAsync(cancellationToken: cancellationToken);
        var userId = userService.GetCurrentUserId().Value;
        var userGuid = userId is null ? (Guid?)null : Guid.Parse(userId);
        
        return workspaces
            .Select(w => w.ToWorkspaceListDto(userGuid))
            .ToList();
    }
}