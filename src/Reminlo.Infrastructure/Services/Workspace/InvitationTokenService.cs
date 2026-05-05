using Microsoft.AspNetCore.Identity;
using Reminlo.Application.Services.Workspace;
using Reminlo.Domain.Entities.Identity;
using Reminlo.Domain.Entities.Workspace;
using Reminlo.Domain.Enums;

namespace Reminlo.Infrastructure.Services.Workspace;

public class InvitationTokenService() : IInvitationTokenService
{
    public string GenerateToken()
    {
        var token = Guid.NewGuid().ToString("N");
        
        return token;
    }


    public bool ValidateToken(string token, WorkspaceInvitation invitation)
    {
        
        var isValid = invitation.Token == token &&
                      invitation.ExpiresAt > DateTime.UtcNow &&
                      invitation.Status == InvitationStatus.Pending;
        
        return isValid;
    }
}