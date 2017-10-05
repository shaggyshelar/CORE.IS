using System.Threading.Tasks;
using CORE.IS.Configuration.Dto;

namespace CORE.IS.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}