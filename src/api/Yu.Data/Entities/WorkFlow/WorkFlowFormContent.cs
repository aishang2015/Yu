using System;
using System.Collections.Generic;
using System.Text;
using Yu.Data.Infrasturctures;
using Yu.Data.Infrasturctures.BaseIdentity;

namespace Yu.Data.Entities.WorkFlow
{
    [BelongTo(typeof(BaseIdentityDbContext))]
    public class WorkFlowFormContent : BaseEntity<Guid>
    {
        // 工作流定义ID
        public Guid DefineId { get; set; }

        // 编辑表单的表单html
        public string EditFormHtml { get; set; }
    }
}
