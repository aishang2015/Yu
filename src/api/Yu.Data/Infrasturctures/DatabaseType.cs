using Microsoft.EntityFrameworkCore;
using Yu.Data.Infrasturctures.MySql;
using Yu.Data.Infrasturctures.PostgreSql;
using Yu.Data.Infrasturctures.SqlLite;
using Yu.Data.Infrasturctures.SqlServer;

namespace Yu.Data.Infrasturctures
{
    /// <summary>
    /// 可配置的数据库类型
    /// </summary>
    public enum DatabaseType
    {
        MySql,
        PostgreSql,
        SqlLite,
        SqlServe,
    }
}
