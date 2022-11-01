using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace ShibbolethLogin.ActiveDirectory
{
    public static class Utils
    {
        /// <summary>
        /// utility method to setup and register active directory
        /// </summary>
        /// <param name="svc"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IServiceCollection UseActiveDirectory(this IServiceCollection svc, Action<IADConfig> options)
        {
            svc.AddSingleton<IADConfig,ADConfig>(s =>
            {
                var config = new ADConfig(s.GetService<ILogger<IADConfig>>());
                options(config);
                svc.AddSingleton<IADConfig>(config);
                return config;
            });

            return svc;

        }

    }
}
