using System;
using Yu.Data.Infrasturctures;
using Yu.Data.Infrasturctures.BaseIdentity;

namespace Yu.Data.Entities.WorkFlow
{
    /// <summary>
    /// 工作流节点
    /// </summary>
    public class WorkFlow_Node : BaseEntity<Guid>
    {
        // 节点名称
        public string Name { get; set; }

        // 节点类型
        public string NodeType { get; set; }

    }
}
