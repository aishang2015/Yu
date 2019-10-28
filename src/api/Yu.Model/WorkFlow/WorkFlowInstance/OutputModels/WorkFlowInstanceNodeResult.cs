using System;
using System.Collections.Generic;
using System.Text;

namespace Yu.Model.WorkFlow.WorkFlowInstance.OutputModels
{
    public class WorkFlowInstanceNodeResult
    {
        public string NodeName { get; set; }

        public string HandlePeople { get; set; }

        public string HandlePeopleName { get; set; }

        public int HandleStatus { get; set; }

        public string Explain { get; set; }

        public DateTime HandleDateTime { get; set; }
    }
}
