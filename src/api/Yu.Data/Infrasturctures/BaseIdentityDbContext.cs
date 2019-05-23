using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using Yu.Core.Extensions;
using Yu.Data.Entities;

namespace Yu.Data.Infrasturctures
{
    /// <summary>
    /// 认证数据库上下文
    /// </summary>
    /// <remarks>
    /// 继承IdentityDbContext来使用asp net core的identity.并使用主键为GUID的用户和角色.
    /// </remarks>
    public class BaseIdentityDbContext : IdentityDbContext<BaseIdentityUser, BaseIdentityRole, Guid>
    {
        public BaseIdentityDbContext(DbContextOptions<BaseIdentityDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // 配置entity以及configuration
            builder.SetEntityConfiguration(GetType());
        }

    }
}
