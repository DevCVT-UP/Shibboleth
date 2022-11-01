using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;


namespace ShibbolethLogin
{
    public abstract class ControllerUserBase : Controller
    {
        protected readonly IShibbolethService ShibbolethService;
        protected readonly ILogger<ControllerUserBase> Logger;


        protected ControllerUserBase(IShibbolethService shibbolethService, ILogger<ControllerUserBase> logger)
        {
            ShibbolethService = shibbolethService;
            Logger = logger;
        }

        /// <summary>
        /// Function to extract user information from shibboleth headers, default ["remoteuser"]
        /// </summary>

        public string SSOUser => Request.Headers[ShibbolethService.Config.HeaderRemoteUser];

        //
        public override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next) //TODO: 
        {
            return base.OnActionExecutionAsync(context, next);
        }

        ///// <summary>
        ///// auto signin/singoff capability
        ///// </summary>
        ///// <param name="context"></param>
        //public override void OnActionExecuting(ActionExecutingContext context) //TODO: vyzkouset jestli odhlasuje sam pokud vyprsi shibboleth
        //{
        //    if (!string.IsNullOrEmpty(SSOUser) && !User.Identity.IsAuthenticated)
        //        ShibbolethService.SignIn(HttpContext, User.Identity.Name, 0);
        //    if (User.Identity.IsAuthenticated && string.IsNullOrEmpty(SSOUser))
        //        ShibbolethService.SignOff(HttpContext);
        //    base.OnActionExecuting(context);
        //}
    }
}
