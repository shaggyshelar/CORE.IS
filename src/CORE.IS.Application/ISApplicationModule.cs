using System.Reflection;
using Abp.AutoMapper;
using Abp.Modules;
using Abp.Reflection.Extensions;
using CORE.IS.Authorization;

namespace CORE.IS
{
    [DependsOn(
        typeof(ISCoreModule), 
        typeof(AbpAutoMapperModule))]
    public class ISApplicationModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.Authorization.Providers.Add<ISAuthorizationProvider>();
        }

        public override void Initialize()
        {
            Assembly thisAssembly = typeof(ISApplicationModule).GetAssembly();
            IocManager.RegisterAssemblyByConvention(thisAssembly);

            Configuration.Modules.AbpAutoMapper().Configurators.Add(cfg =>
            {
                //Scan the assembly for classes which inherit from AutoMapper.Profile
                cfg.AddProfiles(thisAssembly);
            });
        }
    }
}