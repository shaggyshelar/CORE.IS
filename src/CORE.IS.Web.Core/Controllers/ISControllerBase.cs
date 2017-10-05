using Abp.AspNetCore.Mvc.Controllers;
using Abp.IdentityFramework;
using Microsoft.AspNetCore.Identity;

namespace CORE.IS.Controllers
{
    public abstract class ISControllerBase: AbpController
    {
        protected ISControllerBase()
        {
            LocalizationSourceName = ISConsts.LocalizationSourceName;
        }

        protected void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}