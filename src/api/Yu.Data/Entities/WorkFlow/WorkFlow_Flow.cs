using System;
using Yu.Data.Infrasturctures;

namespace Yu.Data.Entities.WorkFlow
{
    /// <summary>
    /// 流程定义
    /// </summary>
    [BelongTo(typeof(BaseIdentityDbContext))]
    public class WorkFlow_Flow : BaseEntity<Guid>
    {
        // 名称
        public string Name { get; set; }

        // 表单
        public Guid FormId { get; set; }

        // 类型
        public string Type { get; set; }

        // 备注
        public string Remark { get; set; }

        // 创建者
        public string Creator { get; set; }

        // 创建时间
        public DateTime CreateTime { get; set; }

        // 更新者
        public string Updater { get; set; }

        // 更新时间
        public DateTime UpdateTime { get; set; }

    }
}
