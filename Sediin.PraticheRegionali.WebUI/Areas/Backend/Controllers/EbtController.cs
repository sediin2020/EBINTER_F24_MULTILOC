using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web.Mvc;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;
using LambdaSqlBuilder;
using Microsoft.AspNet.Identity;
using Sediin.MVC.HtmlHelpers;
using Sediin.PraticheRegionali.DOM.Entitys;
using Sediin.PraticheRegionali.WebUI.Areas.Backend.Models;
using Sediin.PraticheRegionali.WebUI.Controllers;
using Sediin.PraticheRegionali.WebUI.Filters;
using Sediin.PraticheRegionali.WebUI.Models;
using static Sediin.PraticheRegionali.WebUI.IdentityHelper;

namespace Sediin.PraticheRegionali.WebUI.Areas.Backend.Controllers
{
    [@Authorize]
    public class EbtController : BaseController
    {

        [@Authorize(Roles = new Roles[] { Roles.Admin, Roles.Sp_Ebinter })]
        public async Task<ActionResult> Ricerca()
        {
            EbtRicercaModel model = new EbtRicercaModel();
            return AjaxView("Ricerca", model);
        }

        [HttpPost]
        [@Authorize(Roles = new Roles[] { Roles.Admin, Roles.Sp_Ebinter })]
        public ActionResult Ricerca(EbtRicercaModel model, int? page)
        {

            int totalRows = 0;

            var _query = unitOfWork.EbtRepository.Get(ref totalRows, RicercaFilter2(model), model.Ordine, page, model.PageSize);

            var _result = GeModelWithPaging<EbtRicercaViewModel, Ebt>(page, _query, model, totalRows, model.PageSize);

            return AjaxView("RicercaList", _result);
        }

        [@Authorize(Roles = new Roles[] { Roles.Admin, Roles.Sp_Ebinter })]
        public ActionResult RicercaExcel(EbtRicercaModel model)
        {
            var _query = from a in unitOfWork.EbtRepository.Get(RicercaFilter(model)).OrderBy(r => r.Regione)
                         select new
                         {
                             a.Regione,
                             //a.Comune,
                             a.Provincia,
                             a.ReferenteNome,
                             a.ReferenteCognome,
                             a.ReferenteEmail,
                             //a.Comune?.DENCOM,
                             //a.Localita?.CAP,
                             //a.Localita?.DENLOC,
                         };

            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(_query, "EBT");
        }

        private SqlLam<Ebt> RicercaFilter2(EbtRicercaModel model)
        {

            TrimAll(model);

            var f = new SqlLam<Ebt>();



            if (model.EbtRicercaModel_RegioneId.HasValue)
            {
                f.And(x => x.RegioneId == model.EbtRicercaModel_RegioneId);
            }

            return f;
        }

        private Expression<Func<Ebt, bool>> RicercaFilter(EbtRicercaModel model)
        {
            int? _sportelloId = null;

            if (IsInRole(new Roles[] { Roles.Admin, Roles.Sp_Ebinter }))
            {
                _sportelloId = GetSportelloId.Value;
            }

            TrimAll(model);

            return x =>
               model.EbtRicercaModel_RegioneId != null ? x.RegioneId == model.EbtRicercaModel_RegioneId : true;
        }



        [@Authorize(Roles = new Roles[] { Roles.Admin, Roles.Sp_Ebinter })]
        public async Task<ActionResult> Anagrafica(int? id = null)
        {
            try
            {
                var model = new EbtViewModel();

                Ebt _ebt = null;

                ApplicationUser _userSportello = null;

                ApplicationUser _userAzienda = null;


                _ebt = unitOfWork.EbtRepository.Get(x => x.EbtId == id)?.FirstOrDefault();
                model = Reflection.CreateModel<EbtViewModel>(_ebt);

                if (_ebt != null)
                {
                    model.IbanStorico = _ebt.IbanStorico;
                    model.F24Percentuale = _ebt.F24Percentuale;
                    model.MultiLocPercentuale = _ebt.MultiLocPercentuale;
                    model.F24_Percentuale = _ebt.F24_Percentuale/100; 
                }

                //se ebt e già inserita
                if (_ebt != null)
                {
                    //_userAzienda = await UserManager.FindByNameAsync(_ebt.MatricolaInps);


                    model.Regione = _ebt.Regione;
                    model.Provincia = _ebt.Provincia;
                    //model.Comune = _ebt.Comune;
                    //model.Localita = _ebt.Localita;
                }

                return AjaxView("Ebt", model);
            }
            catch (Exception ex)
            {
                return AjaxView("Error", new HandleErrorInfo(ex, "EbtController", "Anagrafica"));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [@Authorize(Roles = new Roles[] { Roles.Admin, Roles.Sp_Ebinter })]
        public async Task<ActionResult> Anagrafica(EbtViewModel model)
        {
            try
            {
                Ebt _ebt = null;

                ApplicationUser _userSportello = null;

                model.Data_Modifica = DateTime.Now;

                _ebt = unitOfWork.EbtRepository.Get(x => x.EbtId == model.EbtId).FirstOrDefault();

                //prendo ebt inserita
                //_ebt = unitOfWork.EbtRepository.Get(x => x.EbtId == model.EbtId).FirstOrDefault();

                //se la Ebt esiste
                if (_ebt != null)
                {
                    model.Data_Inserimento = _ebt.Data_Inserimento;
                }

                //check che non ce una ebt inserita con la stessa regione provincia
                _ebt = unitOfWork.EbtRepository.Get(x => x.Regione == model.Regione && x.Provincia != model.Provincia).FirstOrDefault();
                if (_ebt != null)
                {
                    throw new Exception("EBT Inps già registrato");
                }

                if (model.Sospeso && !model.Data_Sospensione.HasValue)
                    ModelState.AddModelError("Data_Sospensione", "Data sospensione obbilgatorio");

                if (!model.Sospeso && model.Data_Sospensione.HasValue)
                    model.Data_Sospensione = null;

                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                //cast to ebt model
                var _resultModel = Reflection.CreateModel<Ebt>(model);
                _resultModel.F24_Percentuale = model.MultiLoc_Percentuale / 100;
                _resultModel.MultiLoc_Percentuale=model.MultiLoc_Percentuale / 100;
                unitOfWork.EbtRepository.InsertOrUpdate(_resultModel);

                /////////////////////////////////////
                //  INIZIO STORICIZZATIONE 
                ////////////////////////////////////

                if (model.Iban_Operativo_Old != model.Iban_Operativo || model.Iban_Transitorio_Old != model.Iban_Transitorio)
                {

                    var _storico = new IbanStorico
                    {
                        EbtId = model.EbtId,
                        DataInserimento = DateTime.Now,
                        Iban_Operativo = (model.Iban_Operativo_Old != model.Iban_Operativo) ? model.Iban_Operativo_Old : null,
                        Iban_transitorio = (model.Iban_Transitorio_Old != model.Iban_Transitorio) ? model.Iban_Transitorio_Old : null
                    };
                    unitOfWork.IbanStoricoRepository.InsertOrUpdate(_storico);
                }

                if (model.F24_Percentuale_Old != model.F24_Percentuale )
                {

                    var _storico = new F24Percentuale
                    {
                        EbtId = model.EbtId,
                        DataInserimento = DateTime.Now,
                        F24 = model.F24_Percentuale_Old, 
                    };
                    unitOfWork.F24PercentualeRepository.InsertOrUpdate(_storico);
                }
                if (model.MultiLoc_Percentuale_Old != model.MultiLoc_Percentuale)
                {

                    var _storico = new MultiLocPercentuale
                    {
                        EbtId = model.EbtId,
                        DataInserimento = DateTime.Now,
                        MultiLoc = model.MultiLoc_Percentuale_Old,
                    };
                    unitOfWork.MultiLocPercentualeRepository.InsertOrUpdate(_storico);
                }

                /////////////////////////////////////
                //  FINE STORICIZZATIONE 
                ////////////////////////////////////

                unitOfWork.Save(false);


                return Json(new
                {
                    isValid = true,
                    ebtId = _resultModel.EbtId,
                    message = "EBT " + (model.EbtId == 0 ? "inserito" : "aggiornato")
                });
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }



        private JsonResult GetListaAziende(Expression<Func<Ebt, bool>> filter)
        {
            var _result = unitOfWork.EbtRepository.Get(filter);

            if (_result.Count() > 0)
            {
                return Json(_result
                       .OrderBy(p => p.Regione == null)
                       .ThenBy(p => p.Provincia)
                       .Select(x => new { x.EbtId, x.Regione }).Distinct(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

    }
}