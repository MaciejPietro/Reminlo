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
    public int ReminderDays { get; private set; }
    public NotificationType NotificationType { get; private set; }
    public ObligationReminderStatus Status { get; private set; }
    
    public static ObligationReminder Create(ObligationId obligationId, int reminderDays = 1)
    {
        var entity = new ObligationReminder()
        {
            ObligationId = obligationId,
            ReminderDays = reminderDays,
            NotificationType = NotificationType.Email,
            Status = ObligationReminderStatus.Active
        };

        return entity;
    }
}