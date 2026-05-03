using Reminlo.Application.Repositories;
using Reminlo.Domain.Entities.Workspace;
using Reminlo.Infrastructure.Persistence;
using RepositoryKit.EntityFramework.Implementations;

namespace Reminlo.Infrastructure.Repositories;

/// <summary>
/// Entity Framework repository implementation for managing workspaces.
/// </summary>
internal sealed class WorkspaceRepository
    : EfRepository<Workspace, ApplicationDbContext>, IWorkspaceRepository
{
    public WorkspaceRepository(ApplicationDbContext context)
        : base(context)
    {
    }
}