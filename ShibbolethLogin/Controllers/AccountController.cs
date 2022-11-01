using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace ShibbolethLogin
{


    public class AccountController : ControllerUserBase
    {
        private static (string controller, string action) logincallback =(null,null) ;
        private IURLCodec urlcodec;

        public AccountController(IShibbolethService shibbolethService, ILogger<AccountController> logger, IURLCodec codec) : base(shibbolethService, logger)
        {
            this.urlcodec = codec;
            if (logincallback.controller == null)
            {
                var split = ShibbolethService.Config.LoginCalbackAction.TrimStart('~').Trim('/').Split("/");
                if (split.Length != 2)
                    throw new Exception($"Wrong format of {nameof(ShibbolethService.Config.LoginCalbackAction)}");
                logincallback = (split[0], split[1]);
            }
        }

        public ActionResult Login(string returnUrl)
        {
            returnUrl = urlcodec.Encode(returnUrl??"");
            var callback = Url.Action(logincallback.action, logincallback.controller, new { returnUrl }, Request.Scheme);
            if (string.IsNullOrEmpty(SSOUser))
            {
                return Redirect(string.Format(ShibbolethService.Config.SSOLoginUrlFormatString,callback));
            }
            return Redirect(callback);
        }

        public ActionResult LoginCallback(string returnUrl)
        {
            var decoded = urlcodec.Decode(returnUrl??"");
            if (string.IsNullOrEmpty(decoded))
                return Redirect(Url.Content(ShibbolethService.Config.AfterLoginPath));
            return Redirect(decoded);
        }

        //public async Task<ActionResult> Login(string returnUrl)
        //{

        //    if (string.IsNullOrEmpty(SSOUser))
        //    {
        //        if (returnUrl != "/" && !string.IsNullOrEmpty(returnUrl))
        //        {
        //            returnUrl = Url.Action("AfterShibbolethLogin", "Account", new { returnUrl = Base64UrlEncoder.Encode(returnUrl) });
        //        }

        //        return Redirect(string.Format(ShibbolethService.Config.SSOLoginUrlFormatString, returnUrl ?? ""));
        //    }

        //    await ShibbolethService.SignIn(Request.HttpContext, SSOUser, 0);
        //    if (!string.IsNullOrEmpty(returnUrl))
        //    {
        //        return Redirect(returnUrl);
        //    }
        //    return Redirect(Url.Content(ShibbolethService.Config.AfterLoginPath));
        //}

        //public IActionResult AfterShibbolethLogin(string returnUrl)
        //{
        //    return Redirect(Base64UrlEncoder.Decode(returnUrl));
        //}


        //public IActionResult AfterSignIn(string returnUrl = null)
        //{
          
        //    if (!string.IsNullOrEmpty(returnUrl))
        //    {
        //        return Redirect(returnUrl);
        //    }
        //    return Redirect(Url.Content(ShibbolethService.Config.AfterLoginPath));
        //}

        public IActionResult LogOff()
        {
            try
            {
                ShibbolethService.SignOff(HttpContext);
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
            }

            return
                Redirect(ShibbolethService.Config.SSOLogoutUrl);
        }

        public IActionResult FakeLogin(string id)
        {
            try
            {
                if (ShibbolethService.Config.Testing || User.IsInRole("superadmin"))
                {

                    Logger.LogInformation($"Called FakeLogin{(ShibbolethService.Config.Testing ? " in testing mode" : "")}{(!string.IsNullOrEmpty(User.Identity.Name) ? " by user " + User.Identity.Name : "")} with login: {id}");
                    var claims = ShibbolethService.SignIn(Request.HttpContext, id, 1).Result;

                }
            }
            catch (Exception e)
            {
                Logger.LogError(e, e.Message);
            }

            return Redirect(Url.Content(ShibbolethService.Config.AfterLoginPath));
        }

    }
}
