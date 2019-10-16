using System;
using System.Collections.Generic;
using System.Text;
using Yu.Data.Entities.WorkFlow;

namespace Yu.Model.WorkFlow.WorkFlowInstance.InputModels
{
    public class WorkFlowInstanceFormInputViewModel
    {
        public Guid InstanceId { get; set; }

        public List<WorkFlowInstanceForm> WorkFlowInstanceForms { get; set; }
    }
}
