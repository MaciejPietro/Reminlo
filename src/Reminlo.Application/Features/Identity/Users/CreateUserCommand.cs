using Reminlo.Application.Services;
using Reminlo.Domain.DomainEvents.Users;
using Reminlo.Domain.Entities.Identity;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ResultKit;

namespace Reminlo.Application.Features.Identity.Users
{
    /// <summary>
    /// Command to create a new user with email, username, and password.
    /// </summary>
    public sealed record CreateUserCommand(
        string Email,
        string UserName,
        string Password
    ) : IRequest<Result<string>>;

    internal sealed class CreateUserCommandHandler(
        UserManager<ApplicationUser> userManager,
        ICacheService cacheService,
        IMediator mediator
    ) : IRequestHandler<CreateUserCommand, Result<string>>
    {
        public async Task<Result<string>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
        {
            var existingUser = await userManager.FindByEmailAsync(request.Email);
            if (existingUser != null)
                return Result<string>.ValidationFailure(new[] { new ValidationError("DuplicateEmail", "Email is already taken.") });

            var user = request.Adapt<ApplicationUser>();

            var result = await userManager.CreateAsync(user, request.Password);

            if (!result.Succeeded)
                return Result<string>.ValidationFailure(result.Errors.Select(e => new ValidationError(e.Code, e.Description)));

            await mediator.Publish(new UserCreatedEvent(user), cancellationToken);

            cacheService.Remove("users");

            return "User registration completed successfully.";
        }
    }
}
