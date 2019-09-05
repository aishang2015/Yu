using System;
using Yu.Data.Infrasturctures;

namespace Yu.Data.Entities.WorkFlow
{
    /// <summary>
    /// 工作流节点
    /// </summary>
    [BelongTo(typeof(BaseIdentityDbContext))]
    public class WorkFlow_Node : BaseEntity<Guid>
    {
        // 节点名称
        public string Name { get; set; }

        // 节点类型
        public string NodeType { get; set; }

    }
}
