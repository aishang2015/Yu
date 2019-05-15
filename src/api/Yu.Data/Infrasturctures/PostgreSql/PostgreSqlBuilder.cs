using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using System;

namespace Yu.Data.Infrasturctures.PostgreSql
{
    // 相关依赖
    // Npgsql.EntityFrameworkCore.PostgreSQL            PostgreSQL数据提供的支持EF Core的基础类库
    // Npgsql.EntityFrameworkCore.PostgreSQL.Design     使用Guid（对应Postgre数据的类型为uuid）类型的主键必须
    public class PostgreSqlBuilder
    {
        public static DbContextOptionsBuilder UsePostgreSql(DbContextOptionsBuilder optionsBuilder, string connectionString, Action<NpgsqlDbContextOptionsBuilder> npgsqlDbContextOptionsBuilder = null)
        {
            return optionsBuilder.UseNpgsql(connectionString, npgsqlDbContextOptionsBuilder = null);
        }
    }
}
