namespace Reminlo.Domain.Abstractions;

public interface ScopedEntity<TKey>
{
    /// <summary>
    /// The identifier for the relation with workspace.
    /// </summary>
    public TKey WorkspaceId { get; set; } = default!;
}