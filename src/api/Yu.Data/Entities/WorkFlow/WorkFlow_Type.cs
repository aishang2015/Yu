using System;
using Yu.Data.Infrasturctures;
using Yu.Data.Infrasturctures.BaseIdentity;

namespace Yu.Data.Entities.WorkFlow
{
    /// <summary>
    /// 工作流类型
    /// </summary>
    [BelongTo(typeof(BaseIdentityDbContext))]
    public class WorkFlow_Type : BaseEntity<Guid>
    {
        // 类型名称
        public string Name { get; set; }

        // 排序
        public int OrderNumber { get; set; }

    }
}
