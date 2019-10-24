using System;
using System.Collections.Generic;
using System.Text;

namespace Yu.Model.WorkFlow.WorkFlowInstance.OutputModels
{
    public class WorkFlowInstanceNodeResult
    {
        public string NodeName { get; set; }

        public string HandlePeoples { get; set; }

        public string HandlePeopleNames { get; set; }

        public int HandleStatus { get; set; }

        public string Explain { get; set; }

        public DateTime HandleDateTime { get; set; }
    }
}
