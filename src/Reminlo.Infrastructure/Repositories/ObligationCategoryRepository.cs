using Reminlo.Application.Repositories;
using Reminlo.Domain.Entities.Obligation;
using Reminlo.Infrastructure.Persistence;
using RepositoryKit.EntityFramework.Implementations;

namespace Reminlo.Infrastructure.Repositories;

public class ObligationCategoryRepository: EfRepository<ObligationCategory, ApplicationDbContext>,  IObligationCategoryRepository
{
    public ObligationCategoryRepository(ApplicationDbContext context) : base(context)
    {
    }
}