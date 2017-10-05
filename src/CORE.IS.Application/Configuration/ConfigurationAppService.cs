using System.Threading.Tasks;
using Abp.Authorization;
using Abp.Runtime.Session;
using CORE.IS.Configuration.Dto;

namespace CORE.IS.Configuration
{
    [AbpAuthorize]
    public class ConfigurationAppService : ISAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(AbpSession.ToUserIdentifier(), AppSettingNames.UiTheme, input.Theme);
        }
    }
}
