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
    public class MotivazioniRichiestaController : BaseController
    {
        // GET: Backend/Motivazioni
        public ActionResult Ricerca()
        {
            return AjaxView();
        }

        [HttpPost]
        public ActionResult Ricerca(MotivazioniRichiestaSearchModel model, int? page)
        {
            var _query = unitOfWork.MotivazioniRichiestaRepository.Get(RicercaFilter(model)).OrderBy(m => m.Motivazione);

            var _result = GeModelWithPaging<MotivazioniRichiestaModelRicercaViewModel, MotivazioniRichiesta>(page, _query, model, 10);

            return AjaxView("RicercaList", _result);
        }

        private Expression<Func<MotivazioniRichiesta, bool>> RicercaFilter(MotivazioniRichiestaSearchModel model)
        {
            ;
            return x => (model.Motivazione != null ? x.Motivazione.StartsWith(model.Motivazione) : true)
                        && (model.MotivazioniRichiestaRicercaModel_TipoRichiestaId != null ? x.TipoRichiestaId == model.MotivazioniRichiestaRicercaModel_TipoRichiestaId : true);

        }

        public ActionResult RicercaExcel(MotivazioniRichiestaRicercaModel model)
        {
            var _query = from a in unitOfWork.MotivazioniRichiestaRepository.Get(RicercaFilter2(model))
                         select new
                         {
                             a.Motivazione,
                             a.TipoRichiestaId,
                         };

            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(_query, "ErrorLogs");
        }

        private Expression<Func<MotivazioniRichiesta, bool>> RicercaFilter2(MotivazioniRichiestaRicercaModel model)
        {
            return null;
        }

        public ActionResult Nuovo()
        {
            var model = new InsMotivazioniRichiesta();
            model.TipoRichiesta = unitOfWork.TipoRichiestaRepository.Get().ToList();

            return AjaxView("Nuovo", model);
        }

        [HttpPost]
        public ActionResult Nuovo(InsMotivazioniRichiesta model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                //check se Motivazione esiste
                var _Motivazioni = unitOfWork.MotivazioniRichiestaRepository.Get(m => m.Motivazione == model.Motivazione).ToList();
                if (_Motivazioni.Count > 0)
                {
                    throw new Exception("Motivazione Richiesta già presente.");
                }

                //se non esiste
                var _nuovoMotivazioni = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<MotivazioniRichiesta>(model);
                _nuovoMotivazioni.TipoRichiestaId = model.TipoRichiestaId;
                _nuovoMotivazioni.Motivazione = model.Motivazione;
                unitOfWork.MotivazioniRichiestaRepository.Insert(_nuovoMotivazioni);
                unitOfWork.Save();
                return JsonResultTrue("Motivazione Richiesta inserita correttamente");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult Modifica(int id)
        {
            var _Motivazioni = unitOfWork.MotivazioniRichiestaRepository.Get(m => m.MotivazioniRichiestaId == id).FirstOrDefault();
            var _l = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<InsMotivazioniRichiesta>(_Motivazioni);
            _l.TipoRichiesta = unitOfWork.TipoRichiestaRepository.Get();

            return AjaxView("Modifica", _l);
        }

        [HttpPost]
        public ActionResult Modifica(InsMotivazioniRichiesta model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                var _l = unitOfWork.MotivazioniRichiestaRepository.Get(m => m.MotivazioniRichiestaId == model.MotivazioniRichiestaId).FirstOrDefault();

                //check se Motivazione esiste
                //var _Motivazioni = unitOfWork.MotivazioniRichiestaRepository.Get(m => m.Motivazione == model.Motivazione).ToList();
                //var _descr = _Motivazioni.FirstOrDefault().Motivazione;
                //if (_Motivazioni.Count > 0 && model.Motivazione == _descr)
                //{
                //    throw new Exception("Motivazione Richiesta già presente.");
                //}

                //se non esiste allora modifico
                _l.TipoRichiestaId = model.TipoRichiestaId;
                _l.Motivazione = model.Motivazione;
                unitOfWork.MotivazioniRichiestaRepository.Update(_l);
                unitOfWork.Save();
                return JsonResultTrue("Motivazione Richiesta aggiornata correttamente");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult Elimina(int motivazionirichiestaId)
        {
            var model = new EliminaMotivazioneRichiesta();
            model.MotivazioniRichiestaId = motivazionirichiestaId;
            return AjaxView("Elimina", model);
        }

        [HttpPost]
        public ActionResult Elimina(DOM.Entitys.MotivazioniRichiesta model)
        {
            try
            {
                //se non usata
                unitOfWork.MotivazioniRichiestaRepository.Delete(model.MotivazioniRichiestaId);
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