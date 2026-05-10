using Reminlo.Application.Repositories;
using Reminlo.Domain.Entities.Obligation;
using Reminlo.Infrastructure.Persistence;
using RepositoryKit.EntityFramework.Implementations;

namespace Reminlo.Infrastructure.Repositories;

public class ObligationRepository : EfRepository<Obligation, ApplicationDbContext>,  IObligationRepository
{
    public ObligationRepository(ApplicationDbContext context) : base(context)
    {
    }
}