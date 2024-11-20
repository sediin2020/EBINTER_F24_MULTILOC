﻿using System;
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
    public class MotivazioniController : BaseController
    {
        // GET: Backend/Motivazioni
        public ActionResult Ricerca()
        {
            return AjaxView();
        }

        [HttpPost]
        public ActionResult Ricerca(MotivazioniModel model, int? page)
        {
            var _query = unitOfWork.MotivazioniRepository.Get(RicercaFilter(model)).OrderBy(m => m.Motivazione);

            var _result = GeModelWithPaging<MotivazioniModelRicercaViewModel, Motivazioni>(page, _query, model, 10);

            return AjaxView("RicercaList", _result);
        }

        private Expression<Func<Motivazioni, bool>> RicercaFilter(MotivazioniModel model)
        {
            ;
            return x => (model.Motivazione != null ? x.Motivazione.StartsWith(model.Motivazione) : true)
            && (model.MotivazioniRicercaModel_StatoPraticaId != null ? x.StatoPraticaId == model.MotivazioniRicercaModel_StatoPraticaId : true);

        }

        public ActionResult RicercaExcel(MotivazioniRicercaModel model)
        {
            var _query = from a in unitOfWork.MotivazioniRepository.Get(RicercaFilter2(model))
                         select new
                         {
                             a.Motivazione,
                             a.Note,
                             a.StatoPraticaId,
                         };

            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(_query, "ErrorLogs");
        }

        private Expression<Func<Motivazioni, bool>> RicercaFilter2(MotivazioniRicercaModel model)
        {
            return null;
        }

        //public ActionResult GetStatoPraticaId(string phrase)
        //{
        //    Expression<Func<StatoPratica, bool>> _filter = x =>
        //                (phrase != null ? (x.Descrizione).StartsWith(phrase) : true);

        //    var _result = unitOfWork.StatoPraticaRepository.Get(_filter);

        //    if (_result.Count() > 0)
        //    {
        //        return Json(_result.Select(x => new { x.StatoPraticaId, x.Descrizione }), JsonRequestBehavior.AllowGet);
        //    }
        //    else
        //    {
        //        return Json("", JsonRequestBehavior.AllowGet);
        //    }
        //}

        public ActionResult Nuovo()
        {

            var model = new InsMotivazioni();
            model.StatoPratica = unitOfWork.StatoPraticaRepository.Get().ToList();

            return AjaxView("Nuovo", model);
        }

        [HttpPost]
        public ActionResult Nuovo(InsMotivazioni model)
        {
            try
            {
                //check se Motivazione esiste
                var _Motivazioni = unitOfWork.MotivazioniRepository.Get(m => m.Motivazione == model.Motivazione).ToList();
                if (_Motivazioni.Count > 0)
                {
                    throw new Exception("Motivazione già presente.");
                }

                //se non esiste
                var _nuovoMotivazioni = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<Motivazioni>(model);
                _nuovoMotivazioni.StatoPraticaId = model.StatoPraticaId;
                _nuovoMotivazioni.Motivazione = model.Motivazione;
                _nuovoMotivazioni.Note = model.Note;
                unitOfWork.MotivazioniRepository.Insert(_nuovoMotivazioni);
                unitOfWork.Save();
                return JsonResultTrue("Motivazione inserito correttamente");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult Modifica(int id)
        {
            var _Motivazioni = unitOfWork.MotivazioniRepository.Get(m => m.MotivazioniId == id).FirstOrDefault();

            var _l = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<InsMotivazioni>(_Motivazioni);
            _l.StatoPratica = unitOfWork.StatoPraticaRepository.Get();

            return AjaxView("Modifica", _l);
        }

        [HttpPost]
        public ActionResult Modifica(InsMotivazioni model)
        {
            try
            {
                var _l = unitOfWork.MotivazioniRepository.Get(m => m.MotivazioniId == model.MotivazioniId).FirstOrDefault();

                //check se Motivazione esiste
                //var _Motivazioni = unitOfWork.MotivazioniRepository.Get(m => m.Motivazione == model.Motivazione).ToList();
                //var _descr = _Motivazioni.FirstOrDefault().Motivazione;
                //if (_Motivazioni.Count > 0 && model.Motivazione == _descr)
                //{
                //    throw new Exception("Motivazione già presente.");
                //}

                //se non esiste allora modifico
                _l.StatoPraticaId = model.StatoPraticaId;
                _l.Motivazione = model.Motivazione;
                _l.Note = model.Note;
                unitOfWork.MotivazioniRepository.Update(_l);
                unitOfWork.Save();
                return JsonResultTrue("Motivazione aggiornata correttamente");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult Elimina(int motivazioniId)
        {
            var model = new EliminaMotivazione();
            model.MotivazioniId = motivazioniId;
            return AjaxView("Elimina", model);
        }

        [HttpPost]
        public ActionResult Elimina(DOM.Entitys.Motivazioni model)
        {
            try
            {
                var _regionaliimpresa = unitOfWork.PraticheRegionaliImpreseStatoPraticaStoricoRepository.Get(x => x.MotivazioniId == model.MotivazioniId);
                //check se allegato è associato al requisito
                if (_regionaliimpresa.Count() > 0)
                {
                    throw new Exception("Impossibile eliminare: motivazione già associato allo storico delle pratiche regionali stato pratica ");
                }

                //se non usata
                unitOfWork.MotivazioniRepository.Delete(model.MotivazioniId);
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