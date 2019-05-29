using Microsoft.AspNetCore.Identity;
using System;

namespace Yu.Data.Infrasturctures
{
    public partial class BaseIdentityUser : IdentityUser<Guid>
    {
        public string OpenId { get; set; }
    }
}
