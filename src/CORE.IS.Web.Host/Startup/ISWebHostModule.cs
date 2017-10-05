using System.Reflection;
using Abp.Modules;
using Abp.Reflection.Extensions;
using CORE.IS.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;

namespace CORE.IS.Web.Host.Startup
{
    [DependsOn(
       typeof(ISWebCoreModule))]
    public class ISWebHostModule: AbpModule
    {
        private readonly IHostingEnvironment _env;
        private readonly IConfigurationRoot _appConfiguration;

        public ISWebHostModule(IHostingEnvironment env)
        {
            _env = env;
            _appConfiguration = env.GetAppConfiguration();
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(ISWebHostModule).GetAssembly());
        }
    }
}
