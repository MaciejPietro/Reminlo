using ErrorOr;
using MediatR;
using Reminlo.Application.Repositories;
using Reminlo.Domain.Common;
using Reminlo.Domain.Entities.Obligation;
using Reminlo.Domain.Entities.Workspace;
using Reminlo.Domain.Enums.Obligations;
using RepositoryKit.Core.Interfaces;

namespace Reminlo.Application.Features.Obligations;

/// <summary>
/// </summary>
public sealed record CreateObligationCommand : IRequest<ErrorOr<string>>
{
    public WorkspaceId WorkspaceId { get; init; }
    public ObligationCategoryId CategoryId { get; init; }

    public string StartDate { get; init; }

    public string? EndDate { get; init; }

    public string Name { get; init; } = null!;

    public string? Description { get; init; }

    public ObligationFrequency? FrequencyInterval { get; init; }
    public int? FrequencyValue { get; init; }

    public ICollection<WorkspaceMemberId>? VisibleTo { get; init; }

    public ObligationPriority Priority { get; init; }
}

/// <summary>
/// </summary>
internal sealed class CreateObligationCommandHandler(
    IObligationRepository obligationRepository,
    IWorkspaceMemberRepository workspaceMemberRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<CreateObligationCommand, ErrorOr<string>>
{
    public async Task<ErrorOr<string>> Handle(CreateObligationCommand request, CancellationToken cancellationToken)
    {

        if (!DateTime.TryParse(request.StartDate, out var startDate))
        {
            return Error.Validation("Date", "Start date must be a valid date format");
        }
        startDate = DateTime.SpecifyKind(startDate, DateTimeKind.Utc);

        DateTime? endDate = null;
        if (!string.IsNullOrEmpty(request.EndDate))
        {
            if (!DateTime.TryParse(request.EndDate, out var parsedEndDate))
            {
                return Error.Validation("Date", "End date must be a valid date format");
            }
            endDate = DateTime.SpecifyKind(parsedEndDate, DateTimeKind.Utc);
        }

        ICollection<WorkspaceMember>? members = null;

        if (request.VisibleTo is not null && request.VisibleTo.Any())
        {
            members = (await workspaceMemberRepository.GetAllAsync(wm => request.VisibleTo.Contains(wm.Id) && request.WorkspaceId == wm.WorkspaceId, cancellationToken)).ToList();
        }

        var obligation = Obligation.Create(
            request.WorkspaceId,
            request.CategoryId,
            request.Name,
            startDate,
            request.Description,
            request.FrequencyInterval,
            request.FrequencyValue,
            endDate,
            members,
            request.Priority);
        
        await obligationRepository.AddAsync(obligation, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
       
        return "Obligation created.";
    }
}