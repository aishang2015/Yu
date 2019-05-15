using Microsoft.AspNetCore.Identity;
using System;

namespace Yu.Data.Infrasturctures
{
    public partial class BaseUser<T> : IdentityUser<T> where T : IEquatable<T>
    {
    }
}
