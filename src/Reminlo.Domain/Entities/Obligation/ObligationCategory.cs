using Reminlo.Domain.Abstractions;
using Reminlo.Domain.Common;

namespace Reminlo.Domain.Entities.Obligation;

/// <summary>
/// Represents a category for organizing and grouping obligations within a workspace.
/// Categories help users classify obligations by type or purpose.
/// </summary>
public class ObligationCategory : BaseEntity<ObligationCategoryId>
{
    private ObligationCategory()
    {
    }

    /// <summary>
    /// The name of the category.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// The date and time when this category was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Creates a new obligation category with the specified name.
    /// </summary>
    /// <param name="name">The name of the category.</param>
    /// <returns>A new obligation category instance.</returns>
    public static ObligationCategory Create(string name)
    {
        // TODO only admin validation

        var entity = new ObligationCategory()
        {
            Id = Guid.NewGuid(),
            Name = name,
            CreatedAt = DateTime.UtcNow
        };

        return entity;
    }

    /// <summary>
    /// Updates the category name.
    /// </summary>
    /// <param name="name">The new name of the category.</param>
    public void Update(string name)
    {
        Name = name;
    }
}