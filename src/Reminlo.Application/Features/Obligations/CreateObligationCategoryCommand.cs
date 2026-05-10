using ErrorOr;
using MediatR;
using Reminlo.Application.Repositories;
using Reminlo.Domain.Entities.Obligation;
using RepositoryKit.Core.Interfaces;

namespace Reminlo.Application.Features.Obligations;

/// <summary>
/// </summary>
public sealed class CreateObligationCategoryCommand : IRequest<ErrorOr<string>>
{
    public string Name { get; set; } = null!;
}

/// <summary>
/// </summary>
internal sealed class CreateObligationCategoryCommandHandler(
    IObligationCategoryRepository obligationCategoryRepository,
    IUnitOfWork unitOfWork
) : IRequestHandler<CreateObligationCategoryCommand, ErrorOr<string>>
{
    public async Task<ErrorOr<string>> Handle(CreateObligationCategoryCommand request, CancellationToken cancellationToken)
    {
        
        var existingCategory = await obligationCategoryRepository.GetAsync(oc => oc.Name == request.Name, cancellationToken);

        if (existingCategory is not null)
        {
            return Error.Conflict("Duplicated" ,$"Category {request.Name} already exists");
        }
        
        var category = ObligationCategory.Create(request.Name);
      
        await obligationCategoryRepository.AddAsync(category, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
       
        return "Category created.";
    }
}