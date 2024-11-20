using System.Threading;
using System.Web.Mvc;
using Sediin.PraticheRegionali.WebUI.Controllers;
using Sediin.PraticheRegionali.WebUI.Filters;
using static Sediin.PraticheRegionali.WebUI.IdentityHelper;

namespace Sediin.PraticheRegionali.WebUI.Areas.Admin.Controllers
{
    [@Authorize(Roles = new Roles[] { Roles.Admin, Roles.Super })]
    public class HomeController : BaseController
    {
        // GET: Admin/Home
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Useronline()
        {
            return AjaxView();
        }

        [HttpPost]
        public ActionResult LogoutUser(string id)
        {
            UserOnlineAttribute.LogOffUser(id);
            Thread.Sleep(1500);
            return JsonResultTrue("Utente e stato espulso");
        }
    }
}