using System;
using System.Threading.Tasks;
using Abp.Application.Services;
using CORE.IS.MultiTenancy;
using Abp.Runtime.Session;
using Abp.IdentityFramework;
using CORE.IS.Authorization.Users;
using Microsoft.AspNetCore.Identity;

namespace CORE.IS
{
    /// <summary>
    /// Derive your application services from this class.
    /// </summary>
    public abstract class ISAppServiceBase : ApplicationService
    {
        public TenantManager TenantManager { get; set; }

        public UserManager UserManager { get; set; }

        protected ISAppServiceBase()
        {
            LocalizationSourceName = ISConsts.LocalizationSourceName;
        }

        protected virtual Task<User> GetCurrentUserAsync()
        {
            var user = UserManager.FindByIdAsync(AbpSession.GetUserId().ToString());
            if (user == null)
            {
                throw new Exception("There is no current user!");
            }

            return user;
        }

        protected virtual Task<Tenant> GetCurrentTenantAsync()
        {
            return TenantManager.GetByIdAsync(AbpSession.GetTenantId());
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}