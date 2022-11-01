using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using ShibbolethLogin.Claims;

namespace ShibbolethLogin.ActiveDirectory
{
    /// <summary>
    /// Implementation of IClaimsProcessor that creates claims according to underlying active directory attributes
    /// </summary>
    public class ActiveDirectoryAttributeClaimsProcessor : IClaimsProcessor
    {
       private readonly IADConfig config;

       public  IEnumerable<ClaimEntry> Claims { get; set; }

        public ActiveDirectoryAttributeClaimsProcessor(IADConfig config,IEnumerable<ClaimEntry> claims)
        {
            this.config = config;
            this.Claims = claims;
        }

        public void ProcessClaims(ClaimsIdentity claims, HttpContext context, string user)
        {
          
            (string login, string domain) usr = user.CanonizeUserDomainPair(config.DefaultDomain);
            if (usr.domain != config.DefaultDomain) return;

            var aduser = config.GetUser(usr.login);
            if (aduser == null) return;
            claims.AddClaims(config.GetClaims(aduser,Claims));
        }
    }
}