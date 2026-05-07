using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Reminlo.Application.Features.Identity.Users;
using Reminlo.Application.Repositories;
using Reminlo.Application.Services;
using Reminlo.Application.Services.Identity;
using Reminlo.Application.Services.Workspace;
using Reminlo.Domain.Entities.Identity;
using Reminlo.Domain.Entities.Workspace;
using RepositoryKit.Core.Interfaces;
using ErrorOr;

namespace Reminlo.Application.Features.Workspace;

/// <summary>
/// </summary>
public sealed class CreateWorkspaceCommand : IRequest<ErrorOr<string>>
{
    public string Name { get; set; } = null!;
    public IEnumerable<string> Members { get; set; } = [];
}

/// <summary>
/// </summary>
internal sealed class CreateWorkspaceCommandHandler(
    IUserService userService,
    UserManager<ApplicationUser> userManager,
    IInvitationTokenService invitationTokenService,
    IEmailService emailService,
    IConfiguration configuration,
    IWorkspaceRepository workspaceRepository,
    IEmailQueueService emailQueueService,
    IUnitOfWork unitOfWork
) : IRequestHandler<CreateWorkspaceCommand, ErrorOr<string>>
{
    public async Task<ErrorOr<string>> Handle(CreateWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var userResult = await userService.GetCurrentUserAsync();
        var user = userResult.Value;

        if (userResult.IsError || user?.Id is null)
            return Error.Unauthorized(description: "User is not authenticated.");

        var workspace = Reminlo.Domain.Entities.Workspace.Workspace.Create(user.Id, request.Name);

        ICollection<WorkspaceMember> members = [];
        ICollection<WorkspaceInvitation> invitations = [];

        foreach (var memberEmail in request.Members)
        {
            if (memberEmail == user.Email) continue;

            var invitationToken =  invitationTokenService.GenerateToken();
            var invitation = WorkspaceInvitation.Create(workspace.Id, memberEmail, invitationToken);

            invitations.Add(invitation);
            workspace.AddInvitation(invitation);
        }



        await workspaceRepository.AddAsync(workspace, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);


        foreach (var invitation in invitations)
        {
            emailQueueService.QueueInvitationEmail(invitation.Email, workspace.Name, invitation.Token);
        }

        return "Workspace created and invitations sent.";
    }
}