using System;
using App.Metrics.Extensions.Reporting.InfluxDB;
using App.Metrics.Extensions.Reporting.InfluxDB.Client;
using App.Metrics.Configuration;
using App.Metrics.Filtering;
using App.Metrics.Reporting.Interfaces;
using Abp.AspNetCore;
using Abp.Castle.Logging.Log4Net;
using Abp.IdentityServer4;
using CORE.IS.Configuration;
using CORE.IS.Identity;
using CORE.IS.Web.Resources;
using CORE.IS.Authentication.JwtBearer;
using IdentityServer4.AspNetIdentity;
using Castle.Facilities.Logging;
using CORE.IS.EntityFrameworkCore;
using CORE.IS.Authorization.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using App.Metrics;

#if FEATURE_SIGNALR
using Owin;
using Abp.Owin;
using CORE.IS.Owin;
#endif

namespace CORE.IS.Web.Startup
{
    public class Startup
    {
        private readonly IConfigurationRoot _appConfiguration;

        public Startup(IHostingEnvironment env)
        {
            _appConfiguration = env.GetAppConfiguration();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            var database = "appMetrics";
            var uri = new Uri("http://localhost:8086");

            var reportFilter = new DefaultMetricsFilter();
            reportFilter.WithHealthChecks(false);

            services.AddMetrics(_appConfiguration.GetSection("AppMetrics"))
            .AddHealthChecks(
                factory =>
                    {
                        factory.RegisterPingHealthCheck("google ping", "google.com", TimeSpan.FromSeconds(10));
                        factory.RegisterHttpGetHealthCheck("github", new Uri("https://github.com/"), TimeSpan.FromSeconds(10));
                    }
            )
            .AddJsonSerialization()
            .AddReporting(
                factory =>
                {
                    factory.AddInfluxDb(
                        new InfluxDBReporterSettings
                        {
                            InfluxDbSettings = new InfluxDBSettings(database, uri),
                            ReportInterval = TimeSpan.FromSeconds(5)
                        },
                        reportFilter);
                })
            //.AddMetricsMiddleware(options => options.IgnoredHttpStatusCodes = new[] { 404 });
            .AddMetricsMiddleware(_appConfiguration.GetSection("AspNetMetrics"));

            //MVC
            services.AddMvc(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                options.AddMetricsResourceFilter();
            });

            IdentityRegistrar.Register(services);

            services.AddIdentityServer()
                //.AddTemporarySigningCredential()
                .AddDeveloperSigningCredential()
                .AddInMemoryIdentityResources(IdentityServerConfig.GetIdentityResources())
                .AddInMemoryApiResources(IdentityServerConfig.GetApiResources())
                .AddInMemoryClients(IdentityServerConfig.GetClients())
                .AddAbpPersistedGrants<ISDbContext>()
                .AddAbpIdentityServer<User>();

            AuthConfigurer.Configure(services, _appConfiguration);

            services.AddScoped<IWebResourceManager, WebResourceManager>();

            //Configure Abp and Dependency Injection
            return services.AddAbp<ISWebMvcModule>(options =>
            {
                //Configure Log4Net logging
                options.IocManager.IocContainer.AddFacility<LoggingFacility>(
                    f => f.UseAbpLog4Net().WithConfig("log4net.config")
                );
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory, IApplicationLifetime lifetime)
        {
            app.UseAbp(); //Initializes ABP framework.

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseIdentityServer();

            // app.UseIdentityServerAuthentication(
            //     new IdentityServerAuthenticationOptions
            //     {
            //         Authority = "http://localhost:5000/",
            //         RequireHttpsMetadata = false,
            //         AutomaticAuthenticate = true,
            //         AutomaticChallenge = true
            //     });

            app.UseStaticFiles();

            app.UseAuthentication();
            app.UseJwtTokenMiddleware();

#if FEATURE_SIGNALR
            //Integrate to OWIN
            app.UseAppBuilder(ConfigureOwinServices);
#endif
            app.UseMetrics();
            app.UseMetricsReporting(lifetime);

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "defaultWithArea",
                    template: "{area}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

#if FEATURE_SIGNALR
        private static void ConfigureOwinServices(IAppBuilder app)
        {
            app.Properties["host.AppName"] = "IS";

            app.UseAbp();

            app.MapSignalR();
        }
#endif
    }
}
