using Microsoft.EntityFrameworkCore;

namespace Yu.Data.Infrasturctures
{
    /// <summary>
    /// 普通数据库上下文
    /// </summary>
    public class BaseDbContext : DbContext
    {
        public BaseDbContext(DbContextOptions<BaseDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 配置entity以及configuration
            modelBuilder.SetEntityConfiguration(GetType());
        }
    }
}
