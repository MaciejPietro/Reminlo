using Hangfire;
using Microsoft.Extensions.Configuration;
using Reminlo.Application.Services;
using Reminlo.Application.Services.Workspace;

namespace Reminlo.Infrastructure.Services.Workspace;

public class EmailQueueService(
    IBackgroundJobClient backgroundJobClient,
    IConfiguration configuration,
    IEmailService emailService) : IEmailQueueService
{
    public void QueueInvitationEmail(string email, string workspaceName, string invitationToken)
    {
        var baseUrl = configuration["App:BaseUrl"];
        var invitationLink = $"{baseUrl}/api/workspace/invitation?email={email}&token={invitationToken}";

        var subject = "Invitation to workspace";
        var body =
            $"You have been invited to {workspaceName} workspace, to join <a href='{invitationLink}'>click here</a>.";

        backgroundJobClient.Enqueue(() => SendInvitationEmailAsync(email, subject, body));
    }

    public async Task SendInvitationEmailAsync(string email, string subject, string body)
    {
        await emailService.SendAsync(email, subject, body);
    }
}
