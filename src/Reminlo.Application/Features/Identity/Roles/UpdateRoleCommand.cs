using Reminlo.Application.Services;
using Reminlo.Domain.Entities.Identity;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ErrorOr;

namespace Reminlo.Application.Features.Identity.Roles;

/// <summary>
/// Command to update an existing role's properties.
/// </summary>
public sealed record UpdateRoleCommand(
    Guid Id,
    string Name
) : IRequest<ErrorOr<string>>;

/// <summary>
/// Handler that updates an existing application role,
/// clears the roles cache, and returns a confirmation message.
/// </summary>
internal sealed class UpdateRoleCommandHandler(
    RoleManager<ApplicationRole> roleManager,
    ICacheService cacheService
) : IRequestHandler<UpdateRoleCommand, ErrorOr<string>>
{
    public async Task<ErrorOr<string>> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
    {
        var role = await roleManager.FindByIdAsync(request.Id.ToString());

        if (role is null)
            return Error.NotFound(description: "Role not found.");

        request.Adapt(role);

        await roleManager.UpdateAsync(role);

        cacheService.Remove("roles");

        return "Role has been updated successfully.";
    }
}
