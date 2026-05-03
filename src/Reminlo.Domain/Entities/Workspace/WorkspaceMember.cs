using Reminlo.Domain.Abstractions;
using Reminlo.Domain.Common;
using Reminlo.Domain.Enums;

namespace Reminlo.Domain.Entities.Workspace;

public class WorkspaceMember : BaseEntity<WorkspaceMemberId>
{
    private WorkspaceMember()
    {
        
    }
    
    public Guid UserId { get; private set; }
    public WorkspaceId WorkspaceId { get; private set; }
    public WorkspaceMemberRole Role { get; private set; }
    public WorkspaceMemberStatus Status { get; private set; }

    public static WorkspaceMember Create(Guid userId, WorkspaceId workspaceId)
    {
        var entity = new WorkspaceMember
        {
            UserId = userId,
            WorkspaceId = workspaceId,
            Role = WorkspaceMemberRole.Member,
            Status = WorkspaceMemberStatus.Active
        };

        return entity;
    }
}