using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ShibbolethLogin
{
    public class ShibbolethAutoSinginSignOffFilter : IAsyncAuthorizationFilter
    {

        private readonly IShibbolethService ShibbolethService;

        public ShibbolethAutoSinginSignOffFilter(IShibbolethService shibbolethService)
        {
            ShibbolethService = shibbolethService;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext filterContext)
        {
            var context = filterContext?.HttpContext;
            var isAuthenticated = context?.User?.Identity?.IsAuthenticated ?? false;
            var SSOUser = context?.Request?.Headers[ShibbolethService.Config.HeaderRemoteUser];
            if (!string.IsNullOrEmpty(SSOUser) && !isAuthenticated)
            {
                await ShibbolethService.SignIn(context, SSOUser, 0);
                context.Response.Redirect(context.Request.GetDisplayUrl());
                return;
            }

            if (!isAuthenticated || context.User.HasClaim(p=>p.Type == ShibbolethService.Config.ExternalUserClaimType)) return;

            if (string.IsNullOrEmpty(SSOUser))
            {
                ShibbolethService.SignOff(context);
                context.Response.Redirect(context.Request.GetDisplayUrl());
                return;
            }
        }

    }
}