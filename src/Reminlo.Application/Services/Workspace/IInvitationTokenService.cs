using Reminlo.Domain.Entities.Identity;
using Reminlo.Domain.Entities.Workspace;

namespace Reminlo.Application.Services.Workspace;

public interface IInvitationTokenService
{
    string GenerateToken();
    bool ValidateToken(string token, WorkspaceInvitation invitation);
    
}