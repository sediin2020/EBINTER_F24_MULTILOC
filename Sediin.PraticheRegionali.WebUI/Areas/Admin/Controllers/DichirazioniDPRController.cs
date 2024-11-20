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
    public class DichiarazioniDPRController : BaseController
    {
        // GET: Admin/DichiariazioniDPR
        public ActionResult Ricerca()
        {
            return AjaxView();
        }

        [HttpPost]
        public ActionResult Ricerca(DichiarazioniDPRClass model)
        {
            var _requisiti = unitOfWork.DichiarazioniDPRRepository.Get(RicercaFilter(model)).OrderBy(m => m.Descrizione);
            return AjaxView("RicercaList", _requisiti);
        }

        private Expression<Func<Sediin.PraticheRegionali.DOM.Entitys.DichiarazioniDPR, bool>> RicercaFilter(DichiarazioniDPRClass model)
        {
            return x => (model.Descrizione != null ? x.Descrizione.Contains(model.Descrizione) : true);
        }

        public ActionResult Nuovo()
        {
            return AjaxView("Nuovo");
        }

        [HttpPost]
        public ActionResult Nuovo(DichiarazioniDPRClass model)
        {
            try
            {
                //check se allegato esiste
                var _requisiti = unitOfWork.DichiarazioniDPRRepository.Get(m => m.Descrizione == model.Descrizione).ToList();
                if (_requisiti.Count > 0)
                {
                    throw new Exception("Dichiarazioni DPR già presente.");
                }

                //se non esiste
                var _nuovoRequisito = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<Sediin.PraticheRegionali.DOM.Entitys.DichiarazioniDPR>(model);
                _nuovoRequisito.Descrizione = model.Descrizione;
                unitOfWork.DichiarazioniDPRRepository.Insert(_nuovoRequisito);
                unitOfWork.Save();
                return JsonResultTrue("Dichiarazioni DPR inserito");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult Modifica(int id)
        {
            var _requisito = unitOfWork.DichiarazioniDPRRepository.Get(m => m.DichiarazioniDPRId == id).FirstOrDefault();
            return AjaxView("Modifica", _requisito);
        }

        [HttpPost]
        public ActionResult Modifica(DOM.Entitys.DichiarazioniDPR model)
        {
            try
            {
                var _a = unitOfWork.DichiarazioniDPRRepository.Get(m => m.DichiarazioniDPRId == model.DichiarazioniDPRId).FirstOrDefault();

                //check se Dichiarazioni DPR esiste
                var _requisiti = unitOfWork.DichiarazioniDPRRepository.Get(m => m.Descrizione == model.Descrizione).ToList();
                var _descr = _requisiti.FirstOrDefault().Descrizione;
                if (_requisiti.Count > 0 && model.Descrizione == _descr)
                {
                    throw new Exception("Dichiarazione DPR già presente.");
                }

                //se non esiste allora modifico
                _a.Descrizione = model.Descrizione;
                unitOfWork.DichiarazioniDPRRepository.Update(_a);
                unitOfWork.Save();
                return JsonResultTrue("Dichiarazione DPR aggiornato");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult Elimina(int requisitiId)
        {
            var model = new EliminaDichiarazioneDPR();
            model.DichiarazioniDPRId = requisitiId;
            return AjaxView("Elimina", model);
        }

        [HttpPost]
        public ActionResult Elimina(DOM.Entitys.DichiarazioniDPR model)
        {
            try
            {
                var tipoRichieste = unitOfWork.TipoRichiestaDichirazioniDPRRepository.Get(x => x.DichiarazioniDPRId == model.DichiarazioniDPRId);
                //check se allegato è associato al requisito
                if (tipoRichieste.Count() > 0)
                {
                    throw new Exception("Impossibile eliminare: Dichiarazione DPR già associato al requisito");
                }

                //se non usata
                unitOfWork.DichiarazioniDPRRepository.Delete(model.DichiarazioniDPRId);
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