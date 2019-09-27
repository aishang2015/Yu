using System;
using System.Collections.Generic;
using System.Text;
using Yu.Data.Infrasturctures;
using Yu.Data.Infrasturctures.BaseIdentity;

namespace Yu.Data.Entities.WorkFlow
{
    [BelongTo(typeof(BaseIdentityDbContext))]
    public class WorkFlowFormElement : BaseEntity<Guid>
    {
        // 工作流定义ID
        public Guid DefineId { get; set; }

        // dom的id
        public string ElementId { get; set; }

        // 元素类型
        public string Type { get; set; }

        // 宽度
        public int Width { get; set; }

        // 行数
        public int Line { get; set; }

        // 选项
        public string Options { get; set; }
    }
}
