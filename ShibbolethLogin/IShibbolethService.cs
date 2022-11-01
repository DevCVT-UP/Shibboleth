using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ShibbolethLogin.Claims;

namespace ShibbolethLogin
{
    /// <summary>
    /// Main service for sing in and sign off
    /// </summary>
    public interface IShibbolethService
    {
        /// <summary>
        /// configuration object
        /// </summary>
        IShibbolethConfig Config { get; }
        /// <summary>
        /// Logger
        /// </summary>
        ILogger<IShibbolethService> Logger { get; }
     /// <summary>
     /// Array of configured claim profiles, at least one profile is required
     /// </summary>
        IClaimsProfile[] Profiles { get; }
     /// <summary>
     /// Uses HttpContext to sign in user and set claims using selected profile 
     /// </summary>
     /// <param name="context">HttpContext of request</param>
     /// <param name="user">user identifier  (user) or  (user@domain)</param>
     /// <param name="profileindex"></param>
     /// <returns></returns>
        Task<ClaimsIdentity> SignIn(HttpContext context, string user, int profileindex=0);
        /// <summary>
        /// signs off current user in http context 
        /// </summary>
        /// <param name="context"></param>
        void SignOff(HttpContext context);
    }
}