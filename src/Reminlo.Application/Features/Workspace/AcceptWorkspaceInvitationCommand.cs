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
using Reminlo.Domain.Enums;
using RepositoryKit.Core.Interfaces;
using ResultKit;

namespace Reminlo.Application.Features.Workspace;

/// <summary>
/// </summary>
public sealed class AcceptWorkspaceInvitationCommand : IRequest<Result<string>>
{
    public string Token { get; set; } = null!;
    public string Email { get; set; } = null!;
}

/// <summary>
/// </summary>
internal sealed class AcceptWorkspaceInvitationCommandHandler(
    UserManager<ApplicationUser> userManager,
    IInvitationTokenService invitationTokenService,
    IWorkspaceRepository workspaceRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<AcceptWorkspaceInvitationCommand, Result<string>>
{
    public async Task<Result<string>> Handle(AcceptWorkspaceInvitationCommand request, CancellationToken cancellationToken)
    {
        var genericFailure = Result<string>.ValidationFailure([
            new ValidationError("Token", "Something went wrong.")
        ]);
        
        var invitation = await workspaceRepository.GetInvitationAsync(request.Token);
        var existingUser = await userManager.FindByEmailAsync(request.Email);

        var isEmailValid = invitation.Email == request.Email;
        var isTokenValid = invitationTokenService.ValidateToken(request.Token, invitation);
        
        if (!isEmailValid || !isTokenValid || existingUser is null)
        {
            return genericFailure;
        }
        
        invitation.SetStatus(InvitationStatus.Accepted);
        

        var member = WorkspaceMember.Create(existingUser.Id, invitation.WorkspaceId);

        var workspace = workspaceRepository.GetAsync((x) => x.Id == invitation.WorkspaceId, cancellationToken).Result;

        if (workspace is null)
        {
            return genericFailure;
        }
        
        workspace.AddMember(member);
       
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return "Workspace invitation was accepted.";
    }
}