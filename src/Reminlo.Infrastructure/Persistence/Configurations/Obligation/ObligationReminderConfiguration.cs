using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reminlo.Infrastructure.Persistence.Configurations.Obligation;

public class ObligationReminderConfiguration : IEntityTypeConfiguration<Domain.Entities.Obligation.ObligationReminder>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Obligation.ObligationReminder> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new Domain.Common.ObligationReminderId(value));

        // OBLIGATION
        builder.Property(x => x.ObligationId)
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => new Domain.Common.ObligationId(value));

        builder.HasOne(x => x.Obligation)
            .WithMany(x => x.Reminders)
            .HasForeignKey(x => x.ObligationId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);

        // NOTIFICATION OFFSET (in minutes)
        builder.Property(x => x.NotificationOffset)
            .IsRequired();

        // NOTIFICATION TYPE
        builder.Property(x => x.NotificationType)
            .IsRequired()
            .HasConversion<string>();

        // STATUS
        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>();

    }
}