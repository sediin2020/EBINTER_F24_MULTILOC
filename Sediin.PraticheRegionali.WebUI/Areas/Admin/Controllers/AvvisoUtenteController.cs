using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Sediin.MVC.HtmlHelpers;
using Sediin.PraticheRegionali.DOM.Entitys;
using Sediin.PraticheRegionali.WebUI.Areas.Admin.Models;
using Sediin.PraticheRegionali.WebUI.Areas.Backend.Controllers;
using Sediin.PraticheRegionali.WebUI.Controllers;
using Sediin.PraticheRegionali.WebUI.Filters;
using static Sediin.PraticheRegionali.WebUI.IdentityHelper;

namespace Sediin.PraticheRegionali.WebUI.Areas.Admin.Controllers
{
    [@Authorize(Roles = new Roles[] { Roles.Admin, Roles.Super })]
    public class AvvisoUtenteController : BaseController
    {
        #region ricerca

        public ActionResult Ricerca()
        {
            return AjaxView();
        }

        [HttpPost]
        public ActionResult Ricerca(AvvisoUtenteRicercaModel model, int? page)
        {
            var _query = unitOfWork.AvvisoUtenteRepository.Get(RicercaFilter(model));

            var _result = GeModelWithPaging<AvvisoUtenteRicercaViewModel, AvvisoUtente>(page, _query, model, 10);

            return AjaxView("RicercaList", _result);
        }

        public ActionResult RicercaExcel(AvvisoUtenteRicercaModel model)
        {
            var _query = from a in unitOfWork.AvvisoUtenteRepository.Get(RicercaFilter(model))
                         select new
                         {
                             //a.Data,
                             //a.Action,
                             //a.Username,
                             //a.Ruolo,
                             //a.Message,
                         };

            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(_query, "AvvisoUtente");
        }

        private Expression<Func<AvvisoUtente, bool>> RicercaFilter(AvvisoUtenteRicercaModel model)
        {
            return null;
        }

        #endregion

        public ActionResult Avviso(int? id = null)
        {
            try
            {
                AvvisoUtenteViewModel model = new AvvisoUtenteViewModel();

                if (id == null)
                {
                    model.DataInserimento = DateTime.Now;
                }
                else
                {
                    var _avviso = unitOfWork.AvvisoUtenteRepository.Get(x => x.AvvisoUtenteId == id).FirstOrDefault();

                    model = Reflection.CreateModel<AvvisoUtenteViewModel>(_avviso);
                    model.AvvisoUtenteRuoli = _avviso.AvvisoUtenteRuoli;
                }

                return AjaxView("Avviso", model);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Avviso(AvvisoUtenteViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                //cancella ruoli avviso se inseriti
                if (model.AvvisoUtenteId != 0)
                {
                    var _r = unitOfWork.AvvisoUtenteRuoliRepository.Get(x => x.AvvisoUtenteId == model.AvvisoUtenteId);

                    if (_r.Count() > 0)
                    {
                        foreach (var item in _r)
                        {
                            unitOfWork.AvvisoUtenteRuoliRepository.Delete(item);
                        }
                    }
                }

                //insert or update avviso
                AvvisoUtente _model = Reflection.CreateModel<AvvisoUtente>(model);
                _model.DataScadenza = _model.DataScadenza.HasValue ?
                    new DateTime(_model.DataScadenza.Value.Year, _model.DataScadenza.Value.Month, _model.DataScadenza.Value.Day, 23, 59, 59) : _model.DataScadenza;

                unitOfWork.AvvisoUtenteRepository.InsertOrUpdate(_model);
                unitOfWork.Save();

                foreach (var item in model.Ruolo.Where(x => x.Checked))
                {
                    unitOfWork.AvvisoUtenteRuoliRepository.InsertOrUpdate(new AvvisoUtenteRuoli
                    {
                        Ruolo = item.Nome,
                        AvvisoUtenteId = _model.AvvisoUtenteId
                    });
                    unitOfWork.Save();
                }

                return JsonResultTrue("Avviso utente " + (model.AvvisoUtenteId == 0 ? "inserito" : "aggiornato"));
            }
            catch (Exception)
            {
                throw;
            }

        }
    }
}