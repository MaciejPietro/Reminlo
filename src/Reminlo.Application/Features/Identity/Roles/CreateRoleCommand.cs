using Reminlo.Application.Services;
using Reminlo.Domain.Entities.Identity;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ErrorOr;

namespace Reminlo.Application.Features.Identity.Roles;

/// <summary>
/// Command to create a new role with the specified name.
/// </summary>
public sealed record CreateRoleCommand(string Name) : IRequest<ErrorOr<string>>;

/// <summary>
/// Handler that processes creating a new application role,
/// clears the cache, and returns a confirmation message.
/// </summary>
internal sealed class CreateRoleCommandHandler(
    RoleManager<ApplicationRole> roleManager,
    ICacheService cacheService
    ) : IRequestHandler<CreateRoleCommand, ErrorOr<string>>
{
    public async Task<ErrorOr<string>> Handle(CreateRoleCommand request, CancellationToken cancellationToken)
    {
        var isRoleExists = await roleManager.RoleExistsAsync(request.Name);
        if (isRoleExists)
            return Error.Conflict(description: $"Role '{request.Name}' already exists.");

        var role = request.Adapt<ApplicationRole>();

        await roleManager.CreateAsync(role);

        cacheService.Remove("roles");

        return $"Role '{role.Name}' has been created successfully.";
    }
}
