using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Reminlo.Domain.Entities.Workspace;

namespace Reminlo.Infrastructure.Persistence.Configurations.Workspace;

public class WorkspaceMemberConfiguration : IEntityTypeConfiguration<WorkspaceMember>
{
    public void Configure(EntityTypeBuilder<WorkspaceMember> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Id)
            .HasConversion(
                id => id.Value,
                value => new Domain.Common.WorkspaceMemberId(value));

        builder.Property(x => x.UserId)
            .IsRequired();

        builder.Property(x => x.WorkspaceId)
            .IsRequired()
            .HasConversion(
                id => id.Value,
                value => new Domain.Common.WorkspaceId(value));

        builder.Property(x => x.Role)
            .IsRequired()
            .HasConversion<string>();

        builder.Property(x => x.Status)
            .IsRequired()
            .HasConversion<string>();

        builder.HasOne<Domain.Entities.Workspace.Workspace>()
            .WithMany(x => x.Members)
            .HasForeignKey(x => x.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}