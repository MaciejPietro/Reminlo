using Mapster;
using Reminlo.Domain.Enums;

namespace Reminlo.Application.Features.Workspace.Extensions;

internal static class WorkspaceExtensions
{
    public static WorkspaceDto ToWorkspaceDto(this Domain.Entities.Workspace.Workspace workspace, Guid userId)
    {
        var dto = workspace.Adapt<WorkspaceDto>();

        // Get user's role from WorkspaceMember
        var userMember = workspace.Members.FirstOrDefault(m => m.UserId == userId);
        dto.Role = (userMember?.Role ?? WorkspaceMemberRole.Member).ToString();

        // Set members count
        dto.MembersCount = workspace.Members.Count;

        return dto;
    }
}