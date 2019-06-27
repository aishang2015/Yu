using System;
using Yu.Data.Infrasturctures;

namespace Yu.Data.Entities.Right
{
    [BelongTo(typeof(BaseIdentityDbContext))]
    public class RuleGroup : BaseEntity<Guid>
    {
        // 规则组名称
        public string Name { get; set; }
    }
}
