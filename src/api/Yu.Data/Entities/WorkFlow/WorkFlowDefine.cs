
using System;
using Yu.Data.Infrasturctures;
using Yu.Data.Infrasturctures.BaseIdentity;

namespace Yu.Data.Entities.WorkFlow
{
    [BelongTo(typeof(BaseIdentityDbContext))]
    public class WorkFlowDefine : BaseEntity<Guid>
    {

        // 名称
        public string Name { get; set; }

        // 类型
        public Guid TypeId { get; set; }

        // 描述
        public string Describe { get; set; }
    }
}

