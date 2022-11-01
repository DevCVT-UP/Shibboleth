using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ShibbolethLogin.Claims
{
    /// <summary>
    /// Class for encapsulation the collection of claims processor
    /// Creates and fills up claims identity object for given user 
    /// </summary>
    public class ClaimsProfile : IClaimsProfile
    {
        /// <summary>
        /// An array of at IClaimProcessors
        /// </summary>
        public IClaimsProcessor[] ClaimsProcessors { get; set; } = new IClaimsProcessor[] { };
        
        /// <summary>
        /// Name of default domain
        /// </summary>
        public string DefaultDomain { get; set; }
        /// <summary>
        /// Logger
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// sets up user identity main claims (name and name identifier)
        /// </summary>
        /// <param name="user">allows enter in 'user' or 'user@domain' format. if domain is not provided method uses default domain to fill the full name identifier</param>
        /// <returns>collection of claims constructed form given username</returns>
        protected virtual IEnumerable<Claim> CreateUserClaims(string user)
        {
            if (user.Contains("@"))
            {
                var split = user.Split("@");
                yield return new Claim(ClaimTypes.Name, split[0], XMLSchemas.String);
                yield return new Claim(ClaimTypes.NameIdentifier, user, XMLSchemas.String);
                yield break;
            }

            yield return new Claim(ClaimTypes.Name, user, XMLSchemas.String);
            yield return new Claim(ClaimTypes.NameIdentifier, $"{user}@{DefaultDomain}", XMLSchemas.String);
        }

        /// <summary>
        /// Creates, fills and returns ClaimIdentity object for given user
        /// </summary>
        /// <param name="context">httpcontext of request</param>
        /// <param name="user">allows enter in 'user' or 'user@domain' format. if domain is not provided method uses default domain to fill the full name identifier</param>
        /// <returns>claims identity object with processed claims</returns>
        public ClaimsIdentity CreateIdentity(HttpContext context, string user)
        {
            try
            {
                var claims = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);

                claims.AddClaims(CreateUserClaims(user));

                foreach (var proc in ClaimsProcessors)
                {
                    proc?.ProcessClaims(claims, context, user);
                }

                return claims;
            }
            catch (Exception ex)
            {
                Logger?.LogError(ex,"Error Creating Values");
                return null;
            }
        }
    }
}