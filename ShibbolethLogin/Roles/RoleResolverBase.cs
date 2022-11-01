using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ShibbolethLogin.Roles
{
    /// <summary>
    /// Abstract base class for role resolving logic, implements ProcessClaims logic
    /// </summary>
    public abstract class RoleResolverBase : IRoleResolver
    {
        public virtual void ProcessClaims(ClaimsIdentity claims, HttpContext context, string user)
        {
            var roles = GetUserRoles(user, Enumerable.Empty<string>());
            claims.AddClaims(roles.Select(p => new Claim(ClaimTypes.Role, p)));
        }

        public abstract IEnumerable<string> GetUserRoles(string userName, IEnumerable<string> userGroups);
        public abstract bool IsInRole(string userName, string role, IEnumerable<string> userGroups);
    }
}