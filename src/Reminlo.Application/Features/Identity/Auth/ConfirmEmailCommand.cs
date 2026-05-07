using Reminlo.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ErrorOr;

namespace Reminlo.Application.Features.Identity.Auth;

/// <summary>
/// Command to confirm the email address of a user using a provided confirmation token.
/// </summary>
public sealed record ConfirmEmailCommand(
    Guid UserId,
    string Token
    ) : IRequest<ErrorOr<Unit>>;

/// <summary>
/// Handler that processes the confirmation of a user's email.
/// It verifies the user exists and attempts to confirm the email with the token.
/// Returns success or failure accordingly.
/// </summary>
internal sealed class ConfirmEmailCommandHandler(
    UserManager<ApplicationUser> userManager
) : IRequestHandler<ConfirmEmailCommand, ErrorOr<Unit>>
{
    public async Task<ErrorOr<Unit>> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
            return Error.NotFound(description: "User not found.");

        var result = await userManager.ConfirmEmailAsync(user, request.Token);
        if (!result.Succeeded)
            return Error.Conflict(description: "Email could not be confirmed. The token may be invalid or expired.");

        return Unit.Value;
    }
}
