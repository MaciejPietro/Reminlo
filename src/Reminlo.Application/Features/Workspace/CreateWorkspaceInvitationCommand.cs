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
using Reminlo.Domain.Common;
using Reminlo.Domain.Entities.Identity;
using Reminlo.Domain.Entities.Workspace;
using RepositoryKit.Core.Interfaces;
using ResultKit;

namespace Reminlo.Application.Features.Workspace;

/// <summary>
/// </summary>
public sealed class CreateWorkspaceInvitationCommand : IRequest<Result<string>>
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
    IUnitOfWork unitOfWork
) : IRequestHandler<CreateWorkspaceInvitationCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CreateWorkspaceInvitationCommand request, CancellationToken cancellationToken)
    {

        var workspace = await workspaceRepository.GetAsync(x => x.Id == request.WorkspaceId, cancellationToken);
        var invitationToken =  invitationTokenService.GenerateToken();
        
        var invitation = WorkspaceInvitation.Create(request.WorkspaceId, request.Email, invitationToken);
        
        workspace!.AddInvitation(invitation);
        
        await unitOfWork.SaveChangesAsync(cancellationToken);
        
        emailQueueService.QueueInvitationEmail(request.Email, workspace!.Name, invitation.Token);

        return "Workspace invitation sent.";
    }
}