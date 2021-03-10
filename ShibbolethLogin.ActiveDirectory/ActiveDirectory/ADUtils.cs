using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace ShibbolethLogin
{
    public static class ADUtils
    {
        public static IEnumerable<string> GetUserGroups(this Principal src)
        {
            try
            {
                var de = src.GetUnderlyingObject() as DirectoryEntry;
                return de.GetUserGroups();
            }
            catch(Exception ex)
            {
                Trace.TraceError(ex.Message + ex.StackTrace);
                return null;
            }
        }

        public static IEnumerable<Claim> GetClaims(this Principal src, IEnumerable<ClaimValuePair> claims)
        {
            var userClaims = new List<Claim>();
            if (claims == null)
                return userClaims;
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
                    Trace.TraceError(ex.Message + ex.StackTrace);
                }
            }
            return userClaims;
        }

        public static IEnumerable<string> GetUserGroups(this DirectoryEntry de)
        {
            var userGroups = new List<string>();
            if (de.Properties.Contains("memberof"))
            {
                foreach (var dn in de.Properties["memberof"])
                {
                    var group = ParseGroup(dn.ToString());
                    if (!string.IsNullOrEmpty(group))
                    {
                        userGroups.Add(group);
                    }
                }
            }
            return userGroups;
        }

        private static string ParseGroup(string dnString)
        {
            var strings = dnString.Split(',');
            foreach (string s in strings)
            {
                if (s.StartsWith("CN="))
                {
                    return s.Replace("CN=", "");
                }
            }
            return "";
        }
    }
}
