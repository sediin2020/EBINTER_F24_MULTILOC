using System.Web.Mvc;

namespace Sediin.PraticheRegionali.WebUI.Controllers
{
    public class ErrorManagerController : BaseController
    {
        // GET: ErrorManager
        public ActionResult Index()
        {
            return AjaxView();
        }
        public ActionResult Fire404Error()
        {
            return AjaxView();
        }
    }
}