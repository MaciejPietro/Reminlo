using Reminlo.Application.Services;
using Reminlo.Application.Services.Identity;
using Reminlo.Domain.DomainEvents.Users;
using Reminlo.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Reminlo.Application.EventHandlers.Users;

/// <summary>
/// Handles the action after a user is created (e.g., send email).
/// </summary>
public sealed class UserCreatedEventHandler(
    UserManager<ApplicationUser> userManager,
    IEmailService emailService,
    IUserService userService) : INotificationHandler<UserCreatedEvent>
{
    public async Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(notification.User.Id.ToString());

        if (user is null)
        {
            return;
        }

        await emailService.SendAsync(
            "user@yourdomain.com",
            "New User Registered",
            $"A new user was registered: {notification.User.UserName} ({notification.User.Email})",
            true,
            new() { "system@system.com" },
            cancellationToken
        );

        await userService.SendConfirmEmail(user);
    }
}
