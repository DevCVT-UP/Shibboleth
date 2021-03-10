using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace ShibbolethLogin
{
    public interface IRoleResolver
    {
     //   IEnumerable<string> Roles { get; }
        IEnumerable<string> GetUserRoles(string userName, IEnumerable<string> userGroups, ClaimsIdentity claims = null);
        bool IsInRole(string userName, string role, IEnumerable<string> userGroups);
    }
}
