using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Yu.Data.Repositories;

namespace Yu.Data.Infrasturctures.BaseIdentity
{
    public class BaseIdentityUnitOfWork : UnitOfWork<BaseIdentityDbContext>
    {
        public BaseIdentityUnitOfWork(BaseIdentityDbContext dbContext) : base(dbContext)
        {
        }
    }
}
