namespace Reminlo.Domain.Abstractions;

public abstract class ScopedEntity<TKey> : AuditableEntity<TKey>
{
    /// <summary>
    /// The identifier for the relation with workspace.
    /// </summary>
    public TKey WorkspaceId { get; set; }
}


public abstract class ScopedEntity<TKey, TScopeKey>  : AuditableEntity<TKey>
{
    /// <summary>
    /// The identifier for the relation with workspace.
    /// </summary>
    public TScopeKey WorkspaceId { get; set; }
}