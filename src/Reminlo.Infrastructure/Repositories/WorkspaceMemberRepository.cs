using Reminlo.Application.Repositories;
using Reminlo.Domain.Entities.Workspace;
using Reminlo.Infrastructure.Persistence;
using RepositoryKit.EntityFramework.Implementations;

namespace Reminlo.Infrastructure.Repositories;

public class WorkspaceMemberRepository : EfRepository<WorkspaceMember, ApplicationDbContext>, 
    IWorkspaceMemberRepository
{
    public WorkspaceMemberRepository(ApplicationDbContext context) : base(context)
    {
    }
}