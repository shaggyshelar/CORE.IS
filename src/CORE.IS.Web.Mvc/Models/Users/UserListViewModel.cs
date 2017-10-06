using System.Collections.Generic;
using CORE.IS.Roles.Dto;
using CORE.IS.Users.Dto;

namespace CORE.IS.Web.Models.Users
{
    public class UserListViewModel
    {
        public IReadOnlyList<UserDto> Users { get; set; }

        public IReadOnlyList<RoleDto> Roles { get; set; }
    }
}