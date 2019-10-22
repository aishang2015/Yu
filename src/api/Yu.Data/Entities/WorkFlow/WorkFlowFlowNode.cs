
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

        // 节点domID
        public string NodeId { get; set; }

        // 节点类型
        public string NodeType { get; set; }

        // 上边距
        public string Top { get; set; }

        // 左边距
        public string Left { get; set; }

        //----------------------------基本信息----------------------------
        // 工作节点名称
        public string Name { get; set; }

        // 工作节点描述
        public string Describe { get; set; }

        //----------------------------办理方式----------------------------
        // 办理方式类型
        // 1指定人员办理
        // 2指定岗位办理
        public int HandleType { get; set; }

        // 办理人id，用【，】隔开
        public string HandlePeoples { get; set; }

        // 岗位id
        public string PositionId { get; set; }

        // 岗位所属部门
        // 1不指定部门
        // 2发起人部门
        // 3发起人上级部门
        public int PositionGroup { get; set; }
        
    }
}

