using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using ShibbolethLogin.Claims;

namespace ShibbolethLogin.ActiveDirectory
{
    public static class ActiveDirectoryUtils
    {
        public static IEnumerable<string> GetUserGroups(this IADConfig config,  Principal src)
        {
            try
            {
                var de = src.GetUnderlyingObject() as DirectoryEntry;
                if (de == null)
                    return Enumerable.Empty<string>();
                return  config.GetUserGroups(de);
            }
            catch(Exception ex)
            {
                config.Logger.LogError(ex, ex.Message);
                return Enumerable.Empty<string>();
            }
        }

        public static IEnumerable<Claim> GetClaims(this IADConfig config, Principal src, IEnumerable<ClaimEntry> claims)
        {
            if (claims == null)
                return Enumerable.Empty<Claim>();
            var userClaims = new List<Claim>();
            var de = src.GetUnderlyingObject() as DirectoryEntry;
            foreach(var c in claims.Where(x => !string.IsNullOrEmpty(x.ClaimType) && !string.IsNullOrEmpty(x.ValueName)))
            {
                try
                {
                    var value = de.Properties[c.ValueName]?.Value?.ToString();
                    if (!string.IsNullOrEmpty(value))
                    {
                        userClaims.Add(new Claim(c.ClaimType, value, XMLSchemas.String));
                    }
                }
                catch(Exception ex)
                {
                    config.Logger.LogError(ex,ex.Message );
                }
            }
            return userClaims;
        }

        public static IEnumerable<string> GetUserGroups(this IADConfig config,DirectoryEntry de)
        {
            var userGroups = new List<string>();
            if (de.Properties.Contains(config.GroupMemberAttributeName))
            {
                foreach (var dn in de.Properties[config.GroupMemberAttributeName])
                {
                    var group = config.ParseGroup(dn.ToString());
                    if (!string.IsNullOrEmpty(group))
                    {
                        userGroups.Add(group);
                    }
                }
            }
            return userGroups;
        }

        private static string ParseGroup(this IADConfig config,string dnString)
        {
            var strings = dnString.Split(',');
            foreach (string s in strings)
            {
                if (s.StartsWith(config.CommonNameFragmentStart))
                {
                    return s.Replace(config.CommonNameFragmentStart, "");
                }
            }
            return "";
        }
    }
}
