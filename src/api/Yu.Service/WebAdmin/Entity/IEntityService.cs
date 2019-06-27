using System;
using System.Collections.Generic;
using System.Text;
using Yu.Model.WebAdmin.Entity.OutputModels;

namespace Yu.Service.WebAdmin.Entity
{
    public interface IEntityService
    {
        IEnumerable<EntityOutline> GetAllEntityOutline();
    }
}
