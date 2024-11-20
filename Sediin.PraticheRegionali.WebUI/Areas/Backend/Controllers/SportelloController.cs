using Sediin.PraticheRegionali.DOM.Entitys;
using Sediin.PraticheRegionali.WebUI.Areas.Backend.Models;
using Sediin.PraticheRegionali.WebUI.Controllers;
using Sediin.PraticheRegionali.WebUI.Filters;
using LambdaSqlBuilder;
using Sediin.MVC.HtmlHelpers;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using static Sediin.PraticheRegionali.WebUI.IdentityHelper;
using Sediin.PraticheRegionali.Utils;
using Sediin.PraticheRegionali.WebUI.Helpers;

namespace Sediin.PraticheRegionali.WebUI.Areas.Backend.Controllers
{
    [@Authorize]
    public class SportelloController : BaseController
    {
        #region ricerca

        [@Authorize(Roles = new Roles[] { Roles.Admin, Roles.Super })]
        public ActionResult Ricerca()
        {
            return AjaxView("Ricerca");
        }

        [HttpPost]
        [@Authorize(Roles = new Roles[] { Roles.Admin, Roles.Super })]
        public ActionResult Ricerca(SportelloRicercaModel model, int? page)
        {
            int totalRows = 0;
            var _query = unitOfWork.SportelloRepository.Get(ref totalRows, RicercaFilter2(model), model.Ordine, page, model.PageSize);
            //var _query = unitOfWork.SportelloRepository.Get(RicercaFilter(model));

            var _result = GeModelWithPaging<SportelloRicercaViewModel, Sportello>(page, _query, model, totalRows, model.PageSize);

            var _rolesVisibili = ConfigurationProvider.Instance.GetConfiguration().Roles.Where(x => (bool)x.Attivo);

            foreach (var item in _result.Result)
            {
                item.Ruolo = _rolesVisibili.FirstOrDefault(x => x.Rolename == item.Ruolo).FriendlyName;
            }

            return AjaxView("RicercaList", _result);
        }

        [@Authorize(Roles = new Roles[] { Roles.Admin, Roles.Super })]
        public ActionResult RicercaExcel(SportelloRicercaModel model)
        {
            var _query = from a in unitOfWork.SportelloRepository.Get(RicercaFilter(model)).OrderBy(r => r.RagioneSociale)
                         select new
                         {
                             a.RagioneSociale,
                             a.CodiceFiscalePIva,
                             a.Cognome,
                             a.Nome,
                             a.Telefono,
                             a.Email,
                             a.Pec,
                             a.Comune?.DENCOM,
                             a.Localita?.CAP,
                             a.Localita?.DENLOC,
                         };

            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(_query, "Consulenti");
        }

        private Expression<Func<Sportello, bool>> RicercaFilter(SportelloRicercaModel model)
        {
            TrimAll(model);

            return x =>
            ((model.SportelloRicercaModel_RagioneSociale != null ? x.RagioneSociale.Contains(model.SportelloRicercaModel_RagioneSociale) : true)
            && (model.SportelloRicercaModel_Ruolo != null ? x.Ruolo == model.SportelloRicercaModel_Ruolo : true)
            && (model.SportelloRicercaModel_CodiceFiscalePartitaIva != null ? x.CodiceFiscalePIva == model.SportelloRicercaModel_CodiceFiscalePartitaIva : true)
            && (model.SportelloRicercaModel_ComuneId != null ? x.ComuneId == model.SportelloRicercaModel_ComuneId : true));
        }

        private SqlLam<Sportello> RicercaFilter2(SportelloRicercaModel model)
        {
            TrimAll(model);

            var f = new SqlLam<Sportello>();

            if (!string.IsNullOrWhiteSpace(model.SportelloRicercaModel_RagioneSociale))
            {
                f.And(xd => xd.RagioneSociale.Contains(model.SportelloRicercaModel_RagioneSociale));
            }

            if (!string.IsNullOrWhiteSpace(model.SportelloRicercaModel_Ruolo))
            {
                f.And(xd => xd.Ruolo == model.SportelloRicercaModel_Ruolo);
            }

            if (!string.IsNullOrWhiteSpace(model.SportelloRicercaModel_CodiceFiscalePartitaIva))
            {
                f.And(xd => xd.CodiceFiscalePIva.Contains(model.SportelloRicercaModel_CodiceFiscalePartitaIva));
            }

            if (model.SportelloRicercaModel_ComuneId.HasValue)
            {
                f.And(x => x.ComuneId == model.SportelloRicercaModel_ComuneId);
            }

            return f.OrderBy(d => d.Cellulare);
        }

        #endregion

        [@Authorize(Roles = new Roles[] { Roles.Sp_CAF, Roles.Sp_Consulente, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac })]
        public async Task<ActionResult> Anagrafica(int? id = null, string ruolo = null)
        {
            try
            {
                if (id.GetValueOrDefault() == 0 && IsInRole(Roles.Azienda))
                {
                    var _role = GenericHelper.GetRoleFriendlyName(null, ruolo);
                    if (_role == null || !ruolo.StartsWith("sp_", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new Exception("Ruolo non valido");
                    }
                }

                Expression<Func<Sportello, bool>> _filter = x => x.SportelloId == id;

                //utente visualizza solo le informazioni sua
                if (IsInRole(new Roles[] { Roles.Sp_CAF, Roles.Sp_Consulente, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac }))
                {
                    _filter = x => x.CodiceFiscalePIva == User.Identity.Name;
                }

                var _outModel = new SportelloViewModel();

                var _result = unitOfWork.SportelloRepository.Get(_filter).FirstOrDefault();

                //prendere da AspNetUser dati precompilati
                var _user = await UserManager.FindByNameAsync(User.Identity.Name);

                if (_result != null)
                {
                    _outModel = Reflection.CreateModel<SportelloViewModel>(_result);
                    _outModel.DelegheSportelloDipendente = _result.DelegheSportelloDipendente;
                    _outModel.DelegheSportelloAzienda = _result.DelegheSportelloAzienda;
                    _outModel.InformazioniPersonaliCompilati = _user.InformazioniPersonaliCompilati;
                }
                else
                {
                    if (IsInRole(new Roles[] { Roles.Sp_CAF, Roles.Sp_Consulente, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac }))
                    {
                        _outModel = new SportelloViewModel
                        {
                            CodiceFiscalePIva = _user.UserName,
                            Cognome = _user.Cognome,
                            Nome = _user.Nome,
                            Email = _user.Email,
                            InformazioniPersonaliCompilati = false,
                            Ruolo = GetUserRole()
                        };
                    }
                    else
                    {
                        _outModel.Ruolo = ruolo;
                    }
                }

                return AjaxView("Anagrafica", _outModel);
            }
            catch (Exception ex)
            {
                return AjaxView("Error", new HandleErrorInfo(ex, "SportelloController", "Anagrafica"));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [@Authorize(Roles = new Roles[] { Roles.Sp_CAF, Roles.Sp_Consulente, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac })]
        public async Task<ActionResult> Anagrafica(SportelloViewModel model)
        {
            try
            {
                //utente aggiorna solo le informazioni sua
                if (IsInRole(new Roles[] { Roles.Sp_CAF, Roles.Sp_Consulente, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac }))
                {
                    model.CodiceFiscalePIva = User.Identity.Name;

                    Expression<Func<Sportello, bool>> _filter = x => x.CodiceFiscalePIva == User.Identity.Name;

                    var _result = unitOfWork.SportelloRepository.Get(_filter).FirstOrDefault();

                    if (_result != null)
                    {
                        model.SportelloId = _result.SportelloId;
                        model.CodiceFiscalePIva = _result.CodiceFiscalePIva;
                    }

                    model.Ruolo = GetUserRole();
                }

                //check consulente exists con codice fiscale
                var _sportelloExists = unitOfWork.SportelloRepository.Get(x => x.SportelloId != model.SportelloId && x.CodiceFiscalePIva.ToLower() == model.CodiceFiscalePIva.ToLower());
                if (_sportelloExists?.Count() > 0)
                {
                    throw new Exception("Codice Fiscale / Partita Iva già registrata");
                }

                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                //cast to consulenteCs model
                var _model = Reflection.CreateModel<Sportello>(model);
                _model.CodiceFiscalePIva = _model.CodiceFiscalePIva.ToUpper();
                unitOfWork.SportelloRepository.InsertOrUpdate(_model);
                unitOfWork.Save();

                //update flag InformazioniPersonaliCompilati
                if (IsInRole(new Roles[] { Roles.Sp_CAF, Roles.Sp_Consulente, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac }))
                {
                    var _user = await UserManager.FindByNameAsync(User.Identity.Name);

                    if (!_user.InformazioniPersonaliCompilati)
                    {
                        _user.InformazioniPersonaliCompilati = true;
                        await UserManager.UpdateAsync(_user);
                    }
                }

                return Json(new
                {
                    isValid = true,
                    sportelloId = _model.SportelloId,
                    message = "Anagrafica " + (model.SportelloId == 0 ? "inserita" : "aggiornata")
                });
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public JsonResult ListaSportelli(string phrase, int? sportelloId = null)
        {
            Expression<Func<Sportello, bool>> _filter = x =>
            ((phrase != null ? (x.Nome + " " + x.Cognome).Contains(phrase) : true)
            || (phrase != null ? x.CodiceFiscalePIva.Contains(phrase) : true));

            return GetListaSportelli(_filter);
        }

        private JsonResult GetListaSportelli(Expression<Func<Sportello, bool>> filter)
        {
            var _result = unitOfWork.SportelloRepository.Get(filter);

            if (_result.Count() > 0)
            {
                return Json(_result
                       .OrderBy(p => p.Cognome == null || p.Cognome == "")
                       .ThenBy(p => p.Nome == null || p.Nome == "")
                       .ThenBy(p => p.CodiceFiscalePIva)
                       .Select(x => new { x.SportelloId, x.CodiceFiscalePIva, Nominativo = x.Nome + " " + x.Cognome + " - " + x.CodiceFiscalePIva }), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

    }
}