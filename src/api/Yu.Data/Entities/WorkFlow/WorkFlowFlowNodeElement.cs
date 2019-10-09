using System;
using System.Collections.Generic;
using System.Text;
using Yu.Data.Infrasturctures;
using Yu.Data.Infrasturctures.BaseIdentity;

namespace Yu.Data.Entities.WorkFlow
{
    [BelongTo(typeof(BaseIdentityDbContext))]
    public class WorkFlowFlowNodeElement : BaseEntity<Guid>
    {
        // 工作流定义id
        public Guid DefineId { get; set; }

        // 工作流节点id
        public string FlowNodeId { get; set; }

        // 表单元素ID
        public Guid FormElementId { get; set; }

        // 是否可见
        public bool IsVisible { get; set; }

        // 是否可以编辑
        public bool IsEditable { get; set; }
    }
}
