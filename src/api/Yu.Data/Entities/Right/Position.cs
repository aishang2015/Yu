
using System;
using Yu.Data.Infrasturctures;
using Yu.Data.Infrasturctures.BaseIdentity;

namespace Yu.Data.Entities.WebAdmin
{
    [BelongTo(typeof(BaseIdentityDbContext))]
    public class Position : BaseEntity<Guid>
    {

        // 岗位名称
        public string PositionName { get; set; }
    }
}

