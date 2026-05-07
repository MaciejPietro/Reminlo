using Reminlo.Application.Services;
using Reminlo.Application.Services.Identity;
using Reminlo.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using ErrorOr;

namespace Reminlo.Application.Features.Identity.Auth;

/// <summary>
/// Command to send an email confirmation link to a user.
/// </summary>
public sealed record SendConfirmEmailCommand(
    Guid UserId
) : IRequest<ErrorOr<string>>;

/// <summary>
/// Handler that generates an email confirmation token and sends it via email to the user.
/// </summary>
internal sealed class SendConfirmEmailCommandHandler(
    UserManager<ApplicationUser> userManager,
    IUserService userService
) : IRequestHandler<SendConfirmEmailCommand, ErrorOr<string>>
{
    public async Task<ErrorOr<string>> Handle(SendConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());

        if (user is null)
            return Error.NotFound(description: "User not found.");

        if (user.Email is null)
            return Error.NotFound(description: "Email address not found.");

        var result = await userService.SendConfirmEmail(user);

        return result;
    }
}
