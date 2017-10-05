using Abp.Authorization;
using CORE.IS.Authorization.Roles;
using CORE.IS.Authorization.Users;

namespace CORE.IS.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {

        }
    }
}
