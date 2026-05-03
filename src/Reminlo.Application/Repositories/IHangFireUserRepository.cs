using Reminlo.Domain.Entities;
using RepositoryKit.Core.Interfaces;

namespace Reminlo.Application.Repositories;

/// <summary>
/// Repository interface for managing Hangfire dashboard user entities.
/// </summary>
public interface IHangFireUserRepository : IRepository<HangFireUser>
{
}
