using Reminlo.Domain.Abstractions;
using Reminlo.Domain.Common;
using Reminlo.Domain.Entities.Workspace;
using Reminlo.Domain.Enums.Obligations;

namespace Reminlo.Domain.Entities.Obligation;

/// <summary>
/// Represents the lifecycle status of an obligation.
/// </summary>
public enum ObligationStatus { Active, Inactive }

/// <summary>
/// Represents a recurring or one-time obligation within a workspace that tracks tasks or responsibilities.
/// Obligations can be assigned to workspace members with configurable frequency, priority, and visibility settings.
/// </summary>
public class Obligation : AuditableEntity<ObligationId>, ScopedEntity<WorkspaceId>
{
    /// <summary>
    /// The workspace that owns this obligation.
    /// </summary>
    public WorkspaceId WorkspaceId { get; set; }

    /// <summary>
    /// The category this obligation belongs to. Optional.
    /// </summary>
    public ObligationCategoryId? CategoryId { get; private set; }

    /// <summary>
    /// The name or title of the obligation.
    /// </summary>
    public string Name { get; private set; } = $"Meet John Doe-{Random.Shared.Next(2)}";

    /// <summary>
    /// Additional details or description of the obligation.
    /// </summary>
    public string? Description { get; private set; } = null;

    /// <summary>
    /// The frequency interval at which this obligation repeats (e.g., Daily, Weekly, Monthly, Yearly).
    /// If null, the obligation is a one-time occurrence.
    /// </summary>
    public ObligationFrequency? FrequencyInterval { get; private set; }

    /// <summary>
    /// The numeric value for the frequency interval. For example, with FrequencyInterval=Weekly and FrequencyValue=2,
    /// the obligation occurs biweekly. Must be used in conjunction with FrequencyInterval.
    /// If null, the obligation is a one-time occurrence.
    /// </summary>
    public int? FrequencyValue { get; private set; }

    /// <summary>
    /// The date of the next occurrence. This is the date by which the action must be performed.
    /// Reminders are sent at this time or the day before, and notifications are displayed to relevant members.
    /// </summary>
    public DateTime NextDate { get; private set; }

    /// <summary>
    /// The date when the obligation automatically becomes inactive and is no longer valid.
    /// Optional; if not set, the obligation remains active indefinitely.
    /// </summary>
    public DateTime? ExpirationDate { get; private set; }

    /// <summary>
    /// The priority level of the obligation (Low, Medium, High, or Crucial).
    /// </summary>
    public ObligationPriority Priority { get; private set; }

    /// <summary>
    /// Defines which workspace members can view this obligation.
    /// Null means all members can see it.
    /// Empty collection means only creator and owner can see it.
    /// Otherwise contains the specific WorkspaceMemberId values with access.
    /// </summary>
    public ICollection<WorkspaceMemberId>? VisibleTo { get; private set; }

    /// <summary>
    /// The current status of the obligation (Active or Inactive).
    /// </summary>
    public ObligationStatus Status { get; private set; }

    /// <summary>
    /// The category navigation property for this obligation.
    /// </summary>
    public ObligationCategory Category { get; private set; }
    
    public static Obligation Create(
        WorkspaceId workspaceId, 
        ObligationCategoryId categoryId, 
        string name, 
        DateTime startDate,
        string description = "", 
        ObligationFrequency? frequency = null, 
        int? frequencyValue = null,
        DateTime? endDate = null,
        ICollection<WorkspaceMemberId>? visibleTo = null,
        ObligationPriority priority = ObligationPriority.Low
        )
    {
        var entity = new Obligation()
        {
            WorkspaceId = workspaceId,
            CategoryId = categoryId,
            Name =  name,
            Description = description,
            FrequencyInterval = frequency,
            FrequencyValue = frequencyValue,
            NextDate = startDate,
            ExpirationDate = endDate,
            Priority = priority,
            VisibleTo = visibleTo,
            Status = ObligationStatus.Active
        };

        return entity;
    }

}