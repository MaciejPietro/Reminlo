using ErrorOr;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Reminlo.Application.Repositories;
using Reminlo.Application.Services;
using Reminlo.Application.Services.Identity;
using Reminlo.Application.Services.Workspace;
using Reminlo.Domain.Entities.Identity;
using Reminlo.Domain.Entities.Obligation;
using Reminlo.Domain.Entities.Workspace;
using RepositoryKit.Core.Interfaces;

namespace Reminlo.Application.Features.Obligations;

/// <summary>
/// </summary>
public sealed class GetObligationCategoriesQuery : IRequest<ErrorOr<List<ObligationCategory>>>;


/// <summary>
/// </summary>
internal sealed class GetObligationCategoriesQueryHandler(
    IObligationCategoryRepository obligationCategoryRepository
) : IRequestHandler<GetObligationCategoriesQuery, ErrorOr<List<ObligationCategory>>>
{
    public async Task<ErrorOr<List<ObligationCategory>>> Handle(GetObligationCategoriesQuery request, CancellationToken cancellationToken)
    {
        var existingCategory = await obligationCategoryRepository.GetAllAsync(cancellationToken: cancellationToken);
       
        return existingCategory.ToList();
    }
}