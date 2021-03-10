using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Threading.Tasks;

namespace ShibbolethLogin
{
    public class AccountController : ControllerUserBase
    {
        private readonly IShibbolethConfig _shibbolethConfig;
        private readonly ILogger<AccountController> _logger;
        public AccountController(IShibbolethConfig shibbolethConfig, ILogger<AccountController> logger,IRoleResolver roleResolver = null, IClaimsConfig claimsConfig = null) : base(shibbolethConfig,logger,roleResolver,claimsConfig)
        {
            _shibbolethConfig = shibbolethConfig;
            _logger = logger;
        }

        public Task<ActionResult> AutoLogin(string returnUrl = null)
        {
            return AutoLoginShibboleth(returnUrl, _shibbolethConfig.LoginUrl, _shibbolethConfig.AfterLoginPath);
        }

        public IActionResult AfterShibbolethLogin(string returnUrl)
        {
            return Redirect(Base64UrlEncoder.Decode(returnUrl));
        }

        public IActionResult Login(string returnUrl)
        {
            base.OnAuthentication();
            return RedirectToAction("AutoLogin", new { returnUrl });
        }

        public IActionResult AfterSignIn(string returnUrl = null)
        {
            base.OnAuthentication();
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return Redirect(Url.Content(_shibbolethConfig.AfterLoginPath));
        }

        public IActionResult LogOff()
        {
            try
            {
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.Session.Clear();
            }
            catch(Exception e)
            {
                _logger.LogError(e.Message);
            }

            return
                Redirect(_shibbolethConfig.LogoutUrl);
        }

        public IActionResult FakeLogin(string id)
        {
            try
            {
                if (_shibbolethConfig.Testing || User.IsInRole("superadmin"))
                {
                    if (_shibbolethConfig.UseLogger)
                        _logger.LogInformation($"Called FakeLogin{(_shibbolethConfig.Testing ? " in testing mode" : "")}{(!string.IsNullOrEmpty(User.Identity.Name) ? " by user " + User.Identity.Name : "")} with login: {id}");
                    var claims = base.SignIn(id, true).Result;
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return Redirect(Url.Content(_shibbolethConfig.AfterLoginPath));
        }
    }
}
