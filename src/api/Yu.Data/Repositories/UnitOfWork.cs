using System.Threading.Tasks;
using Yu.Data.Infrasturctures;

namespace Yu.Data.Repositories
{
    /// <summary>
    /// 工作单元实现
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BaseDbContext _dbContext;

        public UnitOfWork(BaseDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        /// <summary>
        /// 保存scope范围内的数据变更
        /// </summary>
        /// <returns></returns>
        public async Task CommitAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
