using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;

namespace Yu.Data.Infrasturctures.SqlBuilder.SqlServer
{
    public static class SqlServerBuilder
    {
        public static DbContextOptionsBuilder UseSqlServer(DbContextOptionsBuilder optionsBuilder, string connectionString, Action<SqlServerDbContextOptionsBuilder> sqlServerOptionsAction = null)
        {
            return optionsBuilder.UseSqlServer(connectionString, sqlServerOptionsAction = null);
        }
    }
}
