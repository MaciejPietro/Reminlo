using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Reminlo.Infrastructure.Persistence.Configurations.Workspace;

public class WorkspaceConfiguration : IEntityTypeConfiguration<Domain.Entities.Workspace.Workspace>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Workspace.Workspace> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new Domain.Common.WorkspaceId(value));

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.OwnerId)
            .IsRequired();

        builder.HasMany(x => x.Members)
            .WithOne()
            .HasForeignKey(x => x.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasMany(x => x.Invitations)
            .WithOne()
            .HasForeignKey(x => x.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}