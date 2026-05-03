using Reminlo.Application.Services;
using Reminlo.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ResultKit;

namespace Reminlo.Application.Features.Identity.Users;

public sealed record GetAllUserQuery() : IRequest<Result<List<ApplicationUser>>>;

internal sealed class GetAllUserQueryHandler(
    UserManager<ApplicationUser> userManager,
    ICacheService cacheService
    ) : IRequestHandler<GetAllUserQuery, Result<List<ApplicationUser>>>
{
    public async Task<Result<List<ApplicationUser>>> Handle(GetAllUserQuery request, CancellationToken cancellationToken)
    {
        var users = cacheService.Get<List<ApplicationUser>>("users");

        if (users is null) { 
            users = await userManager.Users.ToListAsync(cancellationToken);
            cacheService.Set("users", users);
        }

        return users;
    }
}
