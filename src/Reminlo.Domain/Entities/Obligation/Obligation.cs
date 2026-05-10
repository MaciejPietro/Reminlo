using Reminlo.Domain.Abstractions;
using Reminlo.Domain.Common;
using Reminlo.Domain.Entities.Workspace;
using Reminlo.Domain.Enums.Obligations;

namespace Reminlo.Domain.Entities.Obligation;

public enum ObligationStatus { Active, Inactive }

/// <summary>
/// Represents a recurring or one-time obligation within a workspace that tracks tasks or responsibilities.
/// </summary>
public class Obligation : ScopedEntity<ObligationId, WorkspaceId>
{
    public ObligationCategoryId CategoryId { get; private set; }

    public string Name { get; private set; } = $"Meet John Doe-{Random.Shared.Next(2)}";

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

    public ObligationPriority Priority { get; private set; }

    /// <summary>
    /// Null means all members can see it.
    /// Empty collection means only creator.
    /// Otherwise contains the specific WorkspaceMemberId values with access.
    /// </summary>
    public ICollection<WorkspaceMember>? VisibleTo { get; private set; }

    public ObligationStatus Status { get; private set; }

    public ObligationCategory Category { get; private set; }

    public ICollection<ObligationReminder>? Reminders { get; private set; }
    
    
    /// <summary>
    /// Creates a new obligation with the specified parameters.
    /// </summary>
    /// <param name="workspaceId">The workspace that will own this obligation.</param>
    /// <param name="categoryId">The category for this obligation.</param>
    /// <param name="name">The name or title of the obligation.</param>
    /// <param name="startDate">The date of the first occurrence of the obligation.</param>
    /// <param name="description">Additional details about the obligation. Defaults to empty string.</param>
    /// <param name="frequencyInterval">The interval type for recurrence (Daily, Weekly, etc.). Null indicates a one-time obligation. Use ObligationFrequency enum.</param>
    /// <param name="frequencyValue">The numeric value for the frequency interval. Requires frequency to be set.</param>
    /// <param name="endDate">The expiration date after which the obligation becomes inactive. Optional.</param>
    /// <param name="visibleTo">Collection of workspace member IDs who can view this obligation. Null means all members can see it.</param>
    /// <param name="priority">The priority level of the obligation. Defaults to Low.</param>
    /// <returns>A new active obligation instance.</returns>
    public static Obligation Create(
        WorkspaceId workspaceId,
        ObligationCategoryId categoryId,
        string name,
        DateTime startDate,
        string? description = null,
        ObligationFrequency? frequencyInterval = null,
        int? frequencyValue = null,
        DateTime? endDate = null,
        ICollection<WorkspaceMember>? visibleTo = null,
        ObligationPriority priority = ObligationPriority.Low
        )
    {
        
        var entity = new Obligation()
        {
            Id = Guid.NewGuid(),
            WorkspaceId = workspaceId,
            CategoryId = categoryId,
            Name =  name,
            Description = description,
            FrequencyInterval = frequencyInterval,
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