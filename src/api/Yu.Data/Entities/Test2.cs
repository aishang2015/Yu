using Yu.Data.Infrasturctures;

namespace Yu.Data.Entities
{
    [BelongTo(typeof(BaseDbContext))]
    public class Test2 : BaseEntity<int>
    {
    }
}
