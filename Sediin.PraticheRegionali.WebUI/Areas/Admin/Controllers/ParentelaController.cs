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
    public class ParentelaController : BaseController
    {
        // GET: Backend/Parentela
        public ActionResult Ricerca()
        {
            return AjaxView();
        }

        [HttpPost]
        public ActionResult Ricerca(ParentelaSearchModel model, int? page)
        {
            var _query = unitOfWork.ParentelaRepository.Get(RicercaFilter(model)).OrderBy(m => m.Descrizione);

            var _result = GeModelWithPaging<ParentelaModelRicercaViewModel, Parentela>(page, _query, model, 10);

            return AjaxView("RicercaList", _result);
        }

        private Expression<Func<Parentela, bool>> RicercaFilter(ParentelaSearchModel model)
        {
            ;
            return x => model.Descrizione != null ? x.Descrizione.StartsWith(model.Descrizione) : true;
        }

        public ActionResult RicercaExcel(ParentelaRicercaModel model)
        {
            var _query = from a in unitOfWork.ParentelaRepository.Get(RicercaFilter2(model))
                         select new
                         {
                             a.Descrizione,
                             a.Note,
                         };

            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(_query, "ErrorLogs");
        }

        private Expression<Func<Parentela, bool>> RicercaFilter2(ParentelaRicercaModel model)
        {
            return null;
        }

        public ActionResult Nuovo()
        {
            return AjaxView("Nuovo");
        }

        [HttpPost]
        public ActionResult Nuovo(InsParentela model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                //check se Parentela esiste
                var _Parentela = unitOfWork.ParentelaRepository.Get(m => m.Descrizione == model.Descrizione).ToList();
                if (_Parentela.Count > 0)
                {
                    throw new Exception("Parentela già presente.");
                }

                //se non esiste
                var _nuovoParentela = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<Parentela>(model);
                _nuovoParentela.Descrizione = model.Descrizione;
                _nuovoParentela.Note = model.Note;
                unitOfWork.ParentelaRepository.Insert(_nuovoParentela);
                unitOfWork.Save();
                return JsonResultTrue("Parentela inserita correttamente");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult Modifica(int id)
        {
            var _Parentela = unitOfWork.ParentelaRepository.Get(m => m.ParentelaId == id).FirstOrDefault();
            var _l = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<ParentelaModel>(_Parentela);
            return AjaxView("Modifica", _l);
        }

        [HttpPost]
        public ActionResult Modifica(ParentelaModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                var _l = unitOfWork.ParentelaRepository.Get(m => m.ParentelaId == model.ParentelaId).FirstOrDefault();

                //check se Motivazione esiste
                //var _Parentela = unitOfWork.ParentelaRepository.Get(m => m.Descrizione == model.Descrizione).ToList();
                //var _descr = _Parentela.FirstOrDefault().Descrizione;
                //if (_Parentela.Count > 0 && model.Descrizione == _descr)
                //{
                //    throw new Exception("Parentela già presente.");
                //}

                //se non esiste allora modifico
                _l.Descrizione = model.Descrizione;
                _l.Note = model.Note;
                unitOfWork.ParentelaRepository.Update(_l);
                unitOfWork.Save();
                return JsonResultTrue("Parentela aggiornata correttamente");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult Elimina(int parentelaId)
        {
            var model = new EliminaParentela();
            model.ParentelaId = parentelaId;
            return AjaxView("Elimina", model);
        }

        [HttpPost]
        public ActionResult Elimina(DOM.Entitys.Parentela model)
        {
            try
            {

                //se non usata
                unitOfWork.ParentelaRepository.Delete(model.ParentelaId);
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