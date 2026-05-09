using Reminlo.Domain.Abstractions;
using Reminlo.Domain.Common;

namespace Reminlo.Domain.Entities.Obligation;

public class ObligationCategory : BaseEntity<ObligationCategoryId>
{
    private ObligationCategory()
    {
    }
    
    public string Name {  get; private set; }
    
    public DateTime CreatedAt { get; set; }

    public static ObligationCategory Create(string name)
    {
        // TODO only admin validation
        
        var entity = new ObligationCategory()
        {
            Name = name
        };

        return entity;
    }
}