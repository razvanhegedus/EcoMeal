
using System.Security.Claims;

namespace EcoMealApp.Helpers 
{
    public static class ClaimsPrincipalExtensions
    {
        public static bool HasCustomRole(this ClaimsPrincipal user, string roleName)
        {
            if (user?.Identity == null || !user.Identity.IsAuthenticated)
            {
                return false;
            }

            if (user.IsInRole(roleName))
            {
                return true;
            }

            return user.Claims.Any(c => 
                (c.Type == ClaimTypes.Role || 
                 c.Type.Equals("role", StringComparison.OrdinalIgnoreCase) || 
                 c.Type.Equals("roles", StringComparison.OrdinalIgnoreCase)) 
                && c.Value.Equals(roleName, StringComparison.OrdinalIgnoreCase)
            );
        }
    }
}