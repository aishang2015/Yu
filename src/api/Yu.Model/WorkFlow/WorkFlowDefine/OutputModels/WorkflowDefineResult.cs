using System;
using System.Collections.Generic;
using System.Text;

namespace Yu.Model.WorkFlow.WorkFlowDefine.OutputModels
{
    public class WorkflowDefineResult
    {
        // ID
        public Guid Id { get; set; }

        // 名称
        public string Name { get; set; }

        // 类型Id
        public Guid TypeId { get; set; }

        // 类型名称
        public string TypeName { get; set; }

        // 描述
        public string Describe { get; set; }
    }
}
