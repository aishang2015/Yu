using System;
using System.Collections.Generic;
using System.Linq;
using Yu.Core.Extensions;
using Yu.Data.Entities;

namespace Yu.Data.Infrasturctures
{
    /// <summary>
    /// 标记entity从属于哪个dbcontext
    /// </summary>
    public class BelongToAttribute : Attribute
    {
        public Type DbContextType { get; set; }

        public BelongToAttribute(Type dbContextType)
        {
            DbContextType = dbContextType;
        }
    }
}
