using Abp.AspNetCore.Mvc.ViewComponents;

namespace CORE.IS.Web.Views
{
    public abstract class ISViewComponent : AbpViewComponent
    {
        protected ISViewComponent()
        {
            LocalizationSourceName = ISConsts.LocalizationSourceName;
        }
    }
}