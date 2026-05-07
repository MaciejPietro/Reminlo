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
using ErrorOr;

namespace Reminlo.Application.Features.Workspace;

/// <summary>
/// </summary>
public sealed class RejectWorkspaceInvitationCommand : IRequest<ErrorOr<string>>
{
    public string Token { get; set; } = null!;
    public string Email { get; set; } = null!;
}

/// <summary>
/// </summary>
internal sealed class RejectWorkspaceInvitationCommandHandler(
    UserManager<ApplicationUser> userManager,
    IInvitationTokenService invitationTokenService,
    IWorkspaceRepository workspaceRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<RejectWorkspaceInvitationCommand, ErrorOr<string>>
{
    public async Task<ErrorOr<string>> Handle(RejectWorkspaceInvitationCommand request, CancellationToken cancellationToken)
    {
        var genericFailure = Error.Validation(code: "Failure", description: "Something went wrong.");

        var invitation = await workspaceRepository.GetInvitationAsync(request.Token);
        var existingUser = await userManager.FindByEmailAsync(request.Email);

        var isEmailValid = invitation.Email == request.Email;
        var isTokenValid = invitationTokenService.ValidateToken(request.Token, invitation);

        if (!isEmailValid || !isTokenValid || existingUser is null)
        {
            return genericFailure;
        }

        invitation.SetStatus(InvitationStatus.Rejected);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return "Workspace invitation was rejected.";
    }
}