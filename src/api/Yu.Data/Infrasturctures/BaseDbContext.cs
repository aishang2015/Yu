using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using Yu.Core.Extensions;
using Yu.Data.Repositories;

namespace Yu.Data.Infrasturctures
{
    /// <summary>
    /// 数据库上下文
    /// </summary>
    /// <remarks>
    /// 继承IdentityDbContext来使用asp net core的identity.并使用主键为GUID的用户和角色.
    /// </remarks>
    public class BaseDbContext : IdentityDbContext<BaseUser<Guid>, BaseRole<Guid>, Guid>
    {
        public BaseDbContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // 查找所有对baseEntity的实现
            var typeList = typeof(BaseEntity<>).GetAllChildType();
            typeList.ForEach(type => builder.Entity(type));
        }

    }
}
