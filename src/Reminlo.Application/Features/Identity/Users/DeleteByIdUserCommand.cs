using Reminlo.Application.Services;
using Reminlo.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ErrorOr;
using Reminlo.Application.Services.Identity;

namespace Reminlo.Application.Features.Identity.Users;
public sealed record DeleteByIdUserCommand(
    Guid Id
    ) : IRequest<ErrorOr<Unit>>;

internal sealed class DeleteByIdUserCommandHandler(
    UserManager<ApplicationUser> userManager,
    ICacheService cacheService,
    IUserService userService
    ) : IRequestHandler<DeleteByIdUserCommand, ErrorOr<Unit>>
{
    public async Task<ErrorOr<Unit>> Handle(DeleteByIdUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.Id.ToString());

        if (user is null)
            return Error.NotFound(description: "User not found!");
        
        var loggedUserId = userService.GetCurrentUserId().Value;
        var isSelf = user.Id == Guid.Parse(loggedUserId);
        if(!isSelf)
            return Error.Unauthorized(description: "You can't perform this action");

        // Anonymize email and username to free them up for future use
        // Use Unix timestamp (10 digits) for shorter suffix
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var suffix = $"_del_{timestamp}";

        // Truncate to fit within max length constraints (Email: 100, UserName: 25)
        var maxEmailLength = 100 - suffix.Length;
        var maxUserNameLength = 25 - suffix.Length;

        var truncatedEmail = user.Email!.Length > maxEmailLength
            ? user.Email.Substring(0, maxEmailLength)
            : user.Email;

        var truncatedUserName = user.UserName!.Length > maxUserNameLength
            ? user.UserName.Substring(0, maxUserNameLength)
            : user.UserName;

        user.Email = truncatedEmail + suffix;
        user.UserName = truncatedUserName + suffix;
        user.NormalizedEmail = user.Email.ToUpper();
        user.NormalizedUserName = user.UserName.ToUpper();

        await userManager.UpdateAsync(user);
        await userManager.DeleteAsync(user);

        cacheService.Remove("users");

        return Unit.Value;
    }
}
