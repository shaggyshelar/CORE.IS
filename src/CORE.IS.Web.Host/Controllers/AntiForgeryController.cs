using CORE.IS.Controllers;
using Microsoft.AspNetCore.Antiforgery;

namespace CORE.IS.Web.Host.Controllers
{
    public class AntiForgeryController : ISControllerBase
    {
        private readonly IAntiforgery _antiforgery;

        public AntiForgeryController(IAntiforgery antiforgery)
        {
            _antiforgery = antiforgery;
        }

        public void GetToken()
        {
            _antiforgery.SetCookieTokenAndHeader(HttpContext);
        }
    }
}