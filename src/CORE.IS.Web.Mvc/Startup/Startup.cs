﻿using System;
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
using App.Metrics.Reporting.Interfaces;
using App.Metrics.Extensions.Reporting.ElasticSearch;
using App.Metrics.Extensions.Reporting.ElasticSearch.Client;
using App.Metrics.Filtering;
using App.Metrics;
using App.Metrics.Configuration;


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
            // var builder = new ConfigurationBuilder().SetBasePath(env.ContentRootPath).
            //                 AddJsonFile("appsettings.json", optional: false, reloadOnChange: true).
            //                 AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true).
            //                 AddEnvironmentVariables();
            _appConfiguration = env.GetAppConfiguration();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            //MVC
            services.AddMvc(options =>
            {
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
                options.AddMetricsResourceFilter();
            });

            // var reportFilter = new DefaultMetricsFilter();
            // reportFilter.WithHealthChecks(false);
            
            //  services.AddMetrics(_appConfiguration.GetSection("AppMetrics")).                 
            //          AddJsonHealthSerialization().
            //          // AddJsonMetricsTextSerialization().
            //          AddElasticsearchMetricsTextSerialization(ElasticSearchIndex).
            //          AddElasticsearchMetricsSerialization(ElasticSearchIndex).
            //          AddReporting(
            //              factory =>
            //              {
            //                  factory.AddElasticSearch(
            //                      new ElasticSearchReporterSettings
            //                      {                                    
            //                          ElasticSearchSettings = new ElasticSearchSettings(ElasticSearchUri, ElasticSearchIndex)                                     
            //                      },
            //                      reportFilter);
            //              }).
            //          AddHealthChecks(
            //              factory =>
            //              {
            //                  factory.RegisterPingHealthCheck("google ping", "google.com", TimeSpan.FromSeconds(10));
            //                  factory.RegisterHttpGetHealthCheck("github", new Uri("https://github.com/"), TimeSpan.FromSeconds(10));
            //              }).
            //          AddMetricsMiddleware(_appConfiguration.GetSection("AspNetMetrics"));
            
            
            
            services.AddMetrics()
		    .AddJsonSerialization() //- Enables json format on the /metrics-text, /metrics, /health and /env endpoints.
			.AddJsonMetricsSerialization() // Enables json format on the /metrics-text endpoint.
			.AddJsonMetricsTextSerialization() // Enables json format on the /metrics endpoint.
			.AddJsonHealthSerialization() // Enables json format on the /health endpont.
			.AddJsonEnvironmentInfoSerialization() // Enables json format on the /env endpont.
			.AddHealthChecks()
			.AddMetricsMiddleware()
            .AddReporting(factory =>
            {
                factory.AddElasticSearch(new ElasticSearchReporterSettings
                {
                    HttpPolicy = new HttpPolicy
                    {
                        FailuresBeforeBackoff = 3,
                        BackoffPeriod = TimeSpan.FromSeconds(30),
                        Timeout = TimeSpan.FromSeconds(3)
                    },
                    ElasticSearchSettings = new ElasticSearchSettings(new Uri("http://localhost:9200"), "metrics"),
                    ReportInterval = TimeSpan.FromSeconds(5)
                });
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

            app.UseMetrics();
            app.UseMetricsReporting(lifetime);

            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseJwtTokenMiddleware();

#if FEATURE_SIGNALR
            //Integrate to OWIN
            app.UseAppBuilder(ConfigureOwinServices);
#endif

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
