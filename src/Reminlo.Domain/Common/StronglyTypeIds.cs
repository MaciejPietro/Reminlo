using StronglyTypedIds;
using System;

[assembly: StronglyTypedIdDefaults(Template.Guid, "guid-efcore")]

namespace Reminlo.Domain.Common;


public interface IGuid {}


[StronglyTypedId]
public partial struct ObligationReminderId : IGuid
{
    public static implicit operator ObligationReminderId(Guid guid)
    {
        return new ObligationReminderId(guid);
    }
}

[StronglyTypedId]
public partial struct ObligationId : IGuid
{
    public static implicit operator ObligationId(Guid guid)
    {
        return new ObligationId(guid);
    }
}

[StronglyTypedId]
public partial struct ObligationCategoryId : IGuid
{
    public static implicit operator ObligationCategoryId(Guid guid)
    {
        return new ObligationCategoryId(guid);
    }
}

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