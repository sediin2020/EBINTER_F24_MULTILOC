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
    public class ContatoreAnnualeController : BaseController
    {
        // GET: Backend/Contatori Annuale
        public ActionResult Ricerca()
        {
            return AjaxView();
        }

        [HttpPost]
        public ActionResult Ricerca(ContatoreAnnualeSearchModel model, int? page)
        {
            var _query = unitOfWork.ContatoreAnnualeRepository.Get(RicercaFilter(model)).OrderBy(m => m.PraticheRegionaliImprese);

            var _result = GeModelWithPaging<ContatoriAnnualeModelRicercaViewModel, ContatoreAnnuale>(page, _query, model, 10);

            return AjaxView("RicercaList", _result);
        }

        private Expression<Func<ContatoreAnnuale, bool>> RicercaFilter(ContatoreAnnualeSearchModel model)
        {
            return x => model.PraticheRegionaliImprese != null ? x.PraticheRegionaliImprese.StartsWith(model.PraticheRegionaliImprese) : true;
        }

        public ActionResult RicercaExcel(ContatoreAnnualeRicercaModel model)
        {
            var _query = from a in unitOfWork.ContatoreAnnualeRepository.Get(RicercaFilter2(model))
                         select new
                         {
                             a.PraticheRegionaliImprese,
                             a.DataInizio,
                             a.DataFine,
                             a.TettoMassimoLordo,
                         };

            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(_query, "ErrorLogs");
        }

        private Expression<Func<ContatoreAnnuale, bool>> RicercaFilter(InsContatoreAnnuale model)
        {
            return x => (model.PraticheRegionaliImprese != null && model.PraticheRegionaliImprese != "" ? x.PraticheRegionaliImprese.Contains(model.PraticheRegionaliImprese) : true);
        }

        private Expression<Func<ContatoreAnnuale, bool>> RicercaFilter3(ContatoreAnnualeModel model)
        {
            return x => (model.PraticheRegionaliImprese != null && model.PraticheRegionaliImprese != "" ? x.PraticheRegionaliImprese.Contains(model.PraticheRegionaliImprese) : true);
        }

        private Expression<Func<ContatoreAnnuale, bool>> RicercaFilter2(ContatoreAnnualeRicercaModel model)
        {
            return null;
        }

        public ActionResult Nuovo()
        {
            return AjaxView("Nuovo");
        }

        [HttpPost]
        public ActionResult Nuovo(InsContatoreAnnuale model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                if (model.DataFine < model.DataInizio)
                {
                    throw new Exception("Non puoi mettere la data Inizio più grande della data fine.");
                }

                if (model.DataFine == null || model.DataInizio == null)
                {
                    throw new Exception("Data Inizio o Data Fine Obbligatorie.");
                }

                //check se Contatore esiste
                var _itemMaxDataFine = unitOfWork.ContatoreAnnualeRepository.Get(RicercaFilter(model)).OrderByDescending(m => m.DataFine).FirstOrDefault()?.DataFine;
                if (_itemMaxDataFine != null && _itemMaxDataFine >= model.DataInizio)
                {
                    throw new Exception("Contatore Annuale già presente con range di date differenti.");
                }

                //se non esiste
                var _nuovoContatoreAnnuale = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<ContatoreAnnuale>(model);
                _nuovoContatoreAnnuale.PraticheRegionaliImprese = model.PraticheRegionaliImprese;
                _nuovoContatoreAnnuale.DataInizio = model.DataInizio.GetValueOrDefault();
                _nuovoContatoreAnnuale.DataFine = model.DataFine.GetValueOrDefault();
                _nuovoContatoreAnnuale.TettoMassimoLordo = model.TettoMassimoLordo.GetValueOrDefault();

                unitOfWork.ContatoreAnnualeRepository.Insert(_nuovoContatoreAnnuale);
                unitOfWork.Save();
                return JsonResultTrue("Record Nuovo Contatore inserito correttamente");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }
        public ActionResult Modifica(int id)
        {
            var _ContatoreAnnuale = unitOfWork.ContatoreAnnualeRepository.Get(m => m.ContatoreAnnualeId == id).FirstOrDefault();
            var _l = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<ContatoreAnnualeModel>(_ContatoreAnnuale);
            return AjaxView("Modifica", _l);
        }

        [HttpPost]
        public ActionResult Modifica(ContatoreAnnualeModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                if (model.DataFine < model.DataInizio)
                {
                    throw new Exception("Non puoi mettere la data Inizio più grande della data fine.");
                }

                if (model.DataFine == null || model.DataInizio == null)
                {
                    throw new Exception("Data Inizio o Data Fine Obbligatorie.");
                }

                var _Items = unitOfWork.ContatoreAnnualeRepository.Get(m => m.PraticheRegionaliImprese == model.PraticheRegionaliImprese).ToList();
                var err = false;
                foreach (var item in _Items)
                {
                    if (item.ContatoreAnnualeId != model.ContatoreAnnualeId)
                    {
                        if (model.DataInizio >= item.DataInizio && model.DataInizio <= item.DataFine)
                        {
                            err = true;
                        }

                        if (model.DataFine >= item.DataInizio && model.DataFine <= item.DataFine)
                        {
                            err = true;
                        }

                        if (model.DataInizio < item.DataInizio && model.DataFine > item.DataFine)
                        {
                            err = true;
                        }
                    }

                }

                if (err)
                {
                    throw new Exception("Contatore Annuale già presente con range di date differenti.");
                }

                //var _item = unitOfWork.ContatoreAnnualeRepository.Get(RicercaFilter3(model)).OrderByDescending(m => m.DataFine).FirstOrDefault();
                //var _itemMaxDataFine = _item?.DataFine;
                //var _itemId = _item.ContatoreAnnualeId;
                //if (_itemMaxDataFine != null && _itemMaxDataFine >= model.DataInizio && _itemId != model.ContatoreAnnualeId)
                //{
                //    throw new Exception("Contatore Annuale già presente con range di date differenti.");
                //}

                var _l = unitOfWork.ContatoreAnnualeRepository.Get(m => m.ContatoreAnnualeId == model.ContatoreAnnualeId).FirstOrDefault();

                ////check se Tipologia esiste
                //var _Items = unitOfWork.ContatoreAnnualeRepository.Get(m => m.PraticheRegionaliImprese == model.PraticheRegionaliImprese).ToList();
                //if (_Items.Count > 0 && model.PraticheRegionaliImprese == _l.PraticheRegionaliImprese)
                //{
                //    foreach ( var item in _Items )
                //    {
                //        if (item.DataFine >= model.DataInizio && model.ContatoreAnnualeId != item.ContatoreAnnualeId)
                //        {
                //            throw new Exception("Contatore Annuale già presente con range di date differenti.");
                //        }

                //    }
                //}

                _l.PraticheRegionaliImprese = model.PraticheRegionaliImprese;
                _l.DataInizio = model.DataInizio.GetValueOrDefault();
                _l.DataFine = model.DataFine.GetValueOrDefault();
                _l.TettoMassimoLordo = model.TettoMassimoLordo.GetValueOrDefault();

                unitOfWork.ContatoreAnnualeRepository.Update(_l);
                unitOfWork.Save();
                return JsonResultTrue("Contatore Annuale aggiornato correttamente");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult Elimina(int contatoreannualeId)
        {
            var model = new EliminaContatoreAnnuale();
            model.contatoreannualeId = contatoreannualeId;
            return AjaxView("Elimina", model);
        }

        [HttpPost]
        public ActionResult Elimina(DOM.Entitys.ContatoreAnnuale model)
        {
            try
            {
                //var tipoRichieste = unitOfWork.ContatoreAnnualeRepository.Get(x => x.ContatoreAnnualeId == model.ContatoreAnnualeId);
                ////check se allegato è associato a tipo richiesta
                //if (tipoRichieste.Count() > 0)
                //{
                //    throw new Exception("Impossibile eliminare: allegato già associato a tipo richiesta");
                //}

                //var _item = unitOfWork.ContatoreAnnualeRepository.Get(m => m.ContatoreAnnualeId == model.ContatoreAnnualeId).FirstOrDefault();
                //if (_item.PraticheRegionaliImprese == "Dipendente" || _item.PraticheRegionaliImprese == "Azienda")
                //{
                //    throw new Exception("Impossibile eliminare il contatore in quanto Dipendente o Azienda." );
                //}

                //se non usata
                unitOfWork.ContatoreAnnualeRepository.Delete(model.ContatoreAnnualeId);
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