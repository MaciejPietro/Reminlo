namespace Reminlo.Application.Services.Workspace;

public interface IEmailQueueService
{
    void QueueInvitationEmail(string email, string workspaceName, string invitationToken);
}