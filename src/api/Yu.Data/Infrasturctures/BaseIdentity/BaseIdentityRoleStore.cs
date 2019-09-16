using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yu.Data.Infrasturctures.BaseIdentity
{
    public class BaseIdentityRoleStore : RoleStore<BaseIdentityRole, BaseIdentityDbContext, Guid>
    {
        public BaseIdentityRoleStore(BaseIdentityDbContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
            AutoSaveChanges = false;
        }
    }
}
