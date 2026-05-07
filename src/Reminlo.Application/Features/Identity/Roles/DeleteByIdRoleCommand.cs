using Reminlo.Application.Services;
using Reminlo.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ErrorOr;

namespace Reminlo.Application.Features.Identity.Roles;

/// <summary>
/// Command to delete a role by its unique identifier.
/// </summary>
public sealed record DeleteByIdRoleCommand(
    Guid Id) : IRequest<ErrorOr<string>>;

/// <summary>
/// Handler that processes deleting a role from the system,
/// clears the roles cache, and returns a confirmation message.
/// </summary>
internal sealed class DeleteByIdRoleCommandHandler(
    RoleManager<ApplicationRole> roleManager,
    ICacheService cacheService
    ) : IRequestHandler<DeleteByIdRoleCommand, ErrorOr<string>>
{
    public async Task<ErrorOr<string>> Handle(DeleteByIdRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(request.Id.ToString());

        if (role is null)
            return Error.NotFound(description: "Role not found.");

        await roleManager.DeleteAsync(role);

        cacheService.Remove("roles");

        return "Role has been deleted successfully.";
    }
}
