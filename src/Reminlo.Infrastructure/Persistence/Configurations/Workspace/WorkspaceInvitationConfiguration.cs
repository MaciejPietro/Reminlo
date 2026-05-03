using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reminlo.Domain.Entities.Workspace;

namespace Reminlo.Infrastructure.Persistence.Configurations.Workspace;

public class WorkspaceInvitationConfiguration : IEntityTypeConfiguration<WorkspaceInvitation>
{
    public void Configure(EntityTypeBuilder<WorkspaceInvitation> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new Domain.Common.WorkspaceInvitationId(value));

        builder.Property(x => x.WorkspaceId)
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => new Domain.Common.WorkspaceId(value));

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.Token)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(x => x.ExpiresAt)
            .IsRequired();

        builder.HasIndex(x => x.Email);
        builder.HasIndex(x => x.Token);

        builder.HasOne<Domain.Entities.Workspace.Workspace>()
            .WithMany()
            .HasForeignKey(x => x.WorkspaceId);
    }
}