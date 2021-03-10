using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace ShibbolethLogin.ActiveDirectory
{
    public static class Utils
    {

        public static IServiceCollection UseActiveDirectory(this IServiceCollection svc,Action<ADConfig> options)
        {
            var config = new ADConfig();
            options(config);
            svc.AddSingleton<ADConfig>(config);
            return svc;
        }

    }
}
