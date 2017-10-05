using System.Threading.Tasks;
using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CORE.IS.Roles.Dto;

namespace CORE.IS.Roles
{
    public interface IRoleAppService : IAsyncCrudAppService<RoleDto, int, PagedResultRequestDto, CreateRoleDto, RoleDto>
    {
        Task<ListResultDto<PermissionDto>> GetAllPermissions();
    }
}
