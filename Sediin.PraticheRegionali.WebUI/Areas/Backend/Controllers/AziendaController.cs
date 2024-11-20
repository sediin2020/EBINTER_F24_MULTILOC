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
    public class AziendaController : BaseController
    {
        public string PathCartaIdentita { get => "Documenti\\Sportello\\{0}\\CartaIdentita"; private set { } }
        public string PathDelegheAzienda { get => "Documenti\\Sportello\\{0}\\Delega\\Azienda"; private set { } }

        #region ricerca

        [@Authorize(Roles = new Roles[] { Roles.Sp_Consulente, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac })]
        public async Task<ActionResult> Ricerca()
        {
            AziendaRicercaModel model = new AziendaRicercaModel
            {
                Tipologie = await unitOfWork.TipologiaRepository.GetAsync(),
            };

            return AjaxView("Ricerca", model);
        }

        [HttpPost]
        [@Authorize(Roles = new Roles[] { Roles.Sp_Consulente, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac })]
        public ActionResult Ricerca(AziendaRicercaModel model, int? page)
        {
            //TODO
            //int totalRows = 0;

            //var memoryCache = MemoryCache.Default;

            //IEnumerable<Azienda> _query = null;

            //if (page == null || memoryCache["RicercaAzienda"] == null)
            //{
            //    memoryCache.Remove("RicercaAzienda");

            //    _query = unitOfWork.AziendaRepository.Get(ref totalRows, RicercaFilter2(model), model.Ordine, page, model.PageSize);

            //    memoryCache.Add("RicercaAzienda", _query, DateTimeOffset.UtcNow.AddMinutes(30));
            //}
            //else
            //{
            //    _query = (IQueryable<Azienda>)memoryCache["RicercaAzienda"];
            //}

            int totalRows = 0;

            var _query = unitOfWork.AziendaRepository.Get(ref totalRows, RicercaFilter2(model), model.Ordine, page, model.PageSize);

            var _result = GeModelWithPaging<AziendaRicercaViewModel, Azienda>(page, _query, model, totalRows, model.PageSize);

            return AjaxView("RicercaList", _result);
        }

        [@Authorize(Roles = new Roles[] { Roles.Sp_Consulente, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac })]
        public ActionResult RicercaExcel(AziendaRicercaModel model)
        {
            var _query = from a in unitOfWork.AziendaRepository.Get(RicercaFilter(model)).OrderBy(r => r.RagioneSociale)
                         select new
                         {
                             a.RagioneSociale,
                             a.CognomeTitolare,
                             a.NomeTitolare,
                             a.CodiceFiscale,
                             a.PartitaIva,
                             a.MatricolaInps,
                             a.Tipologia?.Descrizione,
                             a.CSC,
                             a.Classificazione,
                             a.AttivitaEconomica,
                             a.CodiceIstat,
                             a.Comune?.DENCOM,
                             a.Localita?.CAP,
                             a.Localita?.DENLOC,
                         };

            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(_query, "Aziende");
        }

        private SqlLam<Azienda> RicercaFilter2(AziendaRicercaModel model)
        {
            int? _sportelloId = null;

            if (IsInRole(new Roles[] { Roles.Sp_Consulente, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac }))
            {
                _sportelloId = GetSportelloId.Value;
            }

            TrimAll(model);

            var f = new SqlLam<Azienda>();

            if (_sportelloId.HasValue)
            {
                f.And(x => x.SportelloId == _sportelloId);
            }

            if (!string.IsNullOrWhiteSpace(model.AziendaRicercaModel_RagioneSociale))
            {
                f.And(x => x.RagioneSociale.Contains(model.AziendaRicercaModel_RagioneSociale));
            }

            if (!string.IsNullOrWhiteSpace(model.AziendaRicercaModel_MatricolaInps))
            {
                f.And(x => x.MatricolaInps == model.AziendaRicercaModel_MatricolaInps);
            }

            if (!string.IsNullOrWhiteSpace(model.AziendaRicercaModel_CodiceFiscale))
            {
                f.And(x => x.CodiceFiscale == model.AziendaRicercaModel_CodiceFiscale);
            }

            if (!string.IsNullOrWhiteSpace(model.AziendaRicercaModel_PartitaIva))
            {
                f.And(x => x.PartitaIva == model.AziendaRicercaModel_PartitaIva);
            }

            if (!string.IsNullOrWhiteSpace(model.AziendaRicercaModel_CSC))
            {
                f.And(x => x.CSC == model.AziendaRicercaModel_CSC);
            }

            if (model.AziendaRicercaModel_ComuneId.HasValue)
            {
                f.And(x => x.ComuneId == model.AziendaRicercaModel_ComuneId);
            }

            if (model.AziendaRicercaModel_TipologiaId.HasValue)
            {
                f.And(x => x.TipologiaId == model.AziendaRicercaModel_TipologiaId);
            }

            if (model.AziendaRicercaModel_ConsulenteCS == "1")
            {
                f.And(x => x.SportelloId != null);
            }

            //if (model.AziendaRicercaModel_Coperta != null)
            //{


            //    f.And(x => x.Copertura != null);
            //}

            return f;
        }

        private Expression<Func<Azienda, bool>> RicercaFilter(AziendaRicercaModel model)
        {
            int? _sportelloId = null;

            if (IsInRole(new Roles[] { Roles.Sp_Consulente, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac }))
            {
                _sportelloId = GetSportelloId.Value;
            }

            TrimAll(model);

            return x =>
            (_sportelloId.HasValue ? (x.SportelloId == _sportelloId) : true)
            && (model.AziendaRicercaModel_RagioneSociale != null ? x.RagioneSociale.Contains(model.AziendaRicercaModel_RagioneSociale) : true
            && model.AziendaRicercaModel_MatricolaInps != null ? x.MatricolaInps == model.AziendaRicercaModel_MatricolaInps : true
            && model.AziendaRicercaModel_CodiceFiscale != null ? x.CodiceFiscale == model.AziendaRicercaModel_CodiceFiscale : true
            && model.AziendaRicercaModel_PartitaIva != null ? x.PartitaIva == model.AziendaRicercaModel_PartitaIva : true
            && model.AziendaRicercaModel_ComuneId != null ? x.ComuneId == model.AziendaRicercaModel_ComuneId : true
            && model.AziendaRicercaModel_TipologiaId != null ? x.TipologiaId == model.AziendaRicercaModel_TipologiaId : true
            && (model.AziendaRicercaModel_ConsulenteCS != null ? model.AziendaRicercaModel_ConsulenteCS == "1"
            ? x.SportelloId != null : x.SportelloId == null : true)
            && model.AziendaRicercaModel_CSC != null ? x.CSC.Contains(model.AziendaRicercaModel_CSC) : true);
        }

        #endregion

        [@Authorize(Roles = new Roles[] { Roles.Azienda, Roles.Sp_Consulente, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac })]
        public async Task<ActionResult> Anagrafica(int? id = null)
        {
            try
            {
                CheckSportelloAuthorize(id);

                var model = new AziendaViewModel();

                Azienda _azienda = null;

                ApplicationUser _userSportello = null;

                ApplicationUser _userAzienda = null;

                //se utente che vede dettaglio e una azienda, check che aggiorna solo le informazioni sua, imposta aziendaId e matricola
                if (IsInRole(Roles.Azienda))
                {
                    _azienda = unitOfWork.AziendaRepository.Get(x => x.MatricolaInps == User.Identity.Name).FirstOrDefault();

                    model = Reflection.CreateModel<AziendaViewModel>(_azienda);
                    model.MatricolaInps = User.Identity.Name;


                    if (_azienda != null)
                    {
                        model.AziendaId = _azienda.AziendaId;
                        if (_azienda.Copertura?.Count > 0)
                        {
                            model.Coperto = _azienda.Copertura?.FirstOrDefault()?.Coperto;
                        }
                    }
                    else
                    {
                        //nuovo azienda, prendere da AspNetUser dati precompilati
                        _userAzienda = await UserManager.FindByNameAsync(User.Identity.Name);

                        model.NomeTitolare = _userAzienda.Nome;
                        model.CognomeTitolare = _userAzienda.Cognome;
                        model.Email = _userAzienda.Email;
                        model.InformazioniPersonaliCompilati = false;
                    }
                }
                else
                {
                    _azienda = unitOfWork.AziendaRepository.Get(x => x.AziendaId == id).FirstOrDefault();
                    model = Reflection.CreateModel<AziendaViewModel>(_azienda);

                    if (_azienda != null)
                    {
                        if (_azienda.Copertura.Count > 0)
                        {
                            model.Coperto = _azienda.Copertura.FirstOrDefault().Coperto;
                        }
                    }

                    //sportello po modificare solo azienda associata a se
                    if (IsInRole(new Roles[] { Roles.Sp_Consulente, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac }))
                    {
                        _userSportello = await UserManager.FindByNameAsync(User.Identity.Name);
                        model.ProvinciaIdFilter = _userSportello?.ProvinciaId;
                        model.SportelloId = GetSportelloId.Value;

                        if (_azienda != null && _azienda?.SportelloId != model.SportelloId)
                        {
                            throw new Exception("Azienda non e associata alla sua Utenza");
                        }
                    }
                }

                //se azienda e già inserita
                if (_azienda != null)
                {
                    _userAzienda = await UserManager.FindByNameAsync(_azienda.MatricolaInps);

                    if (_azienda.Sportello != null)
                    {
                        _userSportello = await UserManager.FindByNameAsync(_azienda.Sportello?.CodiceFiscalePIva);
                    }

                    model.ProvinciaIdFilter = _userSportello?.ProvinciaId;
                    model.DelegheSportelloAzienda = _azienda.DelegheSportelloAzienda;
                    model.Sportello = _azienda.Sportello;
                    model.Copertura = _azienda.Copertura;
                    model.Regione = _azienda.Regione;
                    model.Provincia = _azienda.Provincia;
                    model.Comune = _azienda.Comune;
                    model.Localita = _azienda.Localita;
                    model.InformazioniPersonaliCompilati = _userAzienda?.InformazioniPersonaliCompilati;
                }

                model.Tipologie = unitOfWork.TipologiaRepository.Get();

                return AjaxView("Anagrafica", model);
            }
            catch (Exception ex)
            {
                return AjaxView("Error", new HandleErrorInfo(ex, "AziendaController", "Anagrafica"));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [@Authorize(Roles = new Roles[] { Roles.Azienda, Roles.Sp_Consulente, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac })]
        public async Task<ActionResult> Anagrafica(AziendaViewModel model)
        {
            try
            {
                CheckSportelloAuthorize(model.AziendaId);

                if (IsInRole(new Roles[] { Roles.Admin, Roles.Azienda }) || model.AziendaId != 0)
                {
                    ModelState.Remove("DocumentoIdentita");
                    ModelState.Remove("DelegaAzienda");
                }

                Azienda _azienda = null;

                ApplicationUser _userSportello = null;

                model.DataIscrizione = DateTime.Now;

                //se utente che aggiorna e una azienda, check che aggiorna solo le informazioni sua, imposta aziendaId e matricola
                if (IsInRole(Roles.Azienda))
                {
                    model.MatricolaInps = User.Identity.Name;

                    _azienda = unitOfWork.AziendaRepository.Get(x => x.MatricolaInps == model.MatricolaInps).FirstOrDefault();

                    if (_azienda != null)
                    {
                        model.AziendaId = _azienda.AziendaId;

                        //se la azienda e gestita dal sportello, e ha la provincia associata, non e possibile modificare la provincia
                        if (_azienda.Sportello != null)
                        {
                            _userSportello = await UserManager.FindByNameAsync(_azienda.Sportello.CodiceFiscalePIva);

                            model.ProvinciaId = _userSportello?.ProvinciaId ?? _azienda.ProvinciaId;
                            model.RegioneId = _azienda.RegioneId;
                            model.LocalitaId = _azienda.LocalitaId;
                            model.ComuneId = _azienda.ComuneId;
                        }
                    }
                }
                else
                {
                    _azienda = unitOfWork.AziendaRepository.Get(x => x.AziendaId == model.AziendaId).FirstOrDefault();

                    //sportello po modificare solo azienda associata a se
                    if (IsInRole(new Roles[] { Roles.Sp_Consulente, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac }))
                    {
                        _userSportello = await UserManager.FindByNameAsync(User.Identity.Name);
                        model.ProvinciaId = _userSportello?.ProvinciaId ?? model.ProvinciaId;
                        model.SportelloId = GetSportelloId.Value;

                        if (_azienda != null && _azienda?.SportelloId != model.SportelloId)
                        {
                            throw new Exception("Azienda non e associata alla sua Utenza");
                        }
                    }

                }

                //prendo azienda inserita
                _azienda = unitOfWork.AziendaRepository.Get(x => x.AziendaId == model.AziendaId).FirstOrDefault();

                //se la azienda esiste
                if (_azienda != null)
                {
                    //setta valori non modificabili
                    model.SportelloId = _azienda.SportelloId;
                    model.MatricolaInps = _azienda.MatricolaInps;
                    model.DataIscrizione = _azienda.DataIscrizione;
                }

                //check che non ce una azienda inserita con la stessa matricola
                _azienda = unitOfWork.AziendaRepository.Get(x => x.MatricolaInps == model.MatricolaInps && x.AziendaId != model.AziendaId).FirstOrDefault();
                if (_azienda != null)
                {
                    throw new Exception("Matricola Inps già registrata");
                }

                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                //cast to azienda model
                var _resultModel = Reflection.CreateModel<Azienda>(model);
                _resultModel.CodiceFiscale = _resultModel.CodiceFiscale.ToUpper();

                unitOfWork.AziendaRepository.InsertOrUpdate(_resultModel);
                unitOfWork.Save(false);

                //aggiorna tb DelegheConsulenteCSAzienda
                if (model.AziendaId == 0 && IsInRole(new Roles[] { Roles.Sp_Consulente, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac }))
                {
                    AssociaSportelloAzienda(_resultModel.AziendaId, model.DelegaAzienda, model.DocumentoIdentita);
                }

                //alla fine, aggiorna flag InformazioniPersonaliCompilati
                if (IsInRole(Roles.Azienda))
                {
                    //update flag InformazioniPersonaliCompilati
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
                    aziendaId = _resultModel.AziendaId,
                    message = "Anagrafica " + (model.AziendaId == 0 ? "inserita" : "aggiornata")
                });
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }


        [HttpPost]
        [@Authorize(Roles = new Roles[] { Roles.Azienda })]
        public ActionResult AggiornaCopertura(int aziendaId, bool coperta)
        {
            try
            {
                //check se Copertura esiste
                var _Copertura = unitOfWork.CoperturaRepository.Get(m => m.AziendaId == aziendaId).FirstOrDefault();

                if (_Copertura != null)
                {
                    // Aggiorna
                    _Copertura.AziendaId = aziendaId;
                    _Copertura.Coperto = coperta;

                    unitOfWork.CoperturaRepository.Update(_Copertura);
                }
                else
                {
                    // Inserisci
                    _Copertura = new Copertura();

                    _Copertura.AziendaId = aziendaId;
                    _Copertura.Coperto = coperta;

                    unitOfWork.CoperturaRepository.Insert(_Copertura);
                }

                unitOfWork.Save();

                //manda mail a utenti che hanno una richiesta prestazioni
                if (coperta)
                {
                    var _ragionesociale = Sediin.PraticheRegionali.Utils.ConfigurationProvider.Instance.GetConfiguration().RagioneSociale.Nome;

                    var _body = RenderTemplate("Azienda/ContributoMail", new AziendaContributi_Mail
                    {
                        Nominativo = "@Model.Nominativo",
                        Ragionesociale = "@Model.Ragionesociale",
                        Matricola = "@Model.Matricola"
                    });

                    Task.Run(() =>
                    {

                        var _pra = unitOfWork.PraticheRegionaliImpreseRepository.Get(x => x.AziendaId == aziendaId);

                        if (_pra?.Count() > 0)
                        {

                            List<string> _emailList = new List<string>();

                            PraticheController praticheController = new PraticheController();

                            foreach (var item in _pra)
                            {
                                try
                                {


                                    var _email = praticheController.GetEmailAddressFromRichiesta(item);

                                    if (!_emailList.Any(x => x == _email))
                                    {
                                        var _nome = praticheController.GetNominativoFromRichiesta(item);
                                       
                                        _body = _body.Replace("@Model.Nominativo", _nome);
                                        _body = _body.Replace("@Model.Ragionesociale", item.Azienda.RagioneSociale);
                                        _body = _body.Replace("@Model.Matricola", item.Azienda.MatricolaInps);

                                        _emailList.Add(_email);

                                        SendMailAsync(new WebUI.Models.SimpleMailMessage
                                        {
                                            ToEmail = _email,
                                            ToName = _nome,
                                            Subject = _ragionesociale + " - Avviso Regolarità contrubutiva Azienda",
                                            Body = _body
                                        });

                                    }

                                }
                                catch
                                {

                                }
                            }
                        }
                    });
                }

                return JsonResultTrue("Copertura aggiornata");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        #region associa azienda a sportello

        [@Authorize(Roles = new Roles[] { Roles.Sp_Consulente, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac })]
        public ActionResult AssociaAziendaRicerca()
        {
            if (IsInRole(Roles.Admin))
            {
                throw new Exception("Funzione non abilitata al amministratore");
            }

            var _user = UserManager.FindByName(User.Identity.Name);
            AziendaAssociaRicercaModel model = new AziendaAssociaRicercaModel();
            model.ProvinciaIdFilter = _user.ProvinciaId;

            return AjaxView("AssociaAziendaRicerca", model);
        }

        [HttpPost]
        [@Authorize(Roles = new Roles[] { Roles.Sp_Consulente, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac })]
        public async Task<ActionResult> AssociaAziendaRicerca(AziendaAssociaRicercaModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return JsonResultFalse(ModelStateErrorToString(ModelState));
                }

                int? _sportelloId = GetSportelloId;

                Expression<Func<Azienda, bool>> _filter = x =>
                 // (_sportelloId != null ? (x.SportelloId != _sportelloId) : true)&&
                 ((x.AziendaId == model.AziendaAssociaRicercaModel_AziendaId));

                var _result = await unitOfWork.AziendaRepository.GetAsync(_filter);
                if (_result.Count() == 0)
                {
                    return JsonResultFalse("Nessuna Azienda trovata.");
                }

                if (_sportelloId != null && _result.FirstOrDefault().SportelloId == _sportelloId)
                {
                    return JsonResultFalse("Azienda già associata alla sua utenza.");
                }

                AziendaAssociaRicercaViewModel aziendaAssociaRicercaViewModel = new AziendaAssociaRicercaViewModel();
                aziendaAssociaRicercaViewModel.Aziende = _result.FirstOrDefault();
                aziendaAssociaRicercaViewModel.AziendaId = _result.FirstOrDefault().AziendaId;

                return AjaxView("AssociaAziendaRicercaList", aziendaAssociaRicercaViewModel);

            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [@Authorize(Roles = new Roles[] { Roles.Sp_Consulente, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac })]
        public ActionResult AssociaAzienda(AziendaAssociaRicercaViewModel model)
        {
            try
            {
                AssociaSportelloAzienda(model.AziendaId, model.DelegaAzienda, model.DocumentoIdentita);
                return JsonResultTrue("Azienda associata");
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        void AssociaSportelloAzienda(int aziendaId, string delegaAzienda, string documentoIdentita)
        {
            try
            {

                var _azienda = unitOfWork.AziendaRepository.Get(x => x.AziendaId == aziendaId).FirstOrDefault();

                if (_azienda == null)
                {
                    throw new Exception("Azienda non trovata");
                }

                //aggiorna tb DelegheConsulenteCSAzienda
                var _delegaattiva = unitOfWork.DelegheSportelloAziendaRepository.Get(xx => xx.DelegaAttiva == true
                && xx.AziendaId == aziendaId);

                if (_delegaattiva != null)
                {
                    foreach (var item in _delegaattiva)
                    {
                        item.DataDelegaDisdetta = DateTime.Now;
                        item.DelegaAttiva = false;
                        unitOfWork.DelegheSportelloAziendaRepository.Update(item);
                    }
                }

                //inserisci 
                var _sportelloId = GetSportelloId.GetValueOrDefault();

                DelegheSportelloAzienda _delega = new DelegheSportelloAzienda
                {
                    DelegaAttiva = true,
                    AziendaId = aziendaId,
                    SportelloId = GetSportelloId.GetValueOrDefault(),
                    DataInserimento = DateTime.Now,
                    DelegaAzienda = Savefile(GetUploadFolder(PathDelegheAzienda, _sportelloId), delegaAzienda),
                    DocumentoIdentita = Savefile(GetUploadFolder(PathCartaIdentita, _sportelloId), documentoIdentita),
                };

                unitOfWork.DelegheSportelloAziendaRepository.Insert(_delega);

                //aggiorna SportelloId tabella azienda
                _azienda.SportelloId = _sportelloId;

                //aggiorna richieste sportelloid
                var _pratiche = unitOfWork.PraticheRegionaliImpreseRepository.Get(x => x.AziendaId == aziendaId && (bool)!x.TipoRichiesta.IsTipoRichiestaDipendente);

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
        [@Authorize(Roles = new Roles[] { Roles.Azienda, Roles.Sp_Consulente, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac })]
        public async Task<ActionResult> DeAssociaSportelloAzienda(int aziendaId, int deleghaId)
        {
            try
            {
                CheckSportelloAuthorize(aziendaId);

                var _delegaattiva = unitOfWork.DelegheSportelloAziendaRepository.
                    Get(xx => xx.DelegheSportelloAziendaId == deleghaId && xx.AziendaId == aziendaId).FirstOrDefault();

                if (IsInRole(Roles.Azienda))
                {
                    if (GetAziendaId.Value != _delegaattiva.AziendaId)
                    {
                        throw new Exception("Utente non abilitato");
                    }
                }

                var _azienda = unitOfWork.AziendaRepository.Get(xx => xx.AziendaId == _delegaattiva.AziendaId).FirstOrDefault();
                _azienda.SportelloId = null;
                unitOfWork.AziendaRepository.Update(_azienda);

                _delegaattiva.DataDelegaDisdetta = DateTime.Now;
                _delegaattiva.DelegaAttiva = false;
                unitOfWork.DelegheSportelloAziendaRepository.Update(_delegaattiva);

                //aggiorna richieste sportelloid
                var _pratiche = unitOfWork.PraticheRegionaliImpreseRepository.Get(x => x.AziendaId == aziendaId);

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

        [@Authorize(Roles = new Roles[] { Roles.Azienda, Roles.Sp_Consulente, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac })]
        public ActionResult UploadAllegato(int delegaId, string allegato)
        {
            var _delega = unitOfWork.DelegheSportelloAziendaRepository.Get(x => x.DelegheSportelloAziendaId == delegaId).FirstOrDefault();

            if (_delega == null)
            {
                throw new Exception("Delega non trovata");
            }

            CheckSportelloAuthorize(_delega.AziendaId);

            AziendaUploadAllegatoModel model = new AziendaUploadAllegatoModel();
            model.TipoAllegato = allegato;
            model.DelegheConsulenteCSAziendaId = delegaId;
            model.AziendaId = _delega.AziendaId;

            return AjaxView("UploadAllegato", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [@Authorize(Roles = new Roles[] { Roles.Azienda, Roles.Sp_Consulente, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac })]
        public ActionResult UploadAllegato(AziendaUploadAllegatoModel model)
        {
            try
            {
                var _delega = unitOfWork.DelegheSportelloAziendaRepository.
                    Get(x => x.DelegheSportelloAziendaId == model.DelegheConsulenteCSAziendaId && x.DelegaAttiva == true).FirstOrDefault();

                if (_delega == null)
                {
                    throw new Exception("Delega non trovata");
                }

                CheckSportelloAuthorize(_delega.AziendaId);

                if (IsInRole(new Roles[] { Roles.Sp_Consulente, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac }))
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

                if (model.TipoAllegato == "DelegaAzienda")
                {
                    _delega.DelegaAzienda = Savefile(GetUploadFolder(PathDelegheAzienda, _delega.SportelloId), model.Allegato);
                }

                unitOfWork.DelegheSportelloAziendaRepository.Update(_delega);
                unitOfWork.Save(false);

                return JsonResultTrue("Allegato aggiornato");
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion

        public JsonResult ListaAziende(string phrase, int? provinciaId = null, int? sportelloId = null, int? dipendenteId = null)
        {
            var _searchDip = false;
            var aziendeIds = new List<int>();
            if (dipendenteId.GetValueOrDefault() != 0)
            {
                _searchDip = true;
                aziendeIds = unitOfWork.DipendenteAziendaRepository.Get(x => x.DipendenteId == dipendenteId && x.DataCessazione == null)?.Select(x => x.AziendaId)?.ToList();
            }

            Expression<Func<Azienda, bool>> _filter = x =>
            ((provinciaId != null ? x.ProvinciaId == provinciaId.Value : true)
            && (sportelloId != null ? x.SportelloId == sportelloId.Value : true)
            && (!_searchDip ? true : (aziendeIds.Count() > 0 ? (aziendeIds.Contains(x.AziendaId)) : false)))
            && ((phrase != null ? x.RagioneSociale.Contains(phrase) : true)
            || (phrase != null ? x.MatricolaInps.Contains(phrase) : true));

            return GetListaAziende(_filter);
        }

        private JsonResult GetListaAziende(Expression<Func<Azienda, bool>> filter)
        {
            var _result = unitOfWork.AziendaRepository.Get(filter);

            if (_result.Count() > 0)
            {
                return Json(_result
                       .OrderBy(p => p.RagioneSociale == null || p.RagioneSociale == "")
                       .ThenBy(p => p.RagioneSociale)
                       .Select(x => new { x.AziendaId, x.MatricolaInps, RagioneSociale = x.RagioneSociale + " - " + x.MatricolaInps }).Distinct(), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult DownloadAllegato(int? delegaId, string allegato)
        {
            try
            {
                var _allegato = unitOfWork.DelegheSportelloAziendaRepository.Get(x => x.DelegheSportelloAziendaId == delegaId).FirstOrDefault();

                var _uploadFolder = "";

                var _file = allegato == "DelegaAzienda" ? _allegato?.DelegaAzienda : _allegato?.DocumentoIdentita;

                if (allegato == "DelegaAzienda")
                {
                    _uploadFolder = GetUploadFolder(PathDelegheAzienda, _allegato.SportelloId);
                }
                else
                {
                    _uploadFolder = GetUploadFolder(PathCartaIdentita, _allegato.SportelloId);
                }

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
                if (IsInRole(new Roles[] { Roles.Sp_CAF, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebac }))
                {
                    if (GetSportelloId.Value != unitOfWork.AziendaRepository.Get(x => x.AziendaId == id).FirstOrDefault().SportelloId)
                    {
                        throw new Exception("Utente non authorizzato");
                    }
                }
            }
        }

    }
}