using System.Linq.Expressions;
using Reminlo.Domain.Entities.Workspace;
using RepositoryKit.Core.Interfaces;

namespace Reminlo.Application.Repositories;

public class IWorkspaceMemberRepository : IRepository<WorkspaceMember>
{
    public IQueryable<WorkspaceMember> Query()
    {
        throw new NotImplementedException();
    }

    public Task<WorkspaceMember?> GetAsync(Expression<Func<WorkspaceMember, bool>> predicate, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<WorkspaceMember>> GetAllAsync(Expression<Func<WorkspaceMember, bool>>? predicate = null, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(WorkspaceMember entity, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task UpdateAsync(WorkspaceMember entity, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }

    public Task DeleteAsync(WorkspaceMember entity, CancellationToken cancellationToken = new CancellationToken())
    {
        throw new NotImplementedException();
    }
}