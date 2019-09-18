using System;
using System.Collections.Generic;
using System.Text;
using Yu.Data.Entities.WorkFlow;

namespace Yu.Model.WorkFlow.WorkFlowFlow
{
    public class WorkFlowFlowViewModel
    {
        public List<WorkFlowFlowNode> Nodes { get; set; }

        public List<WorkFlowFlowConnection> Connections { get; set; }

        public Guid DefineId { get; set; }
    }
}
