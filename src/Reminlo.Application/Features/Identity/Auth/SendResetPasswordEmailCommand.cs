using Reminlo.Application.Services;
using Reminlo.Domain.Entities.Identity;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using ErrorOr;

namespace Reminlo.Application.Features.Identity.Auth;

/// <summary>
/// Command to request a password reset link for a user identified by email.
/// </summary>
public sealed record SendResetPasswordEmailCommand(
    string Email) : IRequest<ErrorOr<string>>;

/// <summary>
/// Handler that generates a password reset token and sends it via email.
/// </summary>
internal sealed class ResetPasswordRequestCommandHandler(
    UserManager<ApplicationUser> userManager,
    IEmailService emailService,
    IConfiguration configuration) : IRequestHandler<SendResetPasswordEmailCommand, ErrorOr<string>>
{
    public async Task<ErrorOr<string>> Handle(SendResetPasswordEmailCommand request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user == null)
            return Error.NotFound(description: "User not found.");

        var token = await userManager.GeneratePasswordResetTokenAsync(user);

        // Make token URL-safe
        var urlToken = Uri.EscapeDataString(token);

        var baseUrl = configuration["App:BaseUrl"];
        var resetLink = $"{baseUrl}/api/auth/resetpassword?userId={user.Id}&token={urlToken}";

        await emailService.SendAsync(
            user.Email!,
            "Password Reset",
            $"To reset your password, please <a href='{resetLink}'>click here</a>.",
            true
        );

        return "Password reset email has been sent successfully.";
    }
}
