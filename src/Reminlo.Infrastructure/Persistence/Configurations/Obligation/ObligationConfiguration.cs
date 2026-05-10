using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reminlo.Infrastructure.Persistence.Configurations.Obligation;

public class ObligationConfiguration : IEntityTypeConfiguration<Domain.Entities.Obligation.Obligation>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Obligation.Obligation> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new Domain.Common.ObligationId(value));

        // NAME
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(255);

        // DESCRIPTION
        builder.Property(x => x.Description)
            .HasMaxLength(1000);

        // WORKSPACE
        builder.Property(x => x.WorkspaceId)
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => new Domain.Common.WorkspaceId(value));

        // CATEGORY - Foreign Key
        builder.Property(x => x.CategoryId)
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => new Domain.Common.ObligationCategoryId(value));

        builder.HasOne(x => x.Category)
            .WithMany()
            .HasForeignKey(x => x.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // FREQUENCY
        builder.Property(x => x.FrequencyInterval)
            .HasConversion<string>();

        builder.Property(x => x.FrequencyValue);

        // NEXT DATE
        builder.Property(x => x.NextDate)
            .IsRequired();

        // EXPIRATION DATE
        builder.Property(x => x.ExpirationDate);

        // PRIORITY
        builder.Property(x => x.Priority)
            .IsRequired()
            .HasConversion<string>();

        // STATUS
        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>();

        // VISIBLE TO - Many-to-many relationship with WorkspaceMember
        builder.HasMany(x => x.VisibleTo)
            .WithMany()
            .UsingEntity(j => j.ToTable("ObligationVisibility"));

        // REMINDERS - One-to-many relationship
        builder.HasMany(x => x.Reminders)
            .WithOne(x => x.Obligation)
            .HasForeignKey(x => x.ObligationId)
            .OnDelete(DeleteBehavior.Cascade);

    }


}