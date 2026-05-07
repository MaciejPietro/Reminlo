using MediatR;
using Reminlo.Application.Repositories;
using Reminlo.Application.Services.Identity;
using Reminlo.Application.Services.Workspace;
using Reminlo.Domain.Common;
using Reminlo.Domain.Entities.Workspace;
using RepositoryKit.Core.Interfaces;
using ErrorOr;

namespace Reminlo.Application.Features.Workspace;

/// <summary>
/// </summary>
public sealed class CreateWorkspaceInvitationCommand : IRequest<ErrorOr<string>>
{
    public WorkspaceId WorkspaceId { get; set; }
    public string Email { get; set; } = null!;
}

/// <summary>
/// </summary>
internal sealed class CreateWorkspaceInvitationCommandHandler(
    IInvitationTokenService invitationTokenService,
    IWorkspaceRepository workspaceRepository,
    IEmailQueueService emailQueueService,
    IUserService userService,
    IUnitOfWork unitOfWork
) : IRequestHandler<CreateWorkspaceInvitationCommand, ErrorOr<string>>
{
    public async Task<ErrorOr<string>> Handle(CreateWorkspaceInvitationCommand request, CancellationToken cancellationToken)
    {

        var userResult = await userService.GetCurrentUserAsync();
        var user = userResult.Value;


        var userId = user!.Id;
        var workspace = await workspaceRepository.GetAsync(x => x.Id == request.WorkspaceId, cancellationToken);

        if (workspace is null)
        {
            return Error.NotFound(description: "Workspace not found.");
        }

        if (workspace!.OwnerId != userId)
        {
            return Error.Forbidden(description: "You are not authorized to invite new members.");
        }

        if (request.Email == user!.Email)
        {
            return Error.Validation(code: "InvalidEmail", description: "You can't invite yourself.");
        }

        var invitationToken =  invitationTokenService.GenerateToken();

        var invitation = WorkspaceInvitation.Create(request.WorkspaceId, request.Email, invitationToken);

        workspace!.AddInvitation(invitation);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        emailQueueService.QueueInvitationEmail(request.Email, workspace!.Name, invitation.Token);

        return "Workspace invitation sent.";
    }
}