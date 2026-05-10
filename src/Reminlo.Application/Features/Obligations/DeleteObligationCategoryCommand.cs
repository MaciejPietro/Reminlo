using ErrorOr;
using MediatR;
using Reminlo.Application.Repositories;
using RepositoryKit.Core.Interfaces;

namespace Reminlo.Application.Features.Obligations;

/// <summary>
/// </summary>
public record DeleteObligationCategoryCommand : IRequest<ErrorOr<string>>
{
    public Guid Id { get; set; }
}

/// <summary>
/// </summary>
internal sealed class DeleteObligationCategoryCommandHandler(
    IObligationCategoryRepository obligationCategoryRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<DeleteObligationCategoryCommand, ErrorOr<string>>
{
    public async Task<ErrorOr<string>> Handle(DeleteObligationCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await obligationCategoryRepository.GetAsync(oc => oc.Id == request.Id, cancellationToken);

        if (category is null)
        {
            return Error.NotFound("ObligationCategory.NotFound", "Obligation category not found.");
        }

        await obligationCategoryRepository.DeleteAsync(category, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return "Category deleted.";
    }
}
