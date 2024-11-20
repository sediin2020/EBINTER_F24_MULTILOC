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
    public class TipoImpiegoController : BaseController
    {
        // GET: Backend/Tipo Impiego
        public ActionResult Ricerca()
        {
            return AjaxView();
        }

        [HttpPost]
        public ActionResult Ricerca(TipoImpiegoModel model, int? page)
        {
            var _query = unitOfWork.TipoImpiegoRepository.Get(RicercaFilter(model)).OrderBy(m => m.Descrizione);

            var _result = GeModelWithPaging<TipoImpiegoModelRicercaViewModel, TipoImpiego>(page, _query, model, 10);

            return AjaxView("RicercaList", _result);
        }

        private Expression<Func<TipoImpiego, bool>> RicercaFilter(TipoImpiegoModel model)
        {
            return x => (model.Descrizione != null ? x.Descrizione.StartsWith(model.Descrizione) : true);
        }

        public ActionResult RicercaExcel(TipoImpiegoRicercaModel model)
        {
            var _query = from a in unitOfWork.TipoImpiegoRepository.Get(RicercaFilter2(model))
                         select new
                         {
                             a.Descrizione,
                         };

            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(_query, "ErrorLogs");
        }

        private Expression<Func<TipoImpiego, bool>> RicercaFilter2(TipoImpiegoRicercaModel model)
        {
            return null;
        }

        public ActionResult Nuovo()
        {
            return AjaxView("Nuovo");
        }

        [HttpPost]
        public ActionResult Nuovo(InsTipoImpiego model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                //check se Parentela esiste
                var _TipoImpiego = unitOfWork.TipoImpiegoRepository.Get(m => m.Descrizione == model.Descrizione).ToList();
                if (_TipoImpiego.Count > 0)
                {
                    throw new Exception("Tipo Impiego già presente.");
                }

                //se non esiste
                var _nuovoTipoImpiego = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<TipoImpiego>(model);
                _nuovoTipoImpiego.Descrizione = model.Descrizione;
                unitOfWork.TipoImpiegoRepository.Insert(_nuovoTipoImpiego);
                unitOfWork.Save();
                return JsonResultTrue("Record Tipo Impiego inserito correttamente");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult Modifica(int id)
        {
            var _TipoImpiego = unitOfWork.TipoImpiegoRepository.Get(m => m.TipoImpiegoId == id).FirstOrDefault();
            var _l = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<TipoImpiegoModel>(_TipoImpiego);
            return AjaxView("Modifica", _l);
        }

        [HttpPost]
        public ActionResult Modifica(TipoImpiegoModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                var _l = unitOfWork.TipoImpiegoRepository.Get(m => m.TipoImpiegoId == model.TipoImpiegoId).FirstOrDefault();

                //check se Tipo Impiego esiste
                var _TipoImpiego = unitOfWork.TipoImpiegoRepository.Get(m => m.Descrizione == model.Descrizione).ToList();
                var _descr = _TipoImpiego.FirstOrDefault().Descrizione;
                if (_TipoImpiego.Count > 0 && model.Descrizione == _descr)
                {
                    throw new Exception("Tipo Impiego già presente.");
                }

                //se non esiste allora modifico
                _l.Descrizione = model.Descrizione;
                unitOfWork.TipoImpiegoRepository.Update(_l);
                unitOfWork.Save();
                return JsonResultTrue("Tipo Impiego aggiornato correttamente");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult Elimina(int tipoimpiegoId)
        {
            var model = new EliminaTipoImpiego();
            model.TipoImpiegoId = tipoimpiegoId;
            return AjaxView("Elimina", model);
        }

        [HttpPost]
        public ActionResult Elimina(DOM.Entitys.TipoImpiego model)
        {
            try
            {

                //se non usata
                unitOfWork.TipoImpiegoRepository.Delete(model.TipoImpiegoId);
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