using Reminlo.Domain.Abstractions;
using Reminlo.Domain.Common;
using Reminlo.Domain.Enums.Obligations;

namespace Reminlo.Domain.Entities.Obligation;

public enum ObligationReminderStatus { Active, Inactive }

public class ObligationReminder : BaseEntity<ObligationReminderId>
{
    private ObligationReminder()
    {
        
    }
    
    public ObligationId ObligationId { get; private set; }

    /// <summary>
    /// The notification offset in minutes relative to the obligation's NextDate.
    /// Negative values represent time before the obligation date, positive values represent time after.
    /// Examples: -180 (3 hours before), -2880 (2 days before), 2880 (2 days after), 0 (at the time of obligation).
    /// </summary>
    public int NotificationOffset { get; private set; }

    public NotificationType NotificationType { get; private set; }
    public ObligationReminderStatus Status { get; private set; }

    public Obligation Obligation { get; private set; }


    /// <summary>
    /// Creates a new obligation reminder.
    /// </summary>
    /// <param name="obligationId">The obligation to remind for.</param>
    /// <param name="notificationOffset">Offset in minutes (negative = before date, positive = after date). Defaults to -1440 (1 day before).</param>
    /// <param name="notificationType">Type of notification (Email, In-App, etc.). Defaults to Email.</param>
    public static ObligationReminder Create(
        ObligationId obligationId,
        int notificationOffset = -1440,
        NotificationType notificationType = NotificationType.Email)
    {
        var entity = new ObligationReminder()
        {
            ObligationId = obligationId,
            NotificationOffset = notificationOffset,
            NotificationType = notificationType,
            Status = ObligationReminderStatus.Active
        };

        return entity;
    }
}