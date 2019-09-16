using System;
using System.Collections.Generic;
using System.Text;
using Yu.Data.Infrasturctures;
using Yu.Data.Infrasturctures.BaseIdentity;

namespace Yu.Data.Entities.Right
{
    [BelongTo(typeof(BaseIdentityDbContext))]
    public class Group : BaseEntity<Guid>
    {

        // 组织名称
        public string GroupName { get; set; }

        // 备注信息
        public string Remark { get; set; }
    }
}
