using System;
using System.ComponentModel;
using Yu.Data.Infrasturctures;

namespace Yu.Data.Entities.Right
{

    [Description("实体数据")]
    [BelongTo(typeof(BaseIdentityDbContext))]
    public class Entity : BaseEntity<Guid>
    {
        [Description("数据库上下文")]
        public string DbContext { get; set; }

        [Description("表")]
        public string Table { get; set; }

        [Description("表名")]
        public string TableDescribe { get; set; }

        [Description("列")]
        public string Field { get; set; }

        [Description("列名")]
        public string FieldDescribe { get; set; }
    }
}
