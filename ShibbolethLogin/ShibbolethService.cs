using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using ShibbolethLogin.Claims;

namespace ShibbolethLogin
{
    public class ShibbolethService : IShibbolethService
    {
        
        public IShibbolethConfig Config { get; set; }
        
        public ILogger<IShibbolethService> Logger { get; set; }
   
        public IClaimsProfile[] Profiles { get; set; }

      
     
        private void LogInfo(string message)
        {
            //if (!Config.UseLogger) return;
            Logger?.LogInformation(message);
        }


        public virtual async Task<ClaimsIdentity> SignIn(HttpContext context, string user, int profileindex=0)
        {
            if (profileindex >= Profiles.Length || profileindex < 0)
                throw new ArgumentOutOfRangeException( nameof(profileindex), "Wrong profile index");
            try
            {
                user = user.CanonizeUserDomainString(Config?.DefaultDomain).ToLower();
                var claims = Profiles[profileindex].CreateIdentity(context, user);
                await context.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claims));
                LogInfo($"User {user} signed in using profile {profileindex}");
                return claims;
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
            }
            return null;
        }

        public async void SignOff(HttpContext context)
        {
            try
            {
                await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                context.Session.Clear();
            }
            catch (Exception x)
            {
                Logger.LogError(x,x.Message);
            }
        }
    }
}