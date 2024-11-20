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
    public class TempoLavoroController : BaseController
    {
        // GET: Backend/Tempo Lavoro
        public ActionResult Ricerca()
        {
            return AjaxView();
        }

        [HttpPost]
        public ActionResult Ricerca(TempoLavoroSearchModel model, int? page)
        {
            var _query = unitOfWork.TempoLavoroRepository.Get(RicercaFilter(model)).OrderBy(m => m.Descrizione);

            var _result = GeModelWithPaging<TempoLavoroModelRicercaViewModel, TempoLavoro>(page, _query, model, 10);

            return AjaxView("RicercaList", _result);
        }

        private Expression<Func<TempoLavoro, bool>> RicercaFilter(TempoLavoroSearchModel model)
        {
            return x => model.Descrizione != null ? x.Descrizione.StartsWith(model.Descrizione) : true;
        }

        public ActionResult RicercaExcel(TempoLavoroRicercaModel model)
        {
            var _query = from a in unitOfWork.TempoLavoroRepository.Get(RicercaFilter2(model))
                         select new
                         {
                             a.Descrizione,
                             a.TempoPieno,
                         };

            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(_query, "ErrorLogs");
        }

        private Expression<Func<TempoLavoro, bool>> RicercaFilter2(TempoLavoroRicercaModel model)
        {
            return null;
        }

        public ActionResult Nuovo()
        {
            return AjaxView("Nuovo");
        }

        [HttpPost]
        public ActionResult Nuovo(InsTempoLavoro model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                //check se Parentela esiste
                var _TempoLavoro = unitOfWork.TempoLavoroRepository.Get(m => m.Descrizione == model.Descrizione).ToList();
                if (_TempoLavoro.Count > 0)
                {
                    throw new Exception("Tempo Lavoro già presente.");
                }

                //se non esiste
                var _nuovoTempoLavoro = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<TempoLavoro>(model);
                _nuovoTempoLavoro.Descrizione = model.Descrizione;
                if (model.TempoPieno != null)
                {
                    _nuovoTempoLavoro.TempoPieno = model.TempoPieno;
                }
                else
                {
                    _nuovoTempoLavoro.TempoPieno = false;
                }
                unitOfWork.TempoLavoroRepository.Insert(_nuovoTempoLavoro);
                unitOfWork.Save();
                return JsonResultTrue("Record Tempo Lavoro inserito correttamente");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult Modifica(int id)
        {
            var _TempoLavoro = unitOfWork.TempoLavoroRepository.Get(m => m.TempoLavoroId == id).FirstOrDefault();
            var _l = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<TempoLavoroModel>(_TempoLavoro);
            return AjaxView("Modifica", _l);
        }

        [HttpPost]
        public ActionResult Modifica(TempoLavoroModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                var _l = unitOfWork.TempoLavoroRepository.Get(m => m.TempoLavoroId == model.TempoLavoroId).FirstOrDefault();

                //check se Tempo Lavoro esiste
                //var _TempoLavoro = unitOfWork.TempoLavoroRepository.Get(m => m.Descrizione == model.Descrizione).ToList();
                //var _decscr = _TempoLavoro.FirstOrDefault().Descrizione;
                //if (_TempoLavoro.Count > 0 && model.Descrizione == _decscr)
                //{
                //    throw new Exception("Tempo Lavoro già presente.");
                //}

                //se non esiste allora modifico
                _l.Descrizione = model.Descrizione;
                if (model.TempoPieno != null)
                {
                    _l.TempoPieno = model.TempoPieno;
                }
                else
                {
                    _l.TempoPieno = false;
                }
                unitOfWork.TempoLavoroRepository.Update(_l);
                unitOfWork.Save();
                return JsonResultTrue("Tempo Lavoro aggiornato correttamente");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult Elimina(int tempolavoroId)
        {
            var model = new EliminaTempoLavoro();
            model.TempoLavoroId = tempolavoroId;
            return AjaxView("Elimina", model);
        }

        [HttpPost]
        public ActionResult Elimina(DOM.Entitys.TempoLavoro model)
        {
            try
            {

                //se non usata
                unitOfWork.TempoLavoroRepository.Delete(model.TempoLavoroId);
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