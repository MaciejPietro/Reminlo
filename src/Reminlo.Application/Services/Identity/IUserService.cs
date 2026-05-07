using Reminlo.Domain.Entities.Identity;
using ErrorOr;

namespace Reminlo.Application.Services.Identity
{
    /// <summary>
    /// Defines user-related service operations such as generating email confirmation tokens and sending confirmation emails.
    /// </summary>
    public interface IUserService
    {
        Task<ErrorOr<string>> GetConfirmEmailToken(ApplicationUser user);
        Task<string> SendConfirmEmail(ApplicationUser user);
        Task<ErrorOr<ApplicationUser>> GetCurrentUserAsync();

        ErrorOr<string> GetCurrentUserId();

        bool HasCurrentUserRole(string role);
    }
}
