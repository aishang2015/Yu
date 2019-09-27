using System;
using System.Collections.Generic;
using System.Text;
using Yu.Data.Entities.WorkFlow;

namespace Yu.Model.WorkFlow.WorkFlowForm
{
    public class WorkFlowFormViewModel
    {
        public WorkFlowFormContent FormContent { get; set; }

        public List<WorkFlowFormElement> FormElements { get; set; }

        public Guid DefineId { get; set; }
    }
}
