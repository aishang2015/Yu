using System;
using Yu.Data.Entities.Enums;
using Yu.Data.Infrasturctures;
using Yu.Data.Infrasturctures.BaseIdentity;

namespace Yu.Data.Entities.Right
{
    [BelongTo(typeof(BaseIdentityDbContext))]
    public class Element : BaseEntity<Guid>
    {
        // 元素名称
        public string Name { get; set; }

        // 元素类型
        public ElementType ElementType { get; set; }

        // 识别（前端通过这个字段来把页面元素和后台数据进行匹配）
        public string Identification { get; set; }

        // 路由（仅在菜单的情况下）
        public string Route { get; set; }
    }
}
