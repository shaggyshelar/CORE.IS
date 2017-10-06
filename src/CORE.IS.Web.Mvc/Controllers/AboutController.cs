using Abp.AspNetCore.Mvc.Authorization;
using CORE.IS.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace CORE.IS.Web.Controllers
{
    [AbpMvcAuthorize]
    public class AboutController : ISControllerBase
    {
        public ActionResult Index()
        {
            return View();
        }
	}
}