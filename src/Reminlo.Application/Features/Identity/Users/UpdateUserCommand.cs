using Reminlo.Application.Services;
using Reminlo.Domain.Entities.Identity;
using Mapster;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ErrorOr;
using Reminlo.Application.Services.Identity;

namespace Reminlo.Application.Features.Identity.Users;
public sealed record UpdateUserCommand(
    Guid Id,
    string? Email,
    string? UserName,
    string? Password,
    string? CurrentPassword
    ) : IRequest<ErrorOr<string>>;

internal sealed class UpdateUserCommandHandler(
    UserManager<ApplicationUser> userManager,
    IUserService  userService,
    ICacheService cacheService
    ) : IRequestHandler<UpdateUserCommand, ErrorOr<string>>
{
    public async Task<ErrorOr<string>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Id.ToString());
        var loggedUserId =   userService.GetCurrentUserId().Value;

        if (user is null)
            return Error.NotFound(description: "User not found!");

        var isSelf = user.Id == Guid.Parse(loggedUserId);

        if(!isSelf)
            return Error.Unauthorized(description: "You can't perform this action");

        // Update password if provided
        if (!string.IsNullOrEmpty(request.Password))
        {
            if (string.IsNullOrEmpty(request.CurrentPassword))
                return Error.Validation(description: "Current password is required when updating password");

            var isPasswordValid = await userManager.CheckPasswordAsync(user, request.CurrentPassword);
            if (!isPasswordValid)
                return Error.Validation(description: "Current password is incorrect");

            var changePasswordResult = await userManager.ChangePasswordAsync(user, request.CurrentPassword, request.Password);
            if (!changePasswordResult.Succeeded)
                return Error.Validation(description: string.Join(", ", changePasswordResult.Errors.Select(e => e.Description)));
        }

        // Only update Email if provided
        if (!string.IsNullOrEmpty(request.Email))
        {
            if (request.Email != user.Email)
            {
                var emailExists = await userManager.FindByEmailAsync(request.Email);
                if (emailExists != null)
                    return Error.Conflict(description: "Email is already in use");
            }

            user.Email = request.Email;
            user.NormalizedEmail = request.Email.ToUpper();
        }

        // Only update UserName if provided
        if (!string.IsNullOrEmpty(request.UserName))
        {
            if (request.UserName != user.UserName)
            {
                var userNameExists = await userManager.FindByNameAsync(request.UserName);
                if (userNameExists != null)
                    return Error.Conflict(description: "Username is already in use");
            }

            user.UserName = request.UserName;
            user.NormalizedUserName = request.UserName.ToUpper();
        }

        await userManager.UpdateAsync(user);

        cacheService.Remove("users");

        return "User updated successfully!";
    }
}
