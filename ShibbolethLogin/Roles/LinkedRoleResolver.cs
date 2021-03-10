using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace ShibbolethLogin.Roles
{
    public class LinkedRoleResolver: IRoleResolver

    {
        private IRoleResolver _outer;
        private IRoleResolver _inner;

        public LinkedRoleResolver(IRoleResolver outer, IRoleResolver inner)
        {
            this._outer = outer;
            this._inner = inner;
        }

        public IEnumerable<string> GetUserRoles(string userName, IEnumerable<string> userGroups, ClaimsIdentity claims = null)
        {
            return _outer.GetUserRoles(userName, _inner?.GetUserRoles(userName, userGroups, claims) ?? userGroups, claims);
        }

        public bool IsInRole(string userName, string role, IEnumerable<string> userGroups)
        {
            return _outer?.IsInRole(userName, role,
                       _inner?.GetUserRoles(userName, userGroups) ?? userGroups) ?? false;

        }
    }
}
