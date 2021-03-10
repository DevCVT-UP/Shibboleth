using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading;

namespace ShibbolethLogin.ActiveDirectory.ActiveDirectory
{
   public  class ADRoleResolver : IRoleResolver
   {
       private ADConfig config;

        public ADRoleResolver(ADConfig config)
        {
            this.config = config;
        }

        public IEnumerable<string> GetUserRoles(string userName, IEnumerable<string> userGroups, ClaimsIdentity claims = null)
        {
            try
            {
                var usr = config.GetUser(userName);
                if (usr == null)
                    return null;

                if (claims != null)
                {
                    var _claims = ADUtils.GetClaims(usr, config.Claims);
                    if (_claims != null)
                        foreach (var c in _claims)
                            claims.AddClaim(c);
                }
                return ADUtils.GetUserGroups(usr);
            }
            catch(Exception ex)
            {
                Trace.TraceError(ex.Message + ex.StackTrace);
                return null;
            }

        }

        public bool IsInRole(string userName, string role, IEnumerable<string> userGroups)
        {
            return GetUserRoles(userName,userGroups).Any(p => p.Equals(role, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}
