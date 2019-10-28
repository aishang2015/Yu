using System;
using System.Collections.Generic;
using System.Text;

namespace Yu.Model.WorkFlow.WorkFlowInstance.InputModels
{
    public class WorkFlowInstanceHandleModel
    {
        public Guid InstanceId { get; set; }

        public int HandleStatus { get; set; }

        public string Explain { get; set; }
    }
}
