using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;

namespace ShibbolethLogin
{
    public static class ClaimsIdentityExtensions
    {
        public static string GetSpecificClaim(this ClaimsIdentity claimsIdentity, string claimType)
        {
            var claim = claimsIdentity.Claims.FirstOrDefault(x => x.Type == claimType);

            return (claim != null) ? claim.Value : string.Empty;
        }

        /// <summary>
        /// Get claim by type
        /// </summary>
        /// <param name="type">claim type</param>
        public static string GetClaimByType(this IIdentity identity, string type)
        {
            var claimsIdentity = (ClaimsIdentity)identity;
            return claimsIdentity.GetSpecificClaim(type);
        }

        /// <summary>
        /// Returns all user roles
        /// </summary>
        public static List<string> GetRoles(this IIdentity identity)
        {
            var roles = new List<string>();
            var claimsIdentity = (ClaimsIdentity)identity;
            foreach(var r in claimsIdentity.Claims.Where(x => x.Type == ClaimTypes.Role))
            {
                if (r != null)
                    roles.Add(r.Value);
            }
            return roles;
        }
    }
}
