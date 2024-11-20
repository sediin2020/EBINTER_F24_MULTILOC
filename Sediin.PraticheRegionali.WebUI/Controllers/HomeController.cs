using System.Web.Mvc;

namespace Sediin.PraticheRegionali.WebUI.Controllers
{
    public class HomeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

       
        public ActionResult Privacy()
        {
            return AjaxView();
        }
    }
}