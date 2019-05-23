using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Yu.Data.Repositories
{
    /// <summary>
    /// 工作单元定义
    /// </summary>
    public interface IUnitOfWork<TDbContext> where TDbContext : DbContext
    {
        Task CommitAsync();
    }
}
