using System;
using Yu.Data.Infrasturctures;
using Yu.Data.Infrasturctures.BaseIdentity;

namespace Yu.Data.Entities.WorkFlow
{
    /// <summary>
    /// 表单
    /// </summary>
    public class WorkFlow_Form : BaseEntity<Guid>
    {
        // 表单名称
        public string Name { get; set; }

        // 表单分类
        public string Type { get; set; }
    }
}
