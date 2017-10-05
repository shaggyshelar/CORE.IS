using Abp.Application.Services.Dto;
using Abp.AutoMapper;
using CORE.IS.Authorization.Users;
using CORE.IS.Users;

namespace CORE.IS.Sessions.Dto
{
    [AutoMapFrom(typeof(User))]
    public class UserLoginInfoDto : EntityDto<long>
    {
        public string Name { get; set; }

        public string Surname { get; set; }

        public string UserName { get; set; }

        public string EmailAddress { get; set; }
    }
}
