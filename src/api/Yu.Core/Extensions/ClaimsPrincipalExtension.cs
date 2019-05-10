using System.Linq;
using System.Security.Claims;

namespace Yu.Core.Extensions
{
    public static class ClaimsPrincipalExtension
    {
        public static string GetClaimValue(this ClaimsPrincipal claimsPrincipal, string claimType)
        {
            var claim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == claimType);
            return claim == null ? string.Empty : claim.Value;
        }
    }
}
