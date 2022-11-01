using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace ShibbolethLogin.Claims
{
    /// <summary>
    /// Interface for rpocessing user claims from different data sources
    /// </summary>
    public interface IClaimsProcessor
    {
        /// <summary>
        /// Method to override, can add claims to signing in user.
        /// </summary>
        /// <param name="claims">claims</param>
        /// <param name="context">context from controller to get service</param>
        /// <param name="user"></param>
        void ProcessClaims(ClaimsIdentity claims, HttpContext context, string user);
    }
}
