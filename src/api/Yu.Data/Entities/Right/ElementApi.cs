using System;
using Yu.Data.Infrasturctures;
using Yu.Data.Infrasturctures.BaseIdentity;

namespace Yu.Data.Entities.Right
{
    [BelongTo(typeof(BaseIdentityDbContext))]
    public class ElementApi : BaseEntity<Guid>
    {
        // 元素id
        public Guid ElementId { get; set; }

        // api数据id
        public Guid ApiId { get; set; }
    }
}
