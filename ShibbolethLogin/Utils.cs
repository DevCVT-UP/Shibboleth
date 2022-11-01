using System;
using System.Web;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ShibbolethLogin.Claims;

namespace ShibbolethLogin
{
   




    public static class Utils
    {

        /// <summary>
        /// sets up and registers ShibbolethService, AddAuthentication (Microsoft.AspNetCore.Authentication) and AddSession(Microsoft.AspNetCore.Builder)
        /// </summary>
        /// <param name="svc"></param>
        /// <param name="configure">configuration action</param>
        /// <param name="profilesFunc">function to create profiles, if null returns array containing one default httpheader processor</param>
        /// <returns></returns>
        public static IServiceCollection UseShibbolethService(this IServiceCollection svc, Action<IShibbolethConfig> configure, Func<IServiceProvider,IShibbolethConfig, IClaimsProfile[]> profilesFunc)
        {
            IShibbolethConfig config = new ShibbolethConfig();

            configure(config);

            svc.AddSingleton<IShibbolethService>(p =>
                new ShibbolethService()
                {
                    Config = config,
                    Logger =  p.GetService<ILogger<IShibbolethService>>(),
                    Profiles = profilesFunc(p,config)?? new IClaimsProfile[]{ClaimProfileFactory.DefaultProfile(p.GetService<ILogger<IShibbolethService>>(),config.DefaultDomain)}
                });

            svc.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = config.CookieName;
                    options.Cookie.HttpOnly = true;
                    options.ExpireTimeSpan = config.ExpireTimeSpan;
                    options.LoginPath = config.AppLoginUrl ;
                    options.LogoutPath = config.AppLogoutUrl;
                    options.AccessDeniedPath = config.AccessDeniedPath;
                });
            svc.AddSession(options =>
            {
                options.IdleTimeout = config.ExpireTimeSpan;
                options.Cookie.HttpOnly = true;
            });
            return svc;
        }


        /// <summary>
        /// sets up and registers ShibbolethService only
        /// </summary>
        /// <param name="svc"></param>
        /// <param name="configure">configuration action</param>
        /// <param name="profilesFunc">function to create profiles, if null returns array containing one default httpheader processor</param>
        /// <returns></returns>
        public static IServiceCollection UseShibbolethServiceOnly(this IServiceCollection svc, Action<IShibbolethConfig> configure, Func<IServiceProvider, IShibbolethConfig, IClaimsProfile[]> profilesFunc)
        {
            IShibbolethConfig config = new ShibbolethConfig();

            configure(config);

            svc.AddSingleton<IShibbolethService>(p =>
                new ShibbolethService()
                {
                    Config = config,
                    Logger = p.GetService<ILogger<IShibbolethService>>(),
                    Profiles = profilesFunc(p, config) ?? new IClaimsProfile[] { ClaimProfileFactory.DefaultProfile(p.GetService<ILogger<IShibbolethService>>(), config.DefaultDomain) }
                });
            return svc;
        }



    }
}
