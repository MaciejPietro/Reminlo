using Reminlo.Domain.Entities;
using Reminlo.Domain.Entities.Workspace;
using RepositoryKit.Core.Interfaces;
using ResultKit;

namespace Reminlo.Application.Repositories;

public interface IWorkspaceRepository : IRepository<Workspace>
{
    Task<WorkspaceInvitation> GetInvitationAsync(string token);
    Task<IEnumerable<Workspace>> GetAllWithMembersAsync(
        System.Linq.Expressions.Expression<Func<Workspace, bool>>? predicate = null,
        CancellationToken cancellationToken = default);
}