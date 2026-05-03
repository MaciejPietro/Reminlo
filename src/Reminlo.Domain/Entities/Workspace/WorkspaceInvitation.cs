using Reminlo.Domain.Abstractions;
using Reminlo.Domain.Common;
using Reminlo.Domain.Enums;

namespace Reminlo.Domain.Entities.Workspace;

public class WorkspaceInvitation : BaseEntity<WorkspaceInvitationId>
{
    public WorkspaceId WorkspaceId { get; set; }
    public string Email { get; set; }       
    public string Token { get; set; }      
    public InvitationStatus Status { get; set; }
    public DateTime ExpiresAt { get; set; }
}