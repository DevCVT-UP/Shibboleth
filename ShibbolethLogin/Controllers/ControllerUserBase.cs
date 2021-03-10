using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShibbolethLogin
{
    public abstract class ControllerUserBase : Controller
    {
      //  private readonly IADConfig _adConfig;
        private readonly IShibbolethConfig _shibbolethConfig;
        private readonly IRoleResolver _roleResolver;
        private readonly ILogger<ControllerUserBase> _logger;
        private readonly IClaimsConfig _claimsConfig;
        protected ControllerUserBase(IShibbolethConfig shibbolethConfig, ILogger<ControllerUserBase> logger, IRoleResolver roleResolver = null, IClaimsConfig claimsConfig = null)
        {
            _shibbolethConfig = shibbolethConfig;
            _roleResolver = roleResolver;
            _logger = logger;
            _claimsConfig = claimsConfig;
        }

        /// <summary>
        /// Function to extract user information from shibboleth headers, default ["remoteuser"]
        /// </summary>
        public virtual string SSOUser => Request.Headers["remoteuser"];

        /// <summary>
        /// Auxiliary function for extracting user id from a more general format, eg. "userid@domain", default if it contains the @ character, splits it into a cast before @ and uses it as a username
        /// </summary>
        /// <param name="user">username (user123@domain.com)</param>
        /// <returns>user id, eg. "user123"</returns>
        protected virtual string GetUserId(string user)
        {
            if (user.Contains("@"))
                user = user.Split('@')[0];
            return user;
        }

        /// <summary>
        /// Standard user login
        /// </summary>
        /// <param name="user">username (user@domain)</param>
        /// <param name="fakeLogin">if it is fakelogin</param>
        /// <returns>Task returns ClaimsIdentity</returns>
        protected virtual async Task<ClaimsIdentity> SignIn(string user, bool fakeLogin = false)
        {
            var claims = new ClaimsIdentity(CookieAuthenticationDefaults.AuthenticationScheme);
            try
            {
                if (user.Contains("@"))
                {
                    var split = user.Split("@");
                    claims.AddClaim(new Claim(ClaimTypes.Name, split[0], XMLSchemas.String));
                    claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, user,
                        XMLSchemas.String));
                }
                else
                {
                    claims.AddClaim(new Claim(ClaimTypes.Name, user, XMLSchemas.String));
                    claims.AddClaim(new Claim(ClaimTypes.NameIdentifier, $"{user}@{_shibbolethConfig.DefaultDomain}",
                        XMLSchemas.String));
                }

                if (!fakeLogin && !string.IsNullOrEmpty(SSOUser) && _shibbolethConfig.Claims != null)
                {
                    foreach(var c in _shibbolethConfig.Claims.Where(x => !string.IsNullOrEmpty(x.ClaimType) && !string.IsNullOrEmpty(x.ValueName)))
                    {
                        try
                        {
                            var value = Request.Headers[c.ValueName];
                            claims.AddClaim(new Claim(c.ClaimType, value, XMLSchemas.String));
                        }
                        catch (Exception e)
                        {
                            _logger.LogError(e.Message);
                        }
                    }
                }

                if (_roleResolver != null)
                {
                    var roles = _roleResolver.GetUserRoles(GetUserId(user), new string[] { }, claims).ToList();
                    foreach (var r in roles)
                        claims.AddClaim(new Claim(ClaimTypes.Role, r));
                }  

                if(_claimsConfig != null)
                {
                    try
                    {
                        _claimsConfig.EditClaims(claims, HttpContext);
                    }
                    catch(Exception e)
                    {
                        _logger.LogError(e.Message);
                    }
                }

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claims));
                if (_shibbolethConfig.UseLogger)
                    _logger.LogInformation($"User {user} signed in");
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
            }


            return claims;
        }

        /// <summary>
        /// Task which call sign in method, if SSOUser is not null
        /// </summary>
        /// <param name="returnUrl">return url after sign in</param>
        /// <param name="shiburl">login shibboleth url</param>
        /// <param name="defaultUrl">redirect to default url if returnUrl is null</param>
        /// <returns>Redirect to url</returns>
        protected virtual async Task<ActionResult> AutoLoginShibboleth(string returnUrl, string shiburl, string defaultUrl = "~/")
        {
            if (string.IsNullOrEmpty(SSOUser))
            {
                if (returnUrl != "/" && !string.IsNullOrEmpty(returnUrl))
                {
                    returnUrl = Url.Action("AfterShibbolethLogin", "Account", new { returnUrl = Base64UrlEncoder.Encode(returnUrl) });
                }

                return Redirect(string.Format(shiburl, returnUrl ?? ""));
            }

            await SignIn(SSOUser);
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return Redirect(Url.Content(defaultUrl));
        }

        /// <summary>
        /// An auxiliary function that call sign in method if user is not siggned in and sign out user if he is siggned in.
        /// </summary>
        protected void OnAuthentication()
        {
            try
            {
                if (string.IsNullOrEmpty(User.Identity.Name))
                {
                    if (!string.IsNullOrEmpty(SSOUser))
                    {
                        var usr = SignIn(SSOUser);
                    }
                }
                else
                {
                    if (string.IsNullOrEmpty(SSOUser))
                    {
                        HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                        HttpContext.Session.Clear();
                    }
                }
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
            }
        }
    }
}
