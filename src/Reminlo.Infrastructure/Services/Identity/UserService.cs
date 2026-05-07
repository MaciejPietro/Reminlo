using System.Security.Claims;
using Reminlo.Application.Services;
using Reminlo.Application.Services.Identity;
using Reminlo.Domain.Entities.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using ErrorOr;

namespace Reminlo.Infrastructure.Services.Identity;

/// <summary>
/// Service responsible for user-related operations such as email confirmation token generation and sending confirmation emails.
/// </summary>
internal sealed class UserService(
    UserManager<ApplicationUser> userManager,
    IEmailService emailService,
    IConfiguration configuration,
    ICacheService cacheService,
    IHttpContextAccessor httpContextAccessor) : IUserService
{
    /// <summary>
    /// Generates an email confirmation token for the specified user.
    /// </summary>
    public async Task<ErrorOr<string>> GetConfirmEmailToken(ApplicationUser user)
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
    public async Task<ErrorOr<ApplicationUser>> GetCurrentUserAsync()
    {
        var userName = httpContextAccessor.HttpContext?.User?.Identity?.Name;

        if (string.IsNullOrEmpty(userName))
        {
            return Error.Unauthorized(description: "User is not authenticated");
        }

        var user = await userManager.FindByNameAsync(userName);

        if (user is null)
        {
            return Error.NotFound(description: "User not found");
        }

        return user;
    }

    public ErrorOr<string> GetCurrentUserId()
    {
        var userId = httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "Anonymous";

        return userId;
    }

    public bool HasCurrentUserRole(string role)
    {
        return (bool) httpContextAccessor.HttpContext?.User?.IsInRole(role);

        // if (roles is null) return false;
        //
        // var hasRole = roles.Contains(role);
        //
        // return  hasRole ? true : false;
    }
}

