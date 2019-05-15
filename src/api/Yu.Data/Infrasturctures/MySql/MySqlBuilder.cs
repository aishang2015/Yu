using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;

namespace Yu.Data.Infrasturctures.MySql
{
    // 添加依赖
    // Pomelo.EntityFrameworkCore.MySql
    public static class MySqlBuilder
    {
        public static DbContextOptionsBuilder UseMySql(DbContextOptionsBuilder optionsBuilder, string connectionString, Action<MySqlDbContextOptionsBuilder> mySqlOptionsAction = null)
        {
            return optionsBuilder.UseMySql(connectionString, mySqlOptionsAction = null);
        }
    }
}
