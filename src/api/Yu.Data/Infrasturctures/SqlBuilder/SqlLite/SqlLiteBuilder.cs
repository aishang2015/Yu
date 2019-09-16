using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;

namespace Yu.Data.Infrasturctures.SqlBuilder.SqlLite
{
    // 相关依赖
    // Microsoft.EntityFrameworkCore.Sqlite
    public class SqlLiteBuilder
    {
        public static DbContextOptionsBuilder UseSqlLite(DbContextOptionsBuilder optionsBuilder, string connectionString, Action<SqliteDbContextOptionsBuilder> sqliteOptionsAction = null)
        {
            return optionsBuilder.UseSqlite(connectionString, sqliteOptionsAction = null);
        }
    }
}
