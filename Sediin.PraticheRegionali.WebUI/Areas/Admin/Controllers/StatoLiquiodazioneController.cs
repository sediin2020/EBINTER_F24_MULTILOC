using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using Sediin.PraticheRegionali.DOM.Entitys;
using Sediin.PraticheRegionali.WebUI.Areas.Admin.Models;
using Sediin.PraticheRegionali.WebUI.Areas.Backend.Controllers;
using Sediin.PraticheRegionali.WebUI.Controllers;
using Sediin.PraticheRegionali.WebUI.Filters;
using static Sediin.PraticheRegionali.WebUI.IdentityHelper;

namespace Sediin.PraticheRegionali.WebUI.Areas.Admin.Controllers
{
    [@Authorize(Roles = new Roles[] { Roles.Admin, Roles.Super })]
    public class StatoLiquidazioneController : BaseController
    {
        // GET: Backend/StatoLiquidazione
        public ActionResult Ricerca()
        {
            return AjaxView();
        }

        [HttpPost]
        public ActionResult Ricerca(StatoLiquidazioneSearchModel model, int? page)
        {
            var _query = unitOfWork.StatoLiquidazioneRepository.Get(RicercaFilter(model)).OrderBy(m => m.Descrizione);

            var _result = GeModelWithPaging<StatoLiquidazioneModelRicercaViewModel, StatoLiquidazione>(page, _query, model, 10);

            return AjaxView("RicercaList", _result);
        }

        private Expression<Func<StatoLiquidazione, bool>> RicercaFilter(StatoLiquidazioneSearchModel model)
        {
            ;
            return x => model.Descrizione != null ? x.Descrizione.StartsWith(model.Descrizione) : true;
        }

        public ActionResult RicercaExcel(StatoLiquidazioneRicercaModel model)
        {
            var _query = from a in unitOfWork.StatoLiquidazioneRepository.Get(RicercaFilter2(model))
                         select new
                         {
                             a.Descrizione,
                             a.Ordine,
                         };

            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(_query, "ErrorLogs");
        }

        private Expression<Func<StatoLiquidazione, bool>> RicercaFilter2(StatoLiquidazioneRicercaModel model)
        {
            return null;
        }

        public ActionResult Nuovo()
        {
            return AjaxView("Nuovo");
        }

        public ActionResult Modifica(int id)
        {
            var _StatoLiquidazione = unitOfWork.StatoLiquidazioneRepository.Get(m => m.StatoLiquidazioneId == id).FirstOrDefault();
            var _l = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<StatoLiquidazioneModel>(_StatoLiquidazione);
            return AjaxView("Modifica", _l);
        }

        [HttpPost]
        public ActionResult Modifica(StatoLiquidazioneModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                var _l = unitOfWork.StatoLiquidazioneRepository.Get(m => m.StatoLiquidazioneId == model.StatoLiquidazioneId).FirstOrDefault();

                //check se StatoLiquidazione esiste
                var _StatoLiquidazione = unitOfWork.StatoLiquidazioneRepository.Get(m => m.Descrizione == model.Descrizione).ToList();
                var _descr = _StatoLiquidazione.FirstOrDefault().Descrizione;
                if (_StatoLiquidazione.Count > 0 && model.Descrizione == _descr)
                {
                    throw new Exception("Stato Liquidazione già presente.");
                }

                //se non esiste allora modifico
                _l.Descrizione = model.Descrizione;
                _l.Ordine = model.Ordine;
                unitOfWork.StatoLiquidazioneRepository.Update(_l);
                unitOfWork.Save();
                return JsonResultTrue("Stato Liquidazione aggiornata correttamente");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }
    }
}