using System;
using System.Collections.Generic;
using System.Text;
using Yu.Data.Infrasturctures;
using Yu.Data.Infrasturctures.BaseIdentity;

namespace Yu.Data.Entities.WorkFlow
{
    /// <summary>
    /// 工作流实例表单值
    /// </summary>
    [BelongTo(typeof(BaseIdentityDbContext))]
    public class WorkFlowInstanceForm : BaseEntity<Guid>
    {

        // 工作流实例ID
        public Guid InstanceId { get; set; }

        // 元素的ID
        public Guid ElementId { get; set; }

        // 编辑元素的值
        public string Value { get; set; }
    }
}
