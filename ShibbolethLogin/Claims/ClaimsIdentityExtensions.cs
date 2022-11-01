using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;

namespace ShibbolethLogin.Claims
{
    /// <summary>
    /// utility class with claims identity extensions
    /// </summary>
    public static class ClaimsIdentityExtensions
    {

        /// <summary>
        /// Get claim by type, if type is not of ClaimsIdentity returns an empty string
        /// </summary>
        /// <param name="type">claim type</param>
        public static string GetClaimByType(this IIdentity identity, string type)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            var claim = claimsIdentity?.Claims.FirstOrDefault(x => x.Type == type);
            return claim?.Value ?? string.Empty;
        }

        /// <summary>
        /// Returns all user roles
        /// </summary>
        public static IEnumerable<string> GetRoles(this IIdentity identity)
        {
            var claimsIdentity = identity as ClaimsIdentity;
            if (claimsIdentity == null) return Enumerable.Empty<string>();
            return claimsIdentity.Claims.Where(p => p.Type == ClaimTypes.Role).Select(p => p.Value);
        }
    }
}
