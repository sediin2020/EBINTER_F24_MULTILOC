using System;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web.Mvc;
using LambdaSqlBuilder;
using Sediin.MVC.HtmlHelpers;
using Sediin.PraticheRegionali.DOM.Entitys;
using Sediin.PraticheRegionali.WebUI.Areas.Backend.Models;
using Sediin.PraticheRegionali.WebUI.Controllers;
using Sediin.PraticheRegionali.WebUI.Filters;
using static Sediin.PraticheRegionali.WebUI.IdentityHelper;

namespace Sediin.PraticheRegionali.WebUI.Areas.Backend.Controllers
{
    [@Authorize]
    public class DipendenteController : BaseController
    {
        public string PathCartaIdentita { get => "Documenti\\Sportello\\{0}\\CartaIdentita"; private set { } }
        public string PathDelegheDipendente { get => "Documenti\\Sportello\\{0}\\Delega\\Dipendente"; private set { } }
        public string PathCartaIdentitaDipendente { get => "Documenti\\Dipendente\\{0}\\CartaIdentita"; private set { } }
        public string PathAltroDipendente { get => "Documenti\\Dipendente\\{0}\\Altro"; private set { } }

        #region ricerca

        [@Authorize(Roles = new Roles[] { Roles.Sp_Consulente, Roles.Sp_CAF, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac })]
        public ActionResult Ricerca()
        {
            return AjaxView();
        }

        [HttpPost]
        [@Authorize(Roles = new Roles[] { Roles.Sp_Consulente, Roles.Sp_CAF, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac })]
        public ActionResult Ricerca(DipendenteRicercaModel model, int? page)
        {
            int totalRows = 0;
            var _query = unitOfWork.DipendenteRepository.Get(ref totalRows, RicercaFilter2(model), model.Ordine, page, model.PageSize);
            // var _query = await unitOfWork.DipendenteRepository.GetAsync(RicercaFilter(model));

            var _result = GeModelWithPaging<DipendenteRicercaViewModel, Dipendente>(page, _query, model, totalRows, model.PageSize);

            return AjaxView("RicercaList", _result);
        }

        [@Authorize(Roles = new Roles[] { Roles.Sp_Consulente, Roles.Sp_CAF, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac })]
        public ActionResult RicercaExcel(DipendenteRicercaModel model)
        {
            var _query = from a in unitOfWork.DipendenteRepository.Get(RicercaFilter(model)).OrderBy(r => r.Cognome)
                         select new
                         {
                             a.Cognome,
                             a.Nome,
                             a.CodiceFiscale,
                             a.Datanascita,
                             a.Email,
                             a.Cellulare,
                             a.Comune?.DENCOM,
                             a.Localita?.CAP,
                             a.Localita?.DENLOC,
                         };

            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(_query, "Dipendenti");
        }

        private Expression<Func<Dipendente, bool>> RicercaFilter(DipendenteRicercaModel model)
        {
            int? sportelloId = null;

            if (IsInRole(new Roles[] { Roles.Sp_Consulente, Roles.Sp_CAF, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac }))
            {
                sportelloId = GetSportelloId.Value;
            }

            TrimAll(model);

            return x =>
            (sportelloId.HasValue ? (x.SportelloId == sportelloId) : true)
            && (model.DipendenteRicercaModel_GestitoSportello != null ? model.DipendenteRicercaModel_GestitoSportello == true
            ? x.SportelloId != null : x.SportelloId == null : true)
            && (model.DipendenteRicercaModel_Cognome != null ? x.Cognome.Contains(model.DipendenteRicercaModel_Cognome) : true
            && (model.DipendenteRicercaModel_Nome != null ? x.Nome.Contains(model.DipendenteRicercaModel_Nome) : true)
            && (model.DipendenteRicercaModel_CodiceFiscale != null ? x.CodiceFiscale == model.DipendenteRicercaModel_CodiceFiscale : true)
            && (model.DipendenteRicercaModel_ComuneId != null ? x.ComuneId == model.DipendenteRicercaModel_ComuneId : true));
        }

        private SqlLam<Dipendente> RicercaFilter2(DipendenteRicercaModel model)
        {
            int? sportelloId = null;

            if (IsInRole(new Roles[] { Roles.Sp_CAF, Roles.Sp_Consulente, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac }))
            {
                sportelloId = GetSportelloId.Value;
            }

            TrimAll(model);

            var f = new SqlLam<Dipendente>();

            if (sportelloId.HasValue)
            {
                f.And(x => x.SportelloId == sportelloId);
            }

            if (!string.IsNullOrWhiteSpace(model.DipendenteRicercaModel_Cognome))
            {
                f.And(xd => xd.Cognome.Contains(model.DipendenteRicercaModel_Cognome));
            }

            if (!string.IsNullOrWhiteSpace(model.DipendenteRicercaModel_Nome))
            {
                f.And(xd => xd.Nome.Contains(model.DipendenteRicercaModel_Nome));
            }

            if (!string.IsNullOrWhiteSpace(model.DipendenteRicercaModel_CodiceFiscale))
            {
                f.And(xd => xd.CodiceFiscale.Contains(model.DipendenteRicercaModel_CodiceFiscale));
            }

            if (model.DipendenteRicercaModel_ComuneId.HasValue)
            {
                f.And(x => x.ComuneId == model.DipendenteRicercaModel_ComuneId);
            }

            if (model.DipendenteRicercaModel_GestitoSportello.HasValue)
            {
                if (model.DipendenteRicercaModel_GestitoSportello.GetValueOrDefault())
                {
                    f.And(x => x.SportelloId != null);
                }
                else
                {
                    f.And(x => x.SportelloId == null);
                }
            }

            return f;
        }

        #endregion

        #region anagrafica

        [@Authorize(Roles = new Roles[] { Roles.Dipendente, Roles.Sp_Consulente, Roles.Sp_CAF, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac })]
        public async Task<ActionResult> Anagrafica(int? id = null)
        {
            try
            {
                CheckSportelloAuthorize(id);

                Expression<Func<Dipendente, bool>> _filter = x => x.DipendenteId == id;

                //utente visualizza solo le informazioni sua
                if (IsInRole(new Roles[] { Roles.Dipendente }))
                {
                    _filter = x => x.CodiceFiscale == User.Identity.Name;
                }

                var _outModel = new DipendenteViewModel();

                var _result = unitOfWork.DipendenteRepository.Get(_filter).FirstOrDefault();

                //prendere da AspNetUser dati precompilati
                var _user = await UserManager.FindByNameAsync(User.Identity.Name);

                if (_result != null)
                {
                    _outModel = Reflection.CreateModel<DipendenteViewModel>(_result);
                    _outModel.Aziende = _result.Aziende;
                    _outModel.Sportello = _result.Sportello;
                    _outModel.InformazioniPersonaliCompilati = _user.InformazioniPersonaliCompilati;
                }
                else
                {
                    if (IsInRole(new Roles[] { Roles.Dipendente }))
                    {
                        _outModel = new DipendenteViewModel
                        {
                            CodiceFiscale = _user.UserName,
                            Cognome = _user.Cognome,
                            Nome = _user.Nome,
                            Email = _user.Email,
                            InformazioniPersonaliCompilati = false
                        };
                    }
                }

                return AjaxView("Anagrafica", _outModel);
            }
            catch (Exception ex)
            {
                if (Request.IsAjaxRequest())
                {
                    throw;
                }

                return AjaxView("Error", new HandleErrorInfo(ex, "DipendenteController", "Anagrafica"));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [@Authorize(Roles = new Roles[] { Roles.Dipendente, Roles.Sp_Consulente, Roles.Sp_CAF, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac })]
        public async Task<ActionResult> Anagrafica(DipendenteViewModel model)
        {
            try
            {
                CheckSportelloAuthorize(model.DipendenteId);

                //utente aggiorna solo le informazioni sua
                if (IsInRole(new Roles[] { Roles.Dipendente }))
                {
                    model.CodiceFiscale = User.Identity.Name;

                    Expression<Func<Dipendente, bool>> _filter = x => x.CodiceFiscale == User.Identity.Name;

                    var _result = unitOfWork.DipendenteRepository.Get(_filter).FirstOrDefault();

                    if (_result != null)
                    {
                        model.DipendenteId = _result.DipendenteId;
                        model.CodiceFiscale = _result.CodiceFiscale;
                    }
                }

                if (model.DipendenteId != 0)
                {
                    ModelState.Remove("DocumentoIdentita");
                    ModelState.Remove("DelegaDipendente");
                }

                if (!IsInRole(new Roles[] { Roles.Sp_Consulente, Roles.Sp_CAF, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac }))
                {
                    ModelState.Remove("DocumentoIdentita");
                    ModelState.Remove("DelegaDipendente");
                }

                //check dipendente exists con codice fiscale
                var _DipendenteExists = unitOfWork.DipendenteRepository.Get(x => x.DipendenteId != model.DipendenteId
                && x.CodiceFiscale.ToLower() == model.CodiceFiscale.ToLower());
                if (_DipendenteExists?.Count() > 0)
                {
                    throw new Exception("Codice Fiscale già registrata");
                }

                if (IsInRole(new Roles[] { Roles.Sp_Consulente, Roles.Sp_CAF, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac }) && model.DipendenteId != 0)
                {
                    var _dipendente = unitOfWork.DipendenteRepository.Get(x => x.DipendenteId == model.DipendenteId);

                    if (_dipendente?.Count() > 0 && _dipendente.FirstOrDefault()?.SportelloId != GetSportelloId)
                    {
                        throw new Exception("Dipendente non e associata alla sua Utenza");
                    }

                    model.SportelloId = GetSportelloId;
                }

                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                //cast to Dipendente model
                var _resultModel = Reflection.CreateModel<Dipendente>(model);
                _resultModel.CodiceFiscale = _resultModel.CodiceFiscale.ToUpper();
                unitOfWork.DipendenteRepository.InsertOrUpdate(_resultModel);
                unitOfWork.Save(false);

                //inserisci DelegheSportelloDipendente
                if (model.DipendenteId == 0 && IsInRole(new Roles[] { Roles.Sp_Consulente, Roles.Sp_CAF, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac }))
                {
                    AssociaSportelloDipendente(_resultModel.DipendenteId, model.DelegaDipendente, model.DocumentoIdentita);
                }

                //update flag InformazioniPersonaliCompilati
                if (IsInRole(new Roles[] { Roles.Dipendente }))
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
                    DipendenteId = _resultModel.DipendenteId,
                    message = "Anagrafica " + (model.DipendenteId == 0 ? "inserita" : "aggiornata")
                });
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        #endregion

        #region associa dipendente a sportello

        [@Authorize(Roles = new Roles[] { Roles.Sp_Consulente, Roles.Sp_CAF, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac })]
        public ActionResult AssociaSportelloRicerca()
        {
            return AjaxView();
        }

        [HttpPost]
        [@Authorize(Roles = new Roles[] { Roles.Sp_Consulente, Roles.Sp_CAF, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac })]
        public async Task<ActionResult> AssociaSportelloRicerca(DipendenteAssociaSportelloRicercaModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return JsonResultFalse(ModelStateErrorToString(ModelState));
                }

                int? sportelloId = GetSportelloId;

                Expression<Func<Dipendente, bool>> _filter = x =>
                (sportelloId != null ? (x.SportelloId != sportelloId) : true)
                && ((x.DipendenteId == model.DipendenteAssociaRicercaModel_DipendenteId));

                var _result = await unitOfWork.DipendenteRepository.GetAsync(_filter);
                if (_result.Count() == 0)
                {
                    return JsonResultFalse("Dipendente non trovata.");
                }
                DipendenteAssociaSportelloRicercaViewModel dipAssociaRicercaViewModel = new DipendenteAssociaSportelloRicercaViewModel
                {
                    Dipendente = _result.FirstOrDefault(),
                    DipendenteId = _result.FirstOrDefault().DipendenteId,
                };
                //dipAssociaRicercaViewModel.DipendenteId= dipAssociaRicercaViewModel.Dipendente.di
                return AjaxView("AssociaSportelloRicercaList", dipAssociaRicercaViewModel);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [@Authorize(Roles = new Roles[] { Roles.Sp_Consulente, Roles.Sp_CAF, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac })]
        public ActionResult AssociaSportello(DipendenteAssociaSportelloRicercaViewModel model)
        {
            try
            {
                AssociaSportelloDipendente(model.DipendenteId, model.DelegaDipendente, model.DocumentoIdentita);
                return JsonResultTrue("Dipendente associato");
            }
            catch (Exception)
            {
                throw;
            }
        }

        void AssociaSportelloDipendente(int dipendenteId, string delegaDipendente, string documentoIdentita)
        {
            try
            {
                var _dipendente = unitOfWork.DipendenteRepository.Get(x => x.DipendenteId == dipendenteId).FirstOrDefault();

                if (_dipendente == null)
                {
                    throw new Exception("Dipendente non trovato");
                }

                var _delegaattiva = unitOfWork.DelegheSportelloDipendenteRepository.Get(xx => xx.DelegaAttiva == true && xx.DipendenteId == dipendenteId);

                if (_delegaattiva != null)
                {
                    foreach (var item in _delegaattiva)
                    {
                        item.DataDelegaDisdetta = DateTime.Now;
                        item.DelegaAttiva = false;
                        unitOfWork.DelegheSportelloDipendenteRepository.Update(item);
                    }
                }

                var _sportelloId = GetSportelloId.Value;

                DelegheSportelloDipendente _delega = new DelegheSportelloDipendente
                {
                    DelegaAttiva = true,
                    DipendenteId = dipendenteId,
                    SportelloId = _sportelloId,
                    DataInserimento = DateTime.Now,
                    DelegaDipendente = Savefile(GetUploadFolder(PathDelegheDipendente, GetSportelloId.Value), delegaDipendente),
                    DocumentoIdentita = Savefile(GetUploadFolder(PathCartaIdentita, GetSportelloId.Value), documentoIdentita),
                };
                unitOfWork.DelegheSportelloDipendenteRepository.Insert(_delega);

                //aggiorna sportelloId tabella dipendenti
                _dipendente.SportelloId = _sportelloId;

                //aggiorna richieste sportelloid
                var _pratiche = unitOfWork.PraticheRegionaliImpreseRepository.Get(x => x.DipendenteId == dipendenteId && (bool)x.TipoRichiesta.IsTipoRichiestaDipendente);

                if (_pratiche.Count() > 0)
                {
                    foreach (var item in _pratiche)
                    {
                        item.SportelloId = _sportelloId;
                        unitOfWork.PraticheRegionaliImpreseRepository.Update(item);
                    }
                }

                unitOfWork.Save(false);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [@Authorize(Roles = new Roles[] { Roles.Sp_Consulente, Roles.Sp_CAF, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac })]
        public async Task<ActionResult> DeAssociaSportelloDipendente(int dipendenteId, int deleghaId)
        {
            try
            {
                CheckSportelloAuthorize(dipendenteId);

                var _delegaattiva = unitOfWork.DelegheSportelloDipendenteRepository.
                    Get(xx => xx.DelegheSportelloDipendenteId == deleghaId && xx.DipendenteId == dipendenteId).FirstOrDefault();

                if (IsInRole(Roles.Dipendente))
                {
                    if (GetDipendenteId.Value != _delegaattiva.DipendenteId)
                    {
                        throw new Exception("Utente non abilitato");
                    }
                }

                var _dipendente = unitOfWork.DipendenteRepository.Get(xx => xx.DipendenteId == _delegaattiva.DipendenteId).FirstOrDefault();
                _dipendente.SportelloId = null;
                unitOfWork.DipendenteRepository.Update(_dipendente);

                _delegaattiva.DataDelegaDisdetta = DateTime.Now;
                _delegaattiva.DelegaAttiva = false;
                unitOfWork.DelegheSportelloDipendenteRepository.Update(_delegaattiva);

                //aggiorna richieste sportelloid
                var _pratiche = unitOfWork.PraticheRegionaliImpreseRepository.Get(x => x.DipendenteId == dipendenteId);

                if (_pratiche.Count() > 0)
                {
                    foreach (var item in _pratiche)
                    {
                        item.SportelloId = null;
                        unitOfWork.PraticheRegionaliImpreseRepository.Update(item);
                    }
                }

                await unitOfWork.SaveAsync(false);

                return JsonResultTrue("Delega cancellata");
            }
            catch (Exception)
            {
                throw;
            }
        }



        [@Authorize(Roles = new Roles[] { Roles.Sp_Consulente, Roles.Sp_CAF, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac })]
        public ActionResult UploadAllegato(int delegaId, string allegato)
        {
            var _delega = unitOfWork.DelegheSportelloDipendenteRepository.Get(x => x.DelegheSportelloDipendenteId == delegaId).FirstOrDefault();

            if (_delega == null)
            {
                throw new Exception("Delega non trovata");
            }

            CheckSportelloAuthorize(_delega.DipendenteId);

            DipendenteUploadAllegatoModel model = new DipendenteUploadAllegatoModel();
            model.TipoAllegato = allegato;
            model.DelegheSportelloDipendenteId = delegaId;
            model.DipendenteId = _delega.DipendenteId;

            return AjaxView("UploadAllegato", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [@Authorize(Roles = new Roles[] { Roles.Sp_Consulente, Roles.Sp_CAF, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac })]
        public ActionResult UploadAllegato(DipendenteUploadAllegatoModel model)
        {
            try
            {
                var _delega = unitOfWork.DelegheSportelloDipendenteRepository.Get(x => x.DelegheSportelloDipendenteId == model.DelegheSportelloDipendenteId).FirstOrDefault();

                if (_delega == null)
                {
                    throw new Exception("Delega non trovata");
                }

                CheckSportelloAuthorize(_delega.DipendenteId);

                if (IsInRole(new Roles[] { Roles.Dipendente, Roles.Sp_Consulente, Roles.Sp_CAF, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac }))
                {
                    if (_delega.SportelloId != GetSportelloId.Value)
                    {
                        throw new Exception("Richiesta non valida");
                    }
                }

                if (model.TipoAllegato == "DocumentoIdentita")
                {
                    _delega.DocumentoIdentita = Savefile(GetUploadFolder(PathCartaIdentita, _delega.SportelloId), model.Allegato);
                }

                if (model.TipoAllegato == "DelegaDipendente")
                {
                    _delega.DelegaDipendente = Savefile(GetUploadFolder(PathDelegheDipendente, _delega.SportelloId), model.Allegato);
                }

                unitOfWork.DelegheSportelloDipendenteRepository.Update(_delega);
                unitOfWork.Save(false);

                return JsonResultTrue("Allegato aggiornato");
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        public JsonResult ListaDipendenti(string phrase, int? sportelloId = null)
        {
            Expression<Func<Dipendente, bool>> _filter = x =>
            ((sportelloId != null ? x.SportelloId == sportelloId.Value : true))
            && ((phrase != null ? (x.Nome + " " + x.Cognome).Contains(phrase) : true)
            || (phrase != null ? x.CodiceFiscale.Contains(phrase) : true));

            return GetListaDipendenti(_filter);
        }

        private JsonResult GetListaDipendenti(Expression<Func<Dipendente, bool>> filter)
        {
            var _result = unitOfWork.DipendenteRepository.Get(filter);

            if (_result.Count() > 0)
            {
                return Json(_result
                       .OrderBy(p => p.Cognome == null || p.Cognome == "")
                       .ThenBy(p => p.Nome == null || p.Nome == "")
                       .ThenBy(p => p.CodiceFiscale)
                       .Select(x => new { x.DipendenteId, x.CodiceFiscale, Nominativo = x.Nome + " " + x.Cognome + " - " + x.CodiceFiscale }), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        #region associa azienda

        [HttpGet]
        [@Authorize(Roles = new Roles[] { Roles.Dipendente, Roles.Sp_Consulente, Roles.Sp_CAF, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac })]
        public ActionResult AssociaAziendaRicerca(int dipendenteId)
        {
            CheckSportelloAuthorize(dipendenteId);

            if (IsInRole(new Roles[] { Roles.Dipendente }))
            {
                dipendenteId = GetDipendenteId.Value;
            }

            DipendenteTempoPieno(dipendenteId);
            DipendenteAziendaAssociaRicercaModel model = new DipendenteAziendaAssociaRicercaModel();
            model.AziendaAssociaRicercaModel_DipendenteId = dipendenteId;

            return AjaxView("AssociaAziendaRicerca", model);

        }

        [HttpPost]
        [@Authorize(Roles = new Roles[] { Roles.Dipendente, Roles.Sp_Consulente, Roles.Sp_CAF, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac })]
        public async Task<ActionResult> AssociaAziendaRicerca(DipendenteAziendaAssociaRicercaModel model)
        {
            try
            {
                CheckSportelloAuthorize(model.AziendaAssociaRicercaModel_DipendenteId);

                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                if (IsInRole(new Roles[] { Roles.Dipendente }))
                {
                    model.AziendaAssociaRicercaModel_DipendenteId = GetDipendenteId.Value;
                }

                DipendenteTempoPieno(model.AziendaAssociaRicercaModel_DipendenteId.Value);

                //azienda da associare al dipendente
                var _aziendaDaAssociare = unitOfWork.AziendaRepository.Get(x => x.AziendaId == model.AziendaAssociaRicercaModel_AziendaId);

                if (_aziendaDaAssociare.Count() == 0)
                {
                    throw new Exception("Azienda non censita in anagrafica. Contattare gli uffici di EBLAC.");
                }

                //lista dei aziende associate al dipendente
                var _aziendeAssociate = unitOfWork.DipendenteAziendaRepository.Get(x => x.DipendenteId == (int)model.AziendaAssociaRicercaModel_DipendenteId.Value && x.DataCessazione == null);

                var _aziendeAssociateId = _aziendeAssociate?.Select(x => x.AziendaId);

                //check che azienda da associare non e già associata al dipendente
                if (_aziendeAssociateId.Any(x => x == model.AziendaAssociaRicercaModel_AziendaId))
                {
                    throw new Exception("Azienda già associata.");
                }

                DipendenteAziendaAssociaViewModel aziendaAssociaRicercaViewModel = new DipendenteAziendaAssociaViewModel
                {
                    AziendaId = _aziendaDaAssociare.FirstOrDefault().AziendaId,
                    DipendenteAziendaAssociaViewModel_DipendenteId = model.AziendaAssociaRicercaModel_DipendenteId,
                    Aziende = _aziendaDaAssociare.FirstOrDefault(),
                    TipoContratto = await unitOfWork.TipoContrattoRepository.GetAsync(),
                    //se ha aziende associate, rimuovere le voci con flag "tempo pieno" da "TempoLavoro"
                    TempoLavoro = await unitOfWork.TempoLavoroRepository.GetAsync(x => _aziendeAssociateId.Count() > 0 ? (bool)x.TempoPieno != true : true),
                    TipoImpiego = await unitOfWork.TipoImpiegoRepository.GetAsync()
                };

                return AjaxView("AssociaAziendaRicercaList", aziendaAssociaRicercaViewModel);

            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [@Authorize(Roles = new Roles[] { Roles.Dipendente, Roles.Sp_Consulente, Roles.Sp_CAF, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac })]
        [ValidateAntiForgeryToken]
        public ActionResult AssociaAzienda(DipendenteAziendaAssociaViewModel model)
        {
            try
            {
                CheckSportelloAuthorize(model.DipendenteAziendaAssociaViewModel_DipendenteId);

                int dipendenteId = model.DipendenteAziendaAssociaViewModel_DipendenteId.GetValueOrDefault();

                if (IsInRole(new Roles[] { Roles.Dipendente }))
                {
                    ModelState.Remove("DipendenteAziendaAssociaViewModel_DipendenteId");
                    dipendenteId = GetDipendenteId.Value;
                }

                if (!ModelState.IsValid)
                {
                    return JsonResultFalse(ModelStateErrorToString(ModelState));
                }

                DipendenteTempoPieno(dipendenteId);

                DipendenteAzienda dipendenteAzienda = new DipendenteAzienda
                {
                    AziendaId = model.AziendaId,
                    CCNLCNEL = model.CCNLCNEL,
                    DataAssunzione = model.DataAssunzione.GetValueOrDefault(),
                    DipendenteId = dipendenteId,
                    TempoLavoroId = model.TempoLavoroId.GetValueOrDefault(),
                    TipoContrattoId = model.TipoContrattoId.GetValueOrDefault(),
                    TipoImpiegoId = model.TipoImpiegoId.GetValueOrDefault(),
                    DocumentoIdentita = Savefile(GetUploadFolder(PathCartaIdentitaDipendente, dipendenteId), model.DocumentoIdentita),
                    //DocumentoAltro = Savefile(GetUploadFolder(PathAltroDipendente, dipendenteId), model.AltroDocumento),
                };

                unitOfWork.DipendenteAziendaRepository.Insert(dipendenteAzienda);
                unitOfWork.Save(false);

                return JsonResultTrue("Azienda associata");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        [HttpPost]
        [@Authorize(Roles = new Roles[] { Roles.Dipendente, Roles.Sp_Consulente, Roles.Sp_CAF, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac })]
        [ValidateAntiForgeryToken]
        public ActionResult CessazioneContratto(DipendenteAziendaCessazioneContrattoModel model)
        {
#pragma warning disable CS0168 // La variabile è dichiarata, ma non viene mai usata
            try
            {
                CheckSportelloAuthorize(model.DipendenteId);

                if (!Request.IsAjaxRequest())
                {
                    return AjaxView("AziendeAssociateRicerca");
                }

                if (!ModelState.IsValid)
                {
                    return JsonResultFalse(ModelStateErrorToString(ModelState));
                }

                if (IsInRole(new Roles[] { Roles.Dipendente }))
                {
                    model.DipendenteId = GetDipendenteId.Value;
                }

                var _issportello = IsInRole(new Roles[] { Roles.Sp_Consulente, Roles.Sp_CAF, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac });
                var _isdipendente = IsInRole(new Roles[] { Roles.Dipendente });

                var _dipaz = unitOfWork.DipendenteAziendaRepository.Get(x =>
                (x.DipendenteAziendaId == model.DipendenteAziendaId)
                && (_issportello ? x.Dipendente.SportelloId == (int)GetSportelloId.Value : true)
                && (_isdipendente ? x.DipendenteId == (int)GetDipendenteId.Value : true)).FirstOrDefault();

                if (_dipaz == null)
                {
                    throw new Exception("Richiesta non valida");
                }

                _dipaz.DataAssunzione = model.DataAssunzione;
                _dipaz.DataCessazione = model.DataCessione;
                unitOfWork.DipendenteAziendaRepository.Update(_dipaz);
                unitOfWork.Save(false);

                return JsonResultTrue("Dati aggiornati");
            }
            catch (Exception e)
            {
                throw;
            }
#pragma warning restore CS0168 // La variabile è dichiarata, ma non viene mai usata
        }

        [@Authorize(Roles = new Roles[] { Roles.Dipendente, Roles.Sp_Consulente, Roles.Sp_CAF, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac })]
        public ActionResult CessazioneContratto(int dipendenteAziendaId)
        {
#pragma warning disable CS0168 // La variabile è dichiarata, ma non viene mai usata
            try
            {
                if (!Request.IsAjaxRequest())
                {
                    return AjaxView("AziendeAssociateRicerca");
                }

                var _issportello = IsInRole(new Roles[] { Roles.Sp_Consulente, Roles.Sp_CAF, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac });
                var _isdipendente = IsInRole(new Roles[] { Roles.Dipendente });

                var _dipaz = unitOfWork.DipendenteAziendaRepository.Get(x =>
                x.DipendenteAziendaId == dipendenteAziendaId
                && (_issportello ? x.Dipendente.SportelloId == (int)GetSportelloId.Value : true)
                && (_isdipendente ? x.DipendenteId == (int)GetDipendenteId.Value : true)).FirstOrDefault();

                if (_dipaz == null)
                {
                    throw new Exception("Richiesta non valida");
                }

                if (_dipaz.DataCessazione.HasValue)
                {
                    throw new Exception("Richiesta non valida");
                }

                DipendenteAziendaCessazioneContrattoModel model = new DipendenteAziendaCessazioneContrattoModel();
                model.DipendenteAziendaId = _dipaz.DipendenteAziendaId;
                model.DataAssunzione = _dipaz.DataAssunzione;
                model.DipendenteId = _dipaz.DipendenteId;

                return AjaxView("CessazioneContratto", model);
            }
            catch (Exception e)
            {
                throw;
            }
#pragma warning restore CS0168 // La variabile è dichiarata, ma non viene mai usata
        }

        /// <summary>
        /// Dipendente assunto a "Tempo Pieno"
        /// </summary>
        /// <exception cref="Exception"></exception>
        void DipendenteTempoPieno(int dipendenteId)
        {
            var _aziendeAssociate = unitOfWork.DipendenteAziendaRepository.Get(x => x.DipendenteId == dipendenteId && x.DataCessazione == null);

            //CONTROLLI:
            //Se ha inserito la tipologia “Tempo Pieno” non può associare più di un'azienda.
            //Se prova ad associare più di un'azienda e ha dichiarato di essere Tempo Pieno
            //possiamo far tornare il messaggio: “Impossibile associare più di un'azienda in
            //caso di assunzione a Tempo Pieno”
            if (_aziendeAssociate?.Where(d => d.DataCessazione == null && d.TempoLavoro.TempoPieno == true).Count() > 0)
            {
                throw new Exception("Impossibile associare più di un'azienda in caso di assunzione a Tempo Pieno");
            }
        }
        #endregion

        public ActionResult DownloadAllegato(int? delegaId, string allegato)
        {
            try
            {
                var _allegato = unitOfWork.DelegheSportelloDipendenteRepository.Get(x => x.DelegheSportelloDipendenteId == delegaId).FirstOrDefault();

                var _uploadFolder = "";

                var _file = allegato == "DelegaDipendente" ? _allegato?.DelegaDipendente : _allegato?.DocumentoIdentita;

                if (allegato == "DelegaDipendente")
                {
                    _uploadFolder = GetUploadFolder(PathDelegheDipendente, _allegato.SportelloId);
                }
                else
                {
                    _uploadFolder = GetUploadFolder(PathCartaIdentita, _allegato.SportelloId);
                }

                CheckSportelloAuthorize(_allegato.DipendenteId);

                if (_allegato == null || _file == null || !System.IO.File.Exists(Path.Combine(_uploadFolder, _file)))
                {
                    throw new Exception("Allegato non trovato");
                }

                var mimeType = System.Web.MimeMapping.GetMimeMapping(_file);
                return File(Path.Combine(_uploadFolder, _file), mimeType, $"{allegato}{Path.GetExtension(_file)}");
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        private void CheckSportelloAuthorize(int? id)
        {
            if (id.GetValueOrDefault() != 0)
            {
                if (IsInRole(new Roles[] { Roles.Sp_Consulente, Roles.Sp_CAF, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac }))
                {
                    if (GetSportelloId.Value != unitOfWork.DipendenteRepository.Get(x => x.DipendenteId == id).FirstOrDefault().SportelloId)
                    {
                        throw new Exception("Utente non autorizzato");
                    }
                }
            }
        }
    }
}