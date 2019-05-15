using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;

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
        }
    }
}
