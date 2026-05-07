using Reminlo.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using ErrorOr;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Reminlo.Application.Features.Identity.Auth;

/// <summary>
/// Command to reset a user's password using a reset token and a new password.
/// </summary>
public sealed record ResetPasswordCommand(Guid UserId, string Token, string NewPassword) : IRequest<ErrorOr<Success>>;

/// <summary>
/// Handler that processes resetting the password of a user.
/// It decodes the token, performs the reset operation,
/// and returns success or validation error messages.
/// </summary>
internal sealed class ResetPasswordCommandHandler(
    UserManager<ApplicationUser> userManager) : IRequestHandler<ResetPasswordCommand, ErrorOr<Success>>
{
    public async Task<ErrorOr<Success>> Handle(ResetPasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByIdAsync(request.UserId.ToString());
        if (user == null)
            return Error.NotFound(description: "User not found.");

        // The token should be decoded if it comes from the URL
        var decodedToken = Uri.UnescapeDataString(request.Token);

        var result = await userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);

        if (!result.Succeeded)
            return Error.Validation(code: "PasswordResetFailed", description: "Password could not be reset: " + string.Join(", ", result.Errors.Select(x => x.Description)));

        return Result.Success;
    }
}
