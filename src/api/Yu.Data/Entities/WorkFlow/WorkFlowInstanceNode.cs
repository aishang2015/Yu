
using System;
using Yu.Data.Infrasturctures;
using Yu.Data.Infrasturctures.BaseIdentity;

namespace Yu.Data.Entities.WorkFlow
{
    [BelongTo(typeof(BaseIdentityDbContext))]
    public class WorkFlowInstanceNode : BaseEntity<Guid>
    {
        // 工作流实例ID
        public Guid InstanceId { get; set; }

        // 流程节点ID
        public Guid NodeId { get; set; }

        // 处理人ID
        public Guid HandlePeopleID { get; set; }
        
        // 处理结果
        // 1.暂存
        // 2.拒绝
        // 3.同意
        public int HandleResult { get; set; }

        // 处理说明
        public string Explain { get; set; }

        // 处理时间
        public DateTime HandleDateTime { get; set; }
    }
}

