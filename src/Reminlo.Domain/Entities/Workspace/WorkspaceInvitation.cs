using Microsoft.AspNetCore.Identity;
using Reminlo.Domain.Abstractions;
using Reminlo.Domain.Common;
using Reminlo.Domain.Entities.Identity;
using Reminlo.Domain.Enums;

namespace Reminlo.Domain.Entities.Workspace;

public class WorkspaceInvitation : BaseEntity<WorkspaceInvitationId>
{

    private WorkspaceInvitation()
    {
        
    }
    public WorkspaceId WorkspaceId { get; private set; }
    public string Email { get; private set; }       
    public string Token { get; private set; }      
    public InvitationStatus Status { get; private set; }
    public DateTime ExpiresAt { get; private set; }

    public static WorkspaceInvitation Create(WorkspaceId workspaceId, string userEmail, string token)
    {
        var entity = new WorkspaceInvitation
        {
            Id = Guid.NewGuid(),
            WorkspaceId = workspaceId,
            Email = userEmail,
            Token = token,
            Status = InvitationStatus.Pending,
            ExpiresAt = DateTime.UtcNow.AddDays(7)
        };

        return entity;
    }

    public void SetStatus(InvitationStatus status)
    {
        this.Status = status;
    }
}