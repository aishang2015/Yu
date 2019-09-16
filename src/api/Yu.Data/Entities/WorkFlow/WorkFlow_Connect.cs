using System;
using System.Collections.Generic;
using System.Text;
using Yu.Data.Infrasturctures;
using Yu.Data.Infrasturctures.BaseIdentity;

namespace Yu.Data.Entities.WorkFlow
{
    /// <summary>
    /// 节点流转路径
    /// </summary>
    [BelongTo(typeof(BaseIdentityDbContext))]
    public class WorkFlow_Connect : BaseEntity<Guid>
    {
        // 路径名称
        public string Name { get; set; }

        // 开始节点
        public Guid StartNode { get; set; }

        // 结束节点
        public Guid EndNode { get; set; }
    }
}
