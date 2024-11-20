using System;
using System.Web.Mvc;
using Microsoft.AspNet.SignalR;
using Sediin.PraticheRegionali.WebUI.Areas.Admin.Models;
using Sediin.PraticheRegionali.WebUI.Controllers;
using Sediin.PraticheRegionali.WebUI.Filters;
using Sediin.PraticheRegionali.WebUI.Hubs;
using static Sediin.PraticheRegionali.WebUI.IdentityHelper;

namespace Sediin.PraticheRegionali.WebUI.Areas.Admin.Controllers
{
    [@Authorize(Roles = new Roles[] { Roles.Admin, Roles.Super })]
    public class InstantMessageController : BaseController
    {
        // GET: Admin/InstantMessage
        public ActionResult Index()
        {
            return AjaxView();
        }

        [ValidateInput(false)]
        public ActionResult Invia(InstantMessageModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                IHubContext context = GlobalHost.ConnectionManager.GetHubContext<SediinPraticheRegionaliHub>();
                context.Clients.All.onSendInstantMessage("<strong>Messaggio dal Amministratore SediinPraticheRegionali</strong><br/><br/>" + model.Messaggio);

                return JsonResultTrue("Messaggio istantaneo inviato a tutti client connessi");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}