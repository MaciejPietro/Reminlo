using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
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

    public async Task<WorkspaceInvitation> GetInvitationAsync(string token)
    {
        var invitation = await _context.WorkspaceInvitations.FirstAsync(x => x.Token == token);

        return invitation;
    }

    public async Task<IEnumerable<Workspace>> GetAllWithMembersAsync(
        System.Linq.Expressions.Expression<Func<Workspace, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Workspaces.Include(w => w.Members).AsQueryable();

        if (predicate != null)
            query = query.Where(predicate);

        return await query.ToListAsync(cancellationToken);
    }

    public async Task<Workspace?> GetWithMembersAsync(Expression<Func<Workspace, bool>> predicate, CancellationToken cancellationToken = default)
    {
        return await _context.Workspaces
            .Include(w => w.Members)
            .ThenInclude(m => m.User)
            .FirstOrDefaultAsync(predicate, cancellationToken);
    }
}