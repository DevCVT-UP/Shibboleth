using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ShibbolethLogin;
using ShibbolethLogin.ActiveDirectory;
using ShibbolethLogin.Claims;
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

            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.TryAddSingleton<IURLCodec,URLCodecBase64Url>();
            services.AddScoped<IServiceContext, ServiceContext>();

            //list of active directoru entries to be mapped into claims
            var shibbolethHeaderClaims = new [] { new ClaimEntry(ClaimTypes.GivenName, "cn") };
            //array of shibboleth headers to be mapped into claims
            var adClaims = new [] { new ClaimEntry(ClaimTypes.Email, "mail"), new ClaimEntry("givenName", "givenName"), new ClaimEntry("surname", "sn"), new ClaimEntry("unit", "ou"), new ClaimEntry("title", "title") };

            
            //active directory configuration setup
            services.UseActiveDirectory(options =>
            {
                options.Server = Configuration["AD:Server"];
                options.Container = Configuration["AD:Container"];
                options.User = Configuration["AD:User"];
                options.Password = Configuration["AD:Password"];
                options.DefaultDomain = "upol.cz";
            });
            
            //shibboleth service configuration setup
            services.UseShibbolethService(options =>
            {
                options.SSOLoginUrlFormatString = Configuration["Shibboleth:SSOLoginUrl"];
                options.SSOLogoutUrl = Configuration["Shibboleth:SSOLogoutUrl"];
                options.LoginCalbackAction = "/Account/LoginCallback";
                options.AfterLoginPath = Configuration["Shibboleth:AfterLoginPath"];
                options.AccessDeniedPath = "/Home/AccessDenied";
                options.Testing = Configuration["Shibboleth:Testing"] == "true";
                options.DefaultDomain = "upol.cz";
                options.CookieName = "ShibbolethLogin";
                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
                options.HeaderRemoteUser = "remoteuser";
                options.AppLoginUrl= "/Account/Login";
                options.AppLogoutUrl  = "/Account/Logout";

    }, (svc,options)=>
            {
                var logger = svc.GetService<ILoggerFactory>().CreateLogger("ShibbolethInfrastructure");
                var roleResolvers = new LinkedRoleResolver(
                    new JsonConfigRoleResolver(Path.Combine(WebHostEnvironment.ContentRootPath, "Identity/roles.json"),logger, options.DefaultDomain),
                    new ADRoleResolver(svc.GetService<IADConfig>()));
                var customClaimsProcessor = new CustomClaimsProcessor();
                var activeDirectoryClaimsProcessor = new ActiveDirectoryAttributeClaimsProcessor(svc.GetService<IADConfig>(), adClaims);
                return new IClaimsProfile[]
                {
                    ClaimProfileFactory.DefaultProfile(logger, options.DefaultDomain).AddProcessors(new HeaderClaimsProcessor(logger, shibbolethHeaderClaims), roleResolvers, activeDirectoryClaimsProcessor, customClaimsProcessor ), //default processor
                    ClaimProfileFactory.DefaultProfile(logger, options.DefaultDomain).AddProcessors(roleResolvers,activeDirectoryClaimsProcessor,customClaimsProcessor,new ConstClaimsProcessor( options.ExternalUserClaimType)), //external login processor
                };
            });


            services.AddControllersWithViews(options =>
            {
                options.Filters.Add(typeof(ShibbolethAutoSinginSignOffFilter));
            });
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
