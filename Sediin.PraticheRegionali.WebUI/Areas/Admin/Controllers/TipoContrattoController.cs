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
    public class TipoContrattoController : BaseController
    {
        // GET: Backend/Tipo Contratto
        public ActionResult Ricerca()
        {
            return AjaxView();
        }

        [HttpPost]
        public ActionResult Ricerca(TipoContrattoSearchModel model, int? page)
        {
            var _query = unitOfWork.TipoContrattoRepository.Get(RicercaFilter(model)).OrderBy(m => m.Descrizione);

            var _result = GeModelWithPaging<TipoContrattoModelRicercaViewModel, TipoContratto>(page, _query, model, 10);

            return AjaxView("RicercaList", _result);
        }

        private Expression<Func<TipoContratto, bool>> RicercaFilter(TipoContrattoSearchModel model)
        {
            return x => (model.Descrizione != null ? x.Descrizione.StartsWith(model.Descrizione) : true);
        }

        public ActionResult RicercaExcel(TipoContrattoRicercaModel model)
        {
            var _query = from a in unitOfWork.TipoContrattoRepository.Get(RicercaFilter2(model))
                         select new
                         {
                             a.Descrizione,
                         };

            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(_query, "ErrorLogs");
        }

        private Expression<Func<TipoContratto, bool>> RicercaFilter2(TipoContrattoRicercaModel model)
        {
            return null;
        }

        public ActionResult Nuovo()
        {
            return AjaxView("Nuovo");
        }

        [HttpPost]
        public ActionResult Nuovo(InsTipoContratto model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                //check se Parentela esiste
                var _TipoContratto = unitOfWork.TipoContrattoRepository.Get(m => m.Descrizione == model.Descrizione).ToList();
                if (_TipoContratto.Count > 0)
                {
                    throw new Exception("Tipo Contratto già presente.");
                }

                //se non esiste
                var _nuovoTipoContratto = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<TipoContratto>(model);
                _nuovoTipoContratto.Descrizione = model.Descrizione;
                unitOfWork.TipoContrattoRepository.Insert(_nuovoTipoContratto);
                unitOfWork.Save();
                return JsonResultTrue("Record Tipo Contratto inserito correttamente");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult Modifica(int id)
        {
            var _TipoContratto = unitOfWork.TipoContrattoRepository.Get(m => m.TipoContrattoId == id).FirstOrDefault();
            var _l = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<TipoContrattoModel>(_TipoContratto);
            return AjaxView("Modifica", _l);
        }

        [HttpPost]
        public ActionResult Modifica(TipoContrattoModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                var _l = unitOfWork.TipoContrattoRepository.Get(m => m.TipoContrattoId == model.TipoContrattoId).FirstOrDefault();

                //check se Tipo Contratto esiste
                var _TipoContratto = unitOfWork.TipoContrattoRepository.Get(m => m.Descrizione == model.Descrizione).ToList();
                var _descr = _TipoContratto.FirstOrDefault().Descrizione;
                if (_TipoContratto.Count > 0 && model.Descrizione == _descr)
                {
                    throw new Exception("Tipo Contratto già presente.");
                }

                //se non esiste allora modifico
                _l.Descrizione = model.Descrizione;
                unitOfWork.TipoContrattoRepository.Update(_l);
                unitOfWork.Save();
                return JsonResultTrue("Tipo Contratto aggiornato correttamente");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult Elimina(int tipocontrattoId)
        {
            var model = new EliminaTipoContratto();
            model.TipoContrattoId = tipocontrattoId;
            return AjaxView("Elimina", model);
        }

        [HttpPost]
        public ActionResult Elimina(DOM.Entitys.TipoContratto model)
        {
            try
            {

                //se non usata
                unitOfWork.TipoContrattoRepository.Delete(model.TipoContrattoId);
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