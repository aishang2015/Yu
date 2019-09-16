
using System;
using Yu.Data.Infrasturctures;
using Yu.Data.Infrasturctures.BaseIdentity;

namespace Yu.Data.Entities.WorkFlow
{
    [BelongTo(typeof(BaseIdentityDbContext))]
    public class WorkFlowType : BaseEntity<Guid>
    {

        // 名称
        public string Name { get; set; }

        // 排序
        public int Order { get; set; }
    }
}

