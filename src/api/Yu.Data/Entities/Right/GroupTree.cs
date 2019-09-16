using System;
using Yu.Data.Infrasturctures;
using Yu.Data.Infrasturctures.BaseIdentity;

namespace Yu.Data.Entities.Right
{
    [BelongTo(typeof(BaseIdentityDbContext))]
    public class GroupTree : BaseEntity<Guid>
    {       
        // 祖先
        public Guid Ancestor { get; set; }

        // 后代
        public Guid Descendant { get; set; }

        // 长度
        public int Length { get; set; }
    }
}
