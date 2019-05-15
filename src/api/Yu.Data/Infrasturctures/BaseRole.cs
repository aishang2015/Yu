using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Yu.Data.Infrasturctures
{
    public class BaseRole<T> : IdentityRole<T> where T : IEquatable<T>
    {
    }
}
