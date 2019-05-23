using System;
using System.Collections.Generic;
using System.Text;
using Yu.Data.Infrasturctures;

namespace Yu.Data.Entities
{
    [BelongTo(typeof(BaseIdentityDbContext))]
    public class Test:BaseEntity<Guid>
    {
        public string Property1 { get; set; }
    }
}
