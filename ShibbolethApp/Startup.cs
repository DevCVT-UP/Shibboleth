using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ShibbolethLogin;
using ShibbolethLogin.ActiveDirectory;
using ShibbolethLogin.ActiveDirectory.ActiveDirectory;
using ShibbolethLogin.Roles;

namespace ShibbolethApp
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            WebHostEnvironment = env;
        }

        public IWebHostEnvironment WebHostEnvironment { get; }
        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var shibbolethClaims = new List<ClaimValuePair>() { new ClaimValuePair(ClaimTypes.GivenName, "cn") };
            var adClaims = new List<ClaimValuePair>() { new ClaimValuePair(ClaimTypes.Email, "mail"), new ClaimValuePair("givenName", "givenName"), new ClaimValuePair("surname", "sn"), new ClaimValuePair("unit", "ou"), new ClaimValuePair("title", "title") };

            services.AddSingleton<IClaimsConfig, ClaimsConfig>();

            services.UseActiveDirectory(options =>
            {
                options.Server = Configuration["AD:Server"];
                options.Container = Configuration["AD:Container"];
                options.User = Configuration["AD:User"];
                options.Password = Configuration["AD:Password"];
                options.Claims = adClaims;
            });

            
            services.AddSingleton<IRoleResolver>(svc => 
                new LinkedRoleResolver(
                    new JsonConfigRoleResolver(Path.Combine(WebHostEnvironment.ContentRootPath, "Identity/roles.json")),
                    new ADRoleResolver(svc.GetService<ADConfig>()))
            );


        services.UseShibboleth(options =>
        {
            options.LoginUrl = Configuration["Shibboleth:LoginUrl"];
            options.LogoutUrl = Configuration["Shibboleth:LogoutUrl"];
            options.AfterLoginPath = Configuration["Shibboleth:AfterLoginPath"];
            options.Claims = shibbolethClaims;
            options.UseLogger = true;
            options.Testing = Configuration["Shibboleth:Testing"] == "true";
            options.DefaultDomain = "upol.cz";
            options.AccessDeniedPath = "/Home/AccessDenied";
            options.CookieName = "ShibbolethLogin";
            options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        });

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                //app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseSession();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
