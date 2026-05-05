using System.Security.Claims;
using Reminlo.Application.Services;
using Reminlo.Application.Services.Identity;
using Reminlo.Domain.Entities.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using ResultKit;

namespace Reminlo.Infrastructure.Services.Identity;

/// <summary>
/// Service responsible for user-related operations such as email confirmation token generation and sending confirmation emails.
/// </summary>
internal sealed class UserService(
    UserManager<ApplicationUser> userManager,
    IEmailService emailService,
    IConfiguration configuration,
    IHttpContextAccessor httpContextAccessor) : IUserService
{
    /// <summary>
    /// Generates an email confirmation token for the specified user.
    /// </summary>
    public async Task<Result<string>> GetConfirmEmailToken(ApplicationUser user)
    {
        var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
        return token;
    }

    /// <summary>
    /// Sends an email confirmation message to the specified user.
    /// </summary>
    public async Task<string> SendConfirmEmail(ApplicationUser user)
    {
        var token = GetConfirmEmailToken(user).Result.Value;

        var baseUrl = configuration["App:BaseUrl"];
        var confirmationLink = $"{baseUrl}/api/email/confirm?userId={user.Id}&token={Uri.EscapeDataString(token!)}";

        await emailService.SendAsync(
            user.Email!,
            "Email Confirmation",
            $"To confirm your email address, please <a href='{confirmationLink}'>click here</a>.", true
        );

        return "Email confirmation message has been sent successfully.";
    }

    /// <summary>
    /// Gets the currently authenticated user from the HTTP context.
    /// </summary>
    public async Task<Result<ApplicationUser>> GetCurrentUserAsync()
    {
        var userName = httpContextAccessor.HttpContext?.User?.Identity?.Name;

        if (string.IsNullOrEmpty(userName))
        {
            return Result<ApplicationUser>.Failure(new Error(ErrorCodes.Unauthorized, "User is not authenticated"));
        }

        var user = await userManager.FindByNameAsync(userName);

        if (user is null)
        {
            return Result<ApplicationUser>.Failure(new Error(ErrorCodes.NotFound, "User not found"));
        }

        return Result<ApplicationUser>.Success(user);
    }

    public Result<string> GetCurrentUserId()
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Anonymous";

        return Result<string>.Success(userId);
    }
}
