using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using ShibbolethLogin.Roles;

namespace ShibbolethLogin.ActiveDirectory
{




    public  class ADRoleResolver : RoleResolverBase
   {
       private IADConfig config;

        public ADRoleResolver(IADConfig config)
        {
            this.config = config;
        }

        public override IEnumerable<string> GetUserRoles(string userName, IEnumerable<string> userGroups)
        {
            try
            {
                (string login, string domain) usr = userName.CanonizeUserDomainPair(config.DefaultDomain);
                if (usr.domain != config.DefaultDomain)
                {
                    config.Logger.LogWarning($"Unable to process user from different domain {userName.CanonizeUserDomainString()}"); 
                    return null;
                }

                var adusr = config.GetUser(userName);
                if (adusr == null)
                {
                    config.Logger.LogWarning($"Unable to find user {userName.CanonizeUserDomainString()}");
                    return null;
                }

                return config.GetUserGroups(adusr);
            }
            catch(Exception ex)
            {
                config.Logger.LogError(ex,ex.Message);
                return null;
            }

        }

        public override bool IsInRole(string userName, string role, IEnumerable<string> userGroups)
        {
            return GetUserRoles(userName,userGroups).Any(p => p.Equals(role, StringComparison.CurrentCultureIgnoreCase));
        }
       
   }
}
