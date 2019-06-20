using System;
using Yu.Data.Entities.Enums;
using Yu.Data.Infrasturctures;

namespace Yu.Data.Entities.Right
{
    [BelongTo(typeof(BaseIdentityDbContext))]
    public class Permission : BaseEntity<Guid>
    {
        // 权限的类型
        public PermissionType PermissionType { get; set; }

        // 权限对应的资源
        public Guid ResourceId { get; set; }
    }
}
