using Mapster;
using Reminlo.Application.Dto.Workspace;
using Reminlo.Domain.Enums;

namespace Reminlo.Application.Features.Workspace.Extensions;

internal static class WorkspaceExtensions
{
    public static WorkspaceListDto ToWorkspaceListDto(this Domain.Entities.Workspace.Workspace workspace, Guid? userId)
    {
        var dto = workspace.Adapt<WorkspaceListDto>();

        var userMember = workspace.Members.FirstOrDefault(m => m.UserId == userId);
        var workspaceRole = workspace.OwnerId == userId
            ? WorkspaceMemberRole.Admin
            : (userMember?.Role ?? WorkspaceMemberRole.Member);
        
        dto.Role = workspaceRole.ToString();

        // Set members count
        dto.MembersCount = workspace.Members.Count;

        return dto;
    }
}