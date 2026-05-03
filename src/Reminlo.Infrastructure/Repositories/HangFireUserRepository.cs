using Reminlo.Application.Repositories;
using Reminlo.Domain.Entities;
using Reminlo.Infrastructure.Persistence;
using RepositoryKit.EntityFramework.Implementations;

namespace Reminlo.Infrastructure.Repositories;

/// <summary>
/// Entity Framework repository implementation for managing Hangfire dashboard users.
/// </summary>
internal sealed class HangFireUserRepository
    : EfRepository<HangFireUser, ApplicationDbContext>, IHangFireUserRepository
{
    public HangFireUserRepository(ApplicationDbContext context)
        : base(context)
    {
    }
}
