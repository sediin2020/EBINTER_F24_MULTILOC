using System.IO;
using System.Web.Mvc;
using Sediin.PraticheRegionali.WebUI.Controllers;
using Sediin.PraticheRegionali.WebUI.Filters;
using static Sediin.PraticheRegionali.WebUI.IdentityHelper;

namespace EBLIG.WebUI.Areas.Admin.Controllers
{
    [@Authorize(Roles = new Roles[] { Roles.Admin, Roles.Super })]
    public class ThemaController : BaseController
    {
        // GET: Admin/Thema
        public ActionResult Bootstrap()
        {

            foreach (var item in Directory.GetFiles(System.Web.Hosting.HostingEnvironment.MapPath("~/Content/bootstrap-themes")))
            {

            }

            return AjaxView();
        }
    }
}