using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ShibbolethLogin.Claims
{
    /// <summary>
    /// Class for encapsulation the collection of claims processors
    /// Creates and fills up claims identity object for given user 
    /// </summary>
    public interface IClaimsProfile
    {
        /// <summary>
        /// Array of IClaimsProcessors to process identity claims from different data sources 
        /// </summary>
        IClaimsProcessor[] ClaimsProcessors { get; }

        /// <summary>
        /// Creates, fills and returns ClaimIdentity object for given user
        /// </summary>
        /// <param name="context">httpcontext of request</param>
        /// <param name="user">allows enter in 'user' or 'user@domain' format. if domain is not provided method uses default domain to fill the full name identifier</param>
        /// <returns>claims identity object with processed claims</returns>
        ClaimsIdentity CreateIdentity(HttpContext context, string user);

    }
}