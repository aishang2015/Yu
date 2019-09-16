using System;
using Yu.Data.Infrasturctures;
using Yu.Data.Infrasturctures.BaseIdentity;

namespace Yu.Data.Entities.Right
{

    [BelongTo(typeof(BaseIdentityDbContext))]
    public class Api : BaseEntity<Guid>
    {
        // 名称
        public string Name { get; set; }

        // 控制器
        public string Controller { get; set; }

        // HTTP访问类型
        public string Type { get; set; }

        // 访问地址
        public string Address { get; set; }
    }
}
