using Reminlo.Application.Services;
using Reminlo.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ErrorOr;

namespace Reminlo.Application.Features.Identity.Roles;

/// <summary>
/// Query to retrieve all roles from the system.
/// </summary>
public sealed record GetAllRolesQuery() : IRequest<ErrorOr<List<ApplicationRole>>>;

/// <summary>
/// Handler that fetches all application roles, optionally using cached data,
/// and caches the result if not already cached.
/// </summary>
internal sealed class GetAllRolesQueryHandler(
    RoleManager<ApplicationRole> roleManager,
    ICacheService cacheService
    ) : IRequestHandler<GetAllRolesQuery, ErrorOr<List<ApplicationRole>>>
{
    public async Task<ErrorOr<List<ApplicationRole>>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = cacheService.Get<List<ApplicationRole>>("roles");

        if (roles is null)
        {
            roles = await roleManager.Roles.ToListAsync(cancellationToken);
            cacheService.Set("roles", roles);
        }

        return roles;
    }
}
