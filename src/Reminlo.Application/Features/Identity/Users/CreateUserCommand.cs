using Reminlo.Application.Services;
using Reminlo.Application.Services.Identity;
using Reminlo.Domain.DomainEvents.Users;
using Reminlo.Domain.Entities.Identity;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ErrorOr;

namespace Reminlo.Application.Features.Identity.Users
{
    /// <summary>
    /// Command to create a new user with email, username, and password.
    /// </summary>
    public sealed record CreateUserCommand(
        string Email,
        string UserName,
        string Password
    ) : IRequest<ErrorOr<string>>;

    internal sealed class CreateUserCommandHandler(
        UserManager<ApplicationUser> userManager,
        ICacheService cacheService,
        IMediator mediator,
        IUserService userService
    ) : IRequestHandler<CreateUserCommand, ErrorOr<string>>
    {
        public async Task<ErrorOr<string>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var currentUserResult = await userService.GetCurrentUserAsync();
            if (!currentUserResult.IsError)
            {
                var hasAdminRole = userService.HasCurrentUserRole("admin");
                var hasDeveloperRole = userService.HasCurrentUserRole("developer");

                if (!hasAdminRole || !hasDeveloperRole)
                {
                    return Error.Unauthorized(description: "Only admin or developer users can create accounts.");
                }
            }

            var existingUser = await userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                return Error.Validation(code: "DuplicateEmail", description: "Email is already taken.");

            var user = request.Adapt<ApplicationUser>();

            var result = await userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                return result.Errors.Select(e => Error.Validation(code: e.Code, description: e.Description)).ToList();

            await mediator.Publish(new UserCreatedEvent(user), cancellationToken);

            cacheService.Remove("users");

            return "User registration completed successfully.";
        }
    }
}
