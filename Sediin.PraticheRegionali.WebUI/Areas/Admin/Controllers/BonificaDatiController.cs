using System.Web.Mvc;
using Sediin.PraticheRegionali.DOM.Importer;
using Sediin.PraticheRegionali.WebUI.Controllers;
using Sediin.PraticheRegionali.WebUI.Filters;
using static Sediin.PraticheRegionali.WebUI.IdentityHelper;

namespace Sediin.PraticheRegionali.WebUI.Areas.Admin.Controllers
{
    [@Authorize(Roles = new Roles[] { Roles.Admin, Roles.Super })]
    public class BonificaDatiController : BaseController
    {
        // GET: Admin/BonificaDati
        public ActionResult Index()
        {
            return AjaxView();
        }

        public ActionResult AnagraficaAziende()
        {
            ImportProvider provider = new ImportProvider();
            return JsonResultTrue(provider.BonificaAnagraficaAziende());
        }

        public ActionResult AnagraficaDipendenti()
        {
            ImportProvider provider = new ImportProvider();
            return JsonResultTrue(provider.BonificaAnagraficaDipendenti());
        }

        public ActionResult AnagraficaSportello()
        {
            ImportProvider provider = new ImportProvider();
            return JsonResultTrue(provider.BonificaAnagraficaSportello());
        }
    }
}
