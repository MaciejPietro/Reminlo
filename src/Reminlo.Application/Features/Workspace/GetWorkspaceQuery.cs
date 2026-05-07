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
using Mapster;
using Reminlo.Domain.Common;

namespace Reminlo.Application.Features.Workspace;

public sealed record GetWorkspaceQuery() : IRequest<ErrorOr<WorkspaceDto>>
{
    public WorkspaceId Id { get; init; }
};

internal sealed class GetWorkspaceQueryHandler(
    IWorkspaceRepository workspaceRepository,
    IUserService userService
) : IRequestHandler<GetWorkspaceQuery, ErrorOr<WorkspaceDto>>
{
    public async Task<ErrorOr<WorkspaceDto>> Handle(GetWorkspaceQuery request, CancellationToken cancellationToken)
    {
        var workspace = await workspaceRepository.GetWithMembersAsync(x => x.Id == request.Id, cancellationToken);

        var userId = new Guid(userService.GetCurrentUserId().Value);

        var isOwner = workspace!.OwnerId == userId;
        var isMember = workspace.HasMember(userId);
        
        if (!isOwner && !isMember)
        {
            return Error.Forbidden(description: "You are not allowed to view this workspace.");
        }
            
        var workspaceDto = workspace.Adapt<WorkspaceDto>();
        return workspaceDto;
    }
}