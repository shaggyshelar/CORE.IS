using System.Threading.Tasks;
using Abp.Application.Services;
using CORE.IS.Sessions.Dto;

namespace CORE.IS.Sessions
{
    public interface ISessionAppService : IApplicationService
    {
        Task<GetCurrentLoginInformationsOutput> GetCurrentLoginInformations();
    }
}
