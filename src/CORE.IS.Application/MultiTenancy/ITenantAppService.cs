using Abp.Application.Services;
using Abp.Application.Services.Dto;
using CORE.IS.MultiTenancy.Dto;

namespace CORE.IS.MultiTenancy
{
    public interface ITenantAppService : IAsyncCrudAppService<TenantDto, int, PagedResultRequestDto, CreateTenantDto, TenantDto>
    {
    }
}
