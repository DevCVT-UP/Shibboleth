using System.Collections.Generic;
using System.Security.Claims;

namespace ShibbolethLogin.Roles
{

    /// <summary>
    /// Class implementing basic chaining of two IRoleResolvers 
    /// </summary>

    public class LinkedRoleResolver: RoleResolverBase

    {
        private IRoleResolver _outer;
        private IRoleResolver _inner;

        /// <summary>
        /// Public constructor
        /// </summary>
        /// <param name="outer">Outer resolver - evaluates second</param>
        /// <param name="inner">Inner resolver - evaluates first</param>
        public LinkedRoleResolver(IRoleResolver outer, IRoleResolver inner)
        {
            this._outer = outer;
            this._inner = inner;
        }

        public override IEnumerable<string> GetUserRoles(string userName, IEnumerable<string> userGroups)
        {
            return _outer.GetUserRoles(userName, _inner?.GetUserRoles(userName, userGroups) ?? userGroups);
        }

        public override bool IsInRole(string userName, string role, IEnumerable<string> userGroups)
        {
            return _outer?.IsInRole(userName, role,
                       _inner?.GetUserRoles(userName, userGroups) ?? userGroups) ?? false;
        }
        
    }
}
