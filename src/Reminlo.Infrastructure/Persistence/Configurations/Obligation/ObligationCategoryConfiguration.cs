using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reminlo.Infrastructure.Persistence.Configurations.Obligation;

public class ObligationCategoryConfiguration : IEntityTypeConfiguration<Domain.Entities.Obligation.ObligationCategory>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Obligation.ObligationCategory> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new Domain.Common.ObligationCategoryId(value));

        // NAME
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(x => x.Name)
            .IsUnique();

        // CREATED AT
        builder.Property(x => x.CreatedAt)
            .IsRequired();
    }


}