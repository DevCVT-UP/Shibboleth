using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.DependencyInjection;
using ShibbolethLogin.Roles;

namespace ShibbolethLogin
{
    public static class Utils
    {

        public static IServiceCollection UseShibboleth(this IServiceCollection svc, Action<ShibbolethConfig> options)
        {
            var config = new ShibbolethConfig();
            options(config);
            svc.AddSingleton<IShibbolethConfig>(config);
            svc.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.Cookie.Name = config.CookieName;
                    options.Cookie.HttpOnly = true;
                    options.ExpireTimeSpan = config.ExpireTimeSpan;
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/LogOut";
                    options.AccessDeniedPath = config.AccessDeniedPath;
                });
            svc.AddSession(options =>
            {
                options.IdleTimeout = config.ExpireTimeSpan;
                options.Cookie.HttpOnly = true;
            });
            return svc;
        }

        public static IServiceCollection UseRoleResolvers(this IServiceCollection svc, params IRoleResolver[] resolvers)
        {
            if (resolvers.Length == 0) return svc;
            if (resolvers.Length == 1)
                svc.AddSingleton<IRoleResolver>(resolvers[0]);

            svc.AddSingleton<IRoleResolver>(s =>
            {
                IRoleResolver top = s.GetService<IRoleResolver>();

                foreach (var res in resolvers.Reverse())
                {
                    top = new LinkedRoleResolver(top, res);
                }

                return top;
            });

            return svc;
        }
    }
}
