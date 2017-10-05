using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CORE.IS.Roles.Dto;
using CORE.IS.Users.Dto;

namespace CORE.IS.Users
{
    public interface IUserAppService : IAsyncCrudAppService<UserDto, long, PagedResultRequestDto, CreateUserDto, UserDto>
    {
        Task<ListResultDto<RoleDto>> GetRoles();
    }
}