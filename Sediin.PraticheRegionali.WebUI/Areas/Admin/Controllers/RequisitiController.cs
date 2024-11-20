using Sediin.PraticheRegionali.WebUI.Areas.Admin.Models;
using Sediin.PraticheRegionali.WebUI.Controllers;
using Sediin.PraticheRegionali.WebUI.Filters;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using static Sediin.PraticheRegionali.WebUI.IdentityHelper;

namespace Sediin.PraticheRegionali.WebUI.Areas.Admin.Controllers
{
    [@Authorize(Roles = new Roles[] { Roles.Admin, Roles.Super })]
    public class RequisitiController : BaseController
    {
        // GET: Admin/Requisiti
        public ActionResult Ricerca()
        {
            return AjaxView();
        }

        [HttpPost]
        public ActionResult Ricerca(RequisitiClass model)
        {
            var _requisiti = unitOfWork.RequisitiRepository.Get(RicercaFilter(model)).OrderBy(m => m.Descrizione);
            return AjaxView("RicercaList", _requisiti);
        }

        private Expression<Func<Sediin.PraticheRegionali.DOM.Entitys.Requisiti, bool>> RicercaFilter(RequisitiClass model)
        {
            return x => (model.Descrizione != null ? x.Descrizione.Contains(model.Descrizione) : true);
        }

        public ActionResult Nuovo()
        {
            return AjaxView("Nuovo");
        }

        [HttpPost]
        public ActionResult Nuovo(RequisitiClass model)
        {
            try
            {
                //check se allegato esiste
                var _requisiti = unitOfWork.RequisitiRepository.Get(m => m.Descrizione == model.Descrizione).ToList();
                if (_requisiti.Count > 0)
                {
                    throw new Exception("Requisito già presente.");
                }

                //se non esiste
                var _nuovoRequisito = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<Sediin.PraticheRegionali.DOM.Entitys.Requisiti>(model);
                _nuovoRequisito.Descrizione = model.Descrizione;
                unitOfWork.RequisitiRepository.Insert(_nuovoRequisito);
                unitOfWork.Save();
                return JsonResultTrue("Requisito inserito");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult Modifica(int id)
        {
            var _requisito = unitOfWork.RequisitiRepository.Get(m => m.RequisitiId == id).FirstOrDefault();
            return AjaxView("Modifica", _requisito);
        }

        [HttpPost]
        public ActionResult Modifica(DOM.Entitys.Requisiti model)
        {
            try
            {
                var _a = unitOfWork.RequisitiRepository.Get(m => m.RequisitiId == model.RequisitiId).FirstOrDefault();

                //check se allegato esiste
                var _requisiti = unitOfWork.RequisitiRepository.Get(m => m.Descrizione == model.Descrizione).ToList();
                var _descr = _requisiti?.FirstOrDefault()?.Descrizione;
                if (_requisiti.Count > 0 && model.Descrizione == _descr)
                {
                    throw new Exception("Requisito già presente.");
                }

                //se non esiste allora modifico
                _a.Descrizione = model.Descrizione;
                unitOfWork.RequisitiRepository.Update(_a);
                unitOfWork.Save();
                return JsonResultTrue("Requisito aggiornato");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult Elimina(int requisitiId)
        {
            var model = new EliminaRequisito();
            model.RequisitiId = requisitiId;
            return AjaxView("Elimina", model);
        }

        [HttpPost]
        public ActionResult Elimina(DOM.Entitys.Requisiti model)
        {
            try
            {
                var tipoRichieste = unitOfWork.TipoRichiestaRequisitiRepository.Get(x => x.RequisitiId == model.RequisitiId);
                //check se allegato è associato al requisito
                if (tipoRichieste.Count() > 0)
                {
                    throw new Exception("Impossibile eliminare: allegato già associato al requisito");
                }

                //se non usata
                unitOfWork.RequisitiRepository.Delete(model.RequisitiId);
                unitOfWork.Save();
                return JsonResultTrue("Eliminazione effettuata");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }
    }
}