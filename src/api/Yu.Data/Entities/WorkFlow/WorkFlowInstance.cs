
using System;
using Yu.Data.Infrasturctures;
using Yu.Data.Infrasturctures.BaseIdentity;

namespace Yu.Data.Entities.WorkFlow
{
    /// <summary>
    /// 工作流实例
    /// </summary>
    [BelongTo(typeof(BaseIdentityDbContext))]
    public class WorkFlowInstance : BaseEntity<Guid>
    {

        // 工作流定义ID
        public Guid DefineId { get; set; }

        // 发起人
        public string UserName { get; set; }

        // 当前所在的流程节点ID
        public Guid NodeId { get; set; }

        // 发起日期
        public DateTime OpenDate { get; set; }
    }
}

