using StronglyTypedIds;
using System;

[assembly: StronglyTypedIdDefaults(Template.Guid, "guid-efcore")]

namespace Reminlo.Domain.Common;


public interface IGuid {}

[StronglyTypedId]
public partial struct WorkspaceId : IGuid
{
    public static implicit operator WorkspaceId(Guid guid)
    {
        return new WorkspaceId(guid);
    }
}


[StronglyTypedId]
public partial struct WorkspaceMemberId : IGuid
{
    public static implicit operator WorkspaceMemberId(Guid guid)
    {
        return new WorkspaceMemberId(guid);
    }
}

[StronglyTypedId]
public partial struct WorkspaceInvitationId : IGuid
{
    public static implicit operator WorkspaceInvitationId(Guid guid)
    {
        return new WorkspaceInvitationId(guid);
    }
}