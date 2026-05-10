namespace Reminlo.Domain.Abstractions;

/// <summary>
/// Represents a base entity that includes audit metadata for create, update, and delete operations.
/// </summary>
/// <typeparam name="TKey">The type of the entity's identifier.</typeparam>
public abstract class AuditableEntity<TKey> : BaseEntity<TKey>, IUserAuditable
{
    public DateTime CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? DeletedAt { get; set; }
    public string? DeletedBy { get; set; }
    public bool IsDeleted { get; set; }
}
