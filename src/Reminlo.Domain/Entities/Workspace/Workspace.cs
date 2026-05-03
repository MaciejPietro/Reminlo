using Reminlo.Domain.Abstractions;
using Reminlo.Domain.Common;

namespace Reminlo.Domain.Entities.Workspace;

public class Workspace : BaseEntity<WorkspaceId>
{
    public Guid OwnerId { get; set; }
    public string Name { get; private set; } = null!;
    public ICollection<WorkspaceMember> Members { get; private set; }

    private Workspace()
    {
        Members = [];
    }

    public static Workspace Create(string ownerId, string name, ICollection<WorkspaceMember>? members = null)
    {
        var entity = new Workspace
        {
            Id = Guid.NewGuid(),
            OwnerId = new Guid(ownerId),
            Name = name.Trim(),
            Members = members ?? []
        };

        return entity;
    }

    public Workspace Update(string? name, ICollection<WorkspaceMember>? members = null)
    {
        if (name is not null)
        {
            Name = name;
        }

        if (members is not null)
        {
            Members = members;
        }

        return this;
    }
}