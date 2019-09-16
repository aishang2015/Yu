using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yu.Data.Infrasturctures.BaseIdentity
{
    public class BaseIdentityUserStore : UserStore<BaseIdentityUser, BaseIdentityRole, BaseIdentityDbContext, Guid>
    {
        public BaseIdentityUserStore(BaseIdentityDbContext context, IdentityErrorDescriber describer = null) : base(context, describer)
        {
            AutoSaveChanges = false;
        }
    }
}
