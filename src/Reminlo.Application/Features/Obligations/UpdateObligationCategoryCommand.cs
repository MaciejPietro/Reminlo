using ErrorOr;
using MediatR;
using Reminlo.Application.Repositories;
using RepositoryKit.Core.Interfaces;

namespace Reminlo.Application.Features.Obligations;

/// <summary>
/// </summary>
public record UpdateObligationCategoryCommand : IRequest<ErrorOr<string>>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
}

/// <summary>
/// </summary>
internal sealed class UpdateObligationCategoryCommandHandler(
    IObligationCategoryRepository obligationCategoryRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<UpdateObligationCategoryCommand, ErrorOr<string>>
{
    public async Task<ErrorOr<string>> Handle(UpdateObligationCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await obligationCategoryRepository.GetAsync(oc => oc.Id == request.Id, cancellationToken);

        if (category is null)
        {
            return Error.NotFound("ObligationCategory.NotFound", "Obligation category not found.");
        }
        
      
        var existingCategory = await obligationCategoryRepository.GetAsync(oc => oc.Name == request.Name, cancellationToken);
        
        

        if (existingCategory is not null)
        {
            return Error.Conflict("Duplicated", $"Category {request.Name} already exists");
        }

        category.Update(request.Name);

        await obligationCategoryRepository.UpdateAsync(category, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return "Category updated.";
    }
}
