using Reminlo.Domain.Abstractions;
using Reminlo.Domain.Common;

namespace Reminlo.Domain.Entities.Workspace;

public class Workspace : BaseEntity<WorkspaceId>
{
    public Guid OwnerId { get; set; }
    public string Name { get; private set; } = null!;
    public ICollection<WorkspaceMember> Members { get; private set; }

    public ICollection<WorkspaceInvitation> Invitations { get; private set; }

    
    private Workspace()
    {
        Members = [];
        Invitations = [];
    }

    public static Workspace Create(Guid ownerId, string name, ICollection<WorkspaceMember>? members = null)
    {
        var entity = new Workspace
        {
            Id = Guid.NewGuid(),
            OwnerId = ownerId,
            Name = name.Trim(),
            Members = members ?? []
        };

        return entity;
    }

   
    public Workspace AddInvitation(WorkspaceInvitation invitation)
    {
        Invitations.Add(invitation);

        return this;
    }

    public Workspace AddMember(WorkspaceMember member)
    {
        Members.Add(member);

        return this;
    }
}