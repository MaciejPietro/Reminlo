using Reminlo.Application.Services;
using Reminlo.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ErrorOr;

namespace Reminlo.Application.Features.Identity.Auth;

/// <summary>
/// Command to assign a specific role to a user by their IDs.
/// </summary>
public sealed record AssignRoleToUserCommand(
    Guid RoleId,
    Guid UserId
    ) : IRequest<ErrorOr<string>>;

/// <summary>
/// Handler that processes assigning a role to a user.
/// It validates the existence of the role and user,
/// assigns the role, removes related cache entries,
/// and returns a success message.
/// </summary>
internal sealed class AssignRoleToUserCommandHandler(
    RoleManager<ApplicationRole> roleManager,
    UserManager<ApplicationUser> userManager,
    ICacheService cacheService
    ) : IRequestHandler<AssignRoleToUserCommand, ErrorOr<string>>
{
    public async Task<ErrorOr<string>> Handle(AssignRoleToUserCommand request, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(request.RoleId.ToString());
        if (role is null)
            return Error.NotFound(description: "Role not found.");

        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user is null)
            return Error.NotFound(description: "User not found.");

        await userManager.AddToRoleAsync(user, role.Name!);

        cacheService.Remove("users");

        return "Role has been successfully assigned to the user.";
    }
}
