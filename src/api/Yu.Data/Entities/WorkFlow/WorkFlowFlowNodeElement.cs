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
        public Guid FlowNodeId { get; set; }

        public Guid FormElementId { get; set; }

        // 是否可见
        public bool IsVisible { get; set; }

        // 是否可以编辑
        public bool IsEditable { get; set; }
    }
}
