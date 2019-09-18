
using System;
using Yu.Data.Infrasturctures;
using Yu.Data.Infrasturctures.BaseIdentity;

namespace Yu.Data.Entities.WorkFlow
{
    [BelongTo(typeof(BaseIdentityDbContext))]
    public class WorkFlowFlowConnection : BaseEntity<Guid>
    {
        // 工作流定义ID
        public Guid DefineId { get; set; }

        // 源节点ID
        public string SourceId { get; set; }

        // 目标节点ID
        public string TargetId { get; set; }
    }
}

