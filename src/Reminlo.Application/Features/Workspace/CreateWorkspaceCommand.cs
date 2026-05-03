using System.Security.Claims;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Reminlo.Application.Repositories;
using Reminlo.Domain.Entities.Identity;
using Reminlo.Domain.Entities.Workspace;
using RepositoryKit.Core.Interfaces;
using ResultKit;

namespace Reminlo.Application.Features.Workspace;

/// <summary>
/// </summary>
public sealed class CreateWorkspaceCommand : IRequest<Result<string>>
{
    public string Name { get; set; } = null!;
    public IEnumerable<string> Members { get; set; } = [];
}

/// <summary>
/// </summary>
internal sealed class CreateWorkspaceCommandHandler(
    UserManager<ApplicationUser> userManager,
    IHttpContextAccessor httpContextAccessor,
    IWorkspaceRepository workspaceRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<CreateWorkspaceCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CreateWorkspaceCommand request, CancellationToken cancellationToken)
    {
        var loggedUserId = httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

        if (loggedUserId is null)
            return Result<string>.Failure(new Error("401", "User is not authenticated."));

        var workspace = Reminlo.Domain.Entities.Workspace.Workspace.Create(loggedUserId, request.Name);

        ICollection<WorkspaceMember> members = [];
        
            Console.WriteLine("xxxxx");
            Console.WriteLine(request.Members);
        

        foreach (var memberEmail in request.Members)
        {
            var existingUser = await userManager.FindByEmailAsync(memberEmail);
            

            if (existingUser is null) continue;

            var member = WorkspaceMember.Create(existingUser.Id, workspace.Id);
            members.Add(member);
        }

        workspace.Update(null, members);

        await workspaceRepository.AddAsync(workspace, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return "Workspace created.";
    }
}