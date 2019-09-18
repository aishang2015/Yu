
using System;
using Yu.Data.Infrasturctures;
using Yu.Data.Infrasturctures.BaseIdentity;

namespace Yu.Data.Entities.WorkFlow
{
    [BelongTo(typeof(BaseIdentityDbContext))]
    public class WorkFlowFlowNode : BaseEntity<Guid>
    {

        // 工作流定义
        public Guid DefineId { get; set; }

        // 节点ID
        public string NodeId { get; set; }

        // 节点类型
        public string NodeType { get; set; }

        // 上边距
        public string Top { get; set; }

        // 左边距
        public string Left { get; set; }
    }
}

