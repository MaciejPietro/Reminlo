using Reminlo.Application.Features.Identity.Auth;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Reminlo.Api.Abstractions;

namespace Reminlo.Api.Controllers.Identity;

/// <summary>
/// Controller providing authentication and identity-related endpoints.
/// </summary>
public class AuthController(IMediator mediator) : ApiController(mediator)
{
    /// <summary>
    /// Authenticates a user and sets a JWT token as a secure HttpOnly cookie.
    /// Token transport is cookie-only (not via Authorization header) to prevent XSS attacks.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);

        if (response.IsError || response.Value?.token is null)
        {
            return Unauthorized(response.IsError ? response.FirstError.Description : "Invalid credentials.");
        }


        Response.Cookies.Append("accessToken", response.Value.token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            MaxAge = TimeSpan.FromDays(7) // Match JWT expiration
        });

        return Ok();
    }

    /// <summary>
    /// Logs out the user by clearing the HttpOnly access token cookie.
    /// Token transport is cookie-only for XSS protection.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("accessToken");
        return Ok();
    }

    /// <summary>
    /// Confirms a user's email using a confirmation token.
    /// </summary>
    [AllowAnonymous]
    [HttpGet("email/confirm")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string userId, string token, CancellationToken cancellationToken)
    {
        ConfirmEmailCommand request = new(Guid.Parse(userId), token);
        var response = await _mediator.Send(request, cancellationToken);

        return Ok(response.Value);
    }

    /// <summary>
    /// Sends an email confirmation link to the user.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("email/confirmation")]
    public async Task<IActionResult> SendConfirmEmail(SendConfirmEmailCommand request, CancellationToken cancellationToken)
    {
        var response = await _mediator.Send(request, cancellationToken);
        return Ok(response.Value);
    }

    /// <summary>
    /// Sends a password reset email.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("password/reset-request")]
    public async Task<IActionResult> SendResetPasswordEmail([FromBody] SendResetPasswordEmailCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(response.Value);
    }

    /// <summary>
    /// Resets the user's password using the provided token.
    /// </summary>
    [AllowAnonymous]
    [HttpPost("password/reset")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(response.Value);
    }
}
