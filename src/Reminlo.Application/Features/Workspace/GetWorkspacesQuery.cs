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
using ErrorOr;

namespace Reminlo.Application.Features.Workspace;

public sealed record GetWorkspacesQuery() : IRequest<ErrorOr<List<WorkspaceListDto>>>;

internal sealed class GetWorkspacesQueryHandler(
    IWorkspaceRepository workspaceRepository,
    IUserService userService
) : IRequestHandler<GetWorkspacesQuery, ErrorOr<List<WorkspaceListDto>>>
{
    public async Task<ErrorOr<List<WorkspaceListDto>>> Handle(GetWorkspacesQuery request, CancellationToken cancellationToken)
    {
        var userResult = await userService.GetCurrentUserAsync();

        var user = userResult.Value;

        var workspaces = await workspaceRepository.GetAllWithMembersAsync(
            x => x.OwnerId == user!.Id || x.Members.Any(m => m.UserId == user!.Id), cancellationToken);

        return workspaces
            .Select(w => w.ToWorkspaceListDto(user!.Id))
            .ToList();
    }
}