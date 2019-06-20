using System;
using System.Collections.Generic;
using System.Text;
using Yu.Data.Entities.Enums;
using Yu.Data.Infrasturctures;

namespace Yu.Data.Entities.Front
{
    [BelongTo(typeof(BaseIdentityDbContext))]
    public class Element:BaseEntity<Guid>
    {
        // 元素类型
        public ElementType ElementType { get; set; }

        // 识别（前端通过这个字段来把页面元素和后台数据进行匹配）
        public string Identification { get; set; }
    }
}
