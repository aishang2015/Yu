using System;
using Yu.Data.Infrasturctures;

namespace Yu.Data.Entities.WorkFlow
{
    /// <summary>
    /// 表单
    /// </summary>
    [BelongTo(typeof(BaseIdentityDbContext))]
    public class WorkFlow_Form : BaseEntity<Guid>
    {
        // 表单名称
        public string Name { get; set; }

        // 表单分类
        public string Type { get; set; }
    }
}
