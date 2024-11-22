using Microsoft.AspNet.SignalR;
using Sediin.MVC.HtmlHelpers;
using Sediin.PraticheRegionali.DOM;
using Sediin.PraticheRegionali.DOM.DAL;
using Sediin.PraticheRegionali.DOM.Entitys;
using Sediin.PraticheRegionali.WebUI.Areas.Backend.Models;
using Sediin.PraticheRegionali.WebUI.Controllers;
using Sediin.PraticheRegionali.WebUI.Filters;
using Sediin.PraticheRegionali.WebUI.Hubs;
using Sediin.PraticheRegionali.WebUI.ValidationAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Runtime.Caching;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static Sediin.PraticheRegionali.DOM.DAL.PraticheAziendaUtility;
using static Sediin.PraticheRegionali.WebUI.IdentityHelper;
using Reflection = Sediin.MVC.HtmlHelpers.Reflection;

namespace Sediin.PraticheRegionali.WebUI.Areas.Backend.Controllers
{
    [Authorize]
    public class PraticheController : BaseController
    {
        public string PathPraticheAzienda { get => "Pratiche\\Richiesta\\{0}"; private set { } }

        #region ricerca

        public ActionResult Ricerca()
        {
            PraticheAziendaRicercaModel model = new PraticheAziendaRicercaModel
            {
                TipoRichiesta = GetTipoRichieste(),
                StatoPratica = GetStatoPratica(),
                StatoLiquidazione = GetStatoLiquidazione(),
            };

            if (IsInRole(new Roles[] { Roles.Sp_CAF, Roles.Sp_Consulente, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebinter }))
            {
                model.PraticheAziendaRicercaModel_SportelloId = GetSportelloId.Value;
            }

            if (IsInRole(Roles.Azienda))
            {
                model.PraticheAziendaRicercaModel_AziendaId = GetAziendaId.Value;

            }

            if (IsInRole(Roles.Dipendente))
            {
                model.PraticheAziendaRicercaModel_DipendenteId = GetDipendenteId.Value;

            }
            return AjaxView("Ricerca", model);
        }

        [HttpPost]
        //[OutputCache(VaryByParam = "page")]
        public ActionResult Ricerca(PraticheAziendaRicercaModel model, int? page)
        {
            //var memoryCache = MemoryCache.Default;

            IQueryable<PraticheRegionaliImprese> _query = null;

            _query = unitOfWork.PraticheRegionaliImpreseRepository.Get(RicercaFilter(model)).AsQueryable()
                .OrderBy(HttpUtility.UrlDecode(model.PraticheAziendaRicercaModel_OrderBy));

            //if (page == null || memoryCache["RicercaPraticheAzienda"] == null)
            //{
            //    memoryCache.Remove("RicercaPraticheAzienda");

            //    _query = unitOfWork.PraticheRegionaliImpreseRepository.Get(RicercaFilter(model)).AsQueryable()
            //   .OrderBy(HttpUtility.UrlDecode(model.PraticheAziendaRicercaModel_OrderBy));

            //    memoryCache.Add("RicercaPraticheAzienda", _query, DateTimeOffset.UtcNow.AddMinutes(30));
            //}
            //else
            //{
            //    _query = (IQueryable<PraticheRegionaliImprese>)memoryCache["RicercaPraticheAzienda"];
            //}

            var _confermati = _query?.Where(x => x.StatoPraticaId == (int)SediinPraticheRegionaliEnums.StatoPratica.Confermata);

            var _netto = _confermati.Sum(x => x.ImportoContributoNetto);
            var _liquidati = _confermati.Where(c => c.LiquidazionePraticheRegionali.Where(x => x.Liquidazione.StatoLiquidazioneId == (int)SediinPraticheRegionaliEnums.StatoLiqidazione.Liquidata).Count() > 0)?.Sum(x => x.ImportoContributoNetto);
            var _inliquidazione = _confermati.Where(c => c.LiquidazionePraticheRegionali.Where(x => x.Liquidazione.StatoLiquidazioneId == (int)SediinPraticheRegionaliEnums.StatoLiqidazione.InLiquidazione).Count() > 0)?.Sum(x => x.ImportoContributoNetto);
            var _daliquidare = _netto - (_liquidati + _inliquidazione);

            PraticheAziendaRicercaViewModel _result = GeModelWithPaging<PraticheAziendaRicercaViewModel, PraticheRegionaliImprese>(page, _query, model, 10);
            _result.ImportoRiconoscitoNetto = _netto;
            _result.ImportoDaLiquidare = _daliquidare;
            _result.ImportoInLiquidare = _inliquidazione;
            _result.ImportoLiquidato = _liquidati;

            return AjaxView("RicercaList", _result);
        }

        public ActionResult RicercaExcel(PraticheAziendaRicercaModel model)
        {
            var _query = from a in unitOfWork.PraticheRegionaliImpreseRepository.Get(RicercaFilter(model)).AsQueryable()
                .OrderBy(HttpUtility.UrlDecode(model.PraticheAziendaRicercaModel_OrderBy))
                         select new
                         {
                             PrestazioneRegionale = a.TipoRichiesta.IsTipoRichiestaDipendente == true ? "Dipendenti" : "Aziende",
                             Stato = a.StatoPratica.Descrizione,
                             TipoRichiesta = a.TipoRichiesta.Descrizione + " (" + a.TipoRichiesta.Anno + ")",
                             RagioneSociale = a.Azienda != null ? $"{a.Azienda.RagioneSociale}" : "",
                             MatricolaInps = a.Azienda != null ? $"{a.Azienda.MatricolaInps}" : "",
                             //ConsulenteSportello = a.ConsulenteCS != null ? $"{a.ConsulenteCS.RagioneSociale}" : "",
                             //ConsulenteSportelloCodiceFiscalePiva = a.ConsulenteCS != null ? $"{a.ConsulenteCS.CodiceFiscalePIva}" : "",
                             Dipendente = a.TipoRichiesta.IsTipoRichiestaDipendente == true ? (a.Dipendente != null ? $"{a.Dipendente.Nome} {a.Dipendente.Cognome}" : "") : null,
                             DipendenteCodiceFiscale = a.TipoRichiesta.IsTipoRichiestaDipendente == true ? (a.Dipendente != null ? $"{a.Dipendente.CodiceFiscale}" : "") : null,
                             DataRichiesta = a.DataInserimento.ToShortDateString(),
                             DataInvio = a.DataInvio != null ? a.DataInvio.Value.ToShortDateString() : null,
                             Protocollo = a.ProtocolloId,
                             ImportoContributo = a.ImportoContributo,
                             AliquoteIRPEF = a.AliquoteIRPEF,
                             ImportoIRPEF = a.ImportoIRPEF,
                             ImportoRichiesta = a.ImportoContributoNetto,
                             Note = azcoperta(a.Azienda)
                         };

            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(_query, "PraticheRegionali");
        }

        private object azcoperta(Azienda azienda)
        {
            try
            {
                var _m = "L'azienda non risulta in regola con i contributi";

                if (azienda != null)
                {
                    if (azienda.Copertura == null)
                    {
                        return _m;
                    }

                    if (azienda.Copertura?.FirstOrDefault() == null)
                    {
                        return _m;
                    }

                    if (azienda.Copertura?.FirstOrDefault()?.Coperto == false)
                    {
                        return _m;
                    }


                    return "";
                }

                return "";

            }
            catch
            {
                return "";
            }
        }

        private Expression<Func<PraticheRegionaliImprese, bool>> RicercaFilter(PraticheAziendaRicercaModel model)
        {
            TrimAll(model);

            DateTime _d1 = DateTime.Now;
            DateTime? _datastart = null;
            if (!string.IsNullOrWhiteSpace(model.PraticheAziendaRicercaModel_DataInvio))
            {
                DateTime.TryParse(HttpUtility.UrlDecode(model.PraticheAziendaRicercaModel_DataInvio), out _d1);
                _datastart = new DateTime(_d1.Year, _d1.Month, _d1.Day, 0, 0, 0);
            }

            DateTime? _dataend = null;
            if (!string.IsNullOrWhiteSpace(model.PraticheAziendaRicercaModel_DataInvio))
            {
                _dataend = new DateTime(_d1.Year, _d1.Month, _d1.Day, 23, 59, 59);
            }

            List<int> _praticheliq = new List<int>();

            if (model.PraticheAziendaRicercaModel_StatoLiquidazioneId == 0)
            {
                _praticheliq = unitOfWork.LiquidazionePraticheRegionaliRepository.
                   Get(x => x.Liquidazione.StatoLiquidazioneId == (int)SediinPraticheRegionaliEnums.StatoLiqidazione.InLiquidazione
                   || x.Liquidazione.StatoLiquidazioneId == (int)SediinPraticheRegionaliEnums.StatoLiqidazione.Liquidata)
                   .Select(c => c.PraticheRegionaliImpreseId).ToList();
            }

            var _isazienda = IsInRole(new Roles[] { Roles.Azienda });
            var _isdipendente = IsInRole(new Roles[] { Roles.Dipendente });
            var _issportello = IsInRole(new Roles[] { Roles.Sp_CAF, Roles.Sp_Consulente, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebinter });

            return x => //((IsUserConsulenteCs ? (x.SportelloId == (int)GetConsulenteCsId.Value && x.DipendenteId == null) : true)
            ((_isazienda ? (x.AziendaId == (int)GetAziendaId.Value && x.DipendenteId == null) : true)
            && (_isdipendente ? (x.DipendenteId == (int)GetDipendenteId.Value) : true)
            && (_issportello ? (x.SportelloId == (int)GetSportelloId.Value) : true))
            && ((model.PraticheAziendaRicercaModel_TipoRichiestaId != null && (model.PraticheAziendaRicercaModel_TipoRichiestaId != 0 && model.PraticheAziendaRicercaModel_TipoRichiestaId != -1) ? x.TipoRichiestaId == model.PraticheAziendaRicercaModel_TipoRichiestaId : true)
            && (model.PraticheAziendaRicercaModel_TipoRichiestaId != null && (model.PraticheAziendaRicercaModel_TipoRichiestaId == 0 || model.PraticheAziendaRicercaModel_TipoRichiestaId == -1) ? (model.PraticheAziendaRicercaModel_TipoRichiestaId == 0 ? x.TipoRichiesta.IsTipoRichiestaDipendente != true : x.TipoRichiesta.IsTipoRichiestaDipendente == true) : true)
            && (model.PraticheAziendaRicercaModel_DataInvio != null ? x.DataInvio != null && (_datastart < x.DataInvio && _dataend > x.DataInvio) : true)
            && (model.PraticheAziendaRicercaModel_AziendaId != null ? x.AziendaId == model.PraticheAziendaRicercaModel_AziendaId /*&& x.DipendenteId == null*/ : true)
            && (model.PraticheAziendaRicercaModel_SportelloId != null ? x.SportelloId == model.PraticheAziendaRicercaModel_SportelloId : true)

            && (model.PraticheAziendaRicercaModel_StatoLiquidazioneId == null ? true : (
            (model.PraticheAziendaRicercaModel_StatoLiquidazioneId == 0 ? !_praticheliq.Contains(x.PraticheRegionaliImpreseId) : (model.PraticheAziendaRicercaModel_StatoLiquidazioneId != null ?
            x.LiquidazionePraticheRegionali.Where(xx => xx.Liquidazione.StatoLiquidazioneId == model.PraticheAziendaRicercaModel_StatoLiquidazioneId).Count() > 0 : true))
            ))

            && (model.PraticheAziendaRicercaModel_DipendenteId != null ? x.DipendenteId == model.PraticheAziendaRicercaModel_DipendenteId : true)
            && (model.PraticheAziendaRicercaModel_ProtocolloId != null ? x.ProtocolloId.Contains(model.PraticheAziendaRicercaModel_ProtocolloId) : true)
            && (model.PraticheAziendaRicercaModel_StatoPraticaId != null ? (model.PraticheAziendaRicercaModel_StatoPraticaId == -1 ? x.StatoPraticaId == (int)SediinPraticheRegionaliEnums.StatoPratica.Inviata || x.StatoPraticaId == (int)SediinPraticheRegionaliEnums.StatoPratica.InviataRevisionata : x.StatoPraticaId == model.PraticheAziendaRicercaModel_StatoPraticaId) : true)
            && (model.PraticheAziendaRicercaModel_InseritaoSportello != null ? model.PraticheAziendaRicercaModel_InseritaoSportello == true
            ? x.SportelloId != null : x.SportelloId == null : true));
        }

        #endregion

        #region nuova richiesta

        public ActionResult NuovaRichiesta(bool isTipoRichiestaDipendente)
        {
            if (!isTipoRichiestaDipendente && IsInRole(new Roles[] { Roles.Admin, Roles.Azienda, Roles.Sp_Consulente, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebinter }))
            {
                PraticheAziendaNuovaRichiestaAzienda model = new PraticheAziendaNuovaRichiestaAzienda();
                model.TipoRichiesta = GetTipoRichieste().Where(a => a.IsTipoRichiestaDipendente != true && a.AbilitatoNuovaRichiesta == true);

                if (IsInRole(Roles.Azienda))
                {
                    model.PraticheAziendaNuovaRichiesta_AziendaId = GetAziendaId.Value;
                }

                if (IsInRole(new Roles[] { Roles.Sp_Consulente, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebinter }))
                {
                    model.PraticheAziendaNuovaRichiesta_SportelloId = GetSportelloId.Value;
                }

                return AjaxView("NuovaRichiestaAzienda", model);
            }

            if (isTipoRichiestaDipendente && IsInRole(new Roles[] { Roles.Admin, Roles.Dipendente, Roles.Sp_CAF, Roles.Sp_Consulente, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebinter }))
            {
                PraticheAziendaNuovaRichiestaDipendente model = new PraticheAziendaNuovaRichiestaDipendente();
                model.TipoRichiesta = GetTipoRichieste().Where(a => a.IsTipoRichiestaDipendente == true && a.AbilitatoNuovaRichiesta == true);

                if (IsInRole(Roles.Dipendente))
                {
                    model.PraticheAziendaNuovaRichiesta_DipendenteId = GetDipendenteId.Value;
                }

                if (IsInRole(new Roles[] { Roles.Sp_CAF, Roles.Sp_Consulente, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebinter }))
                {
                    model.PraticheAziendaNuovaRichiesta_SportelloId = GetSportelloId.Value;
                }

                return AjaxView("NuovaRichiestaDipendente", model);
            }

            throw new Exception("Tipo richiesta non valida");
        }

        [HttpPost]
        public ActionResult NuovaRichiestaAzienda(PraticheAziendaNuovaRichiestaAzienda model)
        {
            PraticheAziendaNuovaRichiesta _model = new PraticheAziendaNuovaRichiesta
            {
                PraticheAziendaNuovaRichiesta_AziendaId = model.PraticheAziendaNuovaRichiesta_AziendaId,
                PraticheAziendaNuovaRichiesta_TipoRichiestaId = model.PraticheAziendaNuovaRichiesta_TipoRichiestaId
            };

            //check se azienda ha completato anagrafica
            var _result = unitOfWork.AziendaRepository.Get(x => x.AziendaId == model.PraticheAziendaNuovaRichiesta_AziendaId).FirstOrDefault();
            var _valid = IsValidModel(new object[] { _result });

            if (_valid.Count() > 0)
            {
                throw new Exception($"Attenzione, per inserire una richiesta è necessario completare l'anagrafica dell'azienda \"{_result.RagioneSociale}\"");
            }

            return CreateNuovaRichiesta(_model);
        }

        [HttpPost]
        public ActionResult NuovaRichiestaDipendente(PraticheAziendaNuovaRichiestaDipendente model)
        {
            PraticheAziendaNuovaRichiesta _model = new PraticheAziendaNuovaRichiesta
            {
                PraticheAziendaNuovaRichiesta_AziendaId = model.PraticheAziendaNuovaRichiesta_AziendaId,
                PraticheAziendaNuovaRichiesta_DipendenteId = model.PraticheAziendaNuovaRichiesta_DipendenteId,
                PraticheAziendaNuovaRichiesta_TipoRichiestaId = model.PraticheAziendaNuovaRichiesta_TipoRichiestaId
            };

            //check se dipendente ha completato anagrafica
            var _result = unitOfWork.DipendenteRepository.Get(x => x.DipendenteId == model.PraticheAziendaNuovaRichiesta_DipendenteId).FirstOrDefault();
            var _valid = IsValidModel(new object[] { _result });

            if (_valid.Count() > 0)
            {
                throw new Exception($"Attenzione, per inserire una richiesta, completa prima anagrafica del dipendente \"{_result.Nome} {_result.Cognome}\"");
            }

            return CreateNuovaRichiesta(_model);
        }

        ActionResult CreateNuovaRichiesta(PraticheAziendaNuovaRichiesta model)
        {
            try
            {
                var _tiporichiesta = GetTipoRichieste().FirstOrDefault(xx => xx.AbilitatoNuovaRichiesta == true && xx.TipoRichiestaId == model.PraticheAziendaNuovaRichiesta_TipoRichiestaId);

                if (IsInRole(new Roles[] { Roles.Azienda }))
                {
                    model.PraticheAziendaNuovaRichiesta_AziendaId = GetAziendaId;
                }

                if (IsInRole(new Roles[] { Roles.Dipendente }))
                {
                    model.PraticheAziendaNuovaRichiesta_DipendenteId = GetDipendenteId;
                }

                if (IsInRole(new Roles[] { Roles.Sp_CAF, Roles.Sp_Consulente, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebinter }))
                {
                    //check se azienda/dipendente e associato
                    var _sportelloId = GetSportelloId;

                    var _dipendente = unitOfWork.DipendenteRepository.Get(x => x.DipendenteId == model.PraticheAziendaNuovaRichiesta_DipendenteId).FirstOrDefault();
                    if (_dipendente != null && _tiporichiesta.IsTipoRichiestaDipendente.GetValueOrDefault() && _dipendente.SportelloId != _sportelloId)
                    {
                        throw new Exception("Dipendente non e associato alla sua Utenza");
                    }

                    var _azienda = unitOfWork.AziendaRepository.Get(x => x.AziendaId == model.PraticheAziendaNuovaRichiesta_AziendaId).FirstOrDefault();
                    if (_azienda != null && !_tiporichiesta.IsTipoRichiestaDipendente.GetValueOrDefault() && _azienda.SportelloId != _sportelloId)
                    {
                        throw new Exception("Azienda non e associata alla sua Utenza");
                    }
                }

                var _valid = IsValidModel(new object[] { model });

                if (_valid.Count() > 0)
                {
                    throw new Exception(ErrorsToString(_valid));
                }

                if (_tiporichiesta == null)
                {
                    throw new Exception("Tipo richiesta non valida");
                }

                VerificaMaxRichiesteAzienda(_tiporichiesta, model.PraticheAziendaNuovaRichiesta_AziendaId.GetValueOrDefault(), model.PraticheAziendaNuovaRichiesta_DipendenteId);
                VerificaTettoMassimoAnnuale(_tiporichiesta.IsTipoRichiestaDipendente, model.PraticheAziendaNuovaRichiesta_AziendaId.GetValueOrDefault(), model.PraticheAziendaNuovaRichiesta_DipendenteId);

                //contaner di base
                PraticheAziendaContainer outModel = new PraticheAziendaContainer
                {
                    TipoRichiestaId = model.PraticheAziendaNuovaRichiesta_TipoRichiestaId.Value,
                    AziendaId = model.PraticheAziendaNuovaRichiesta_AziendaId.GetValueOrDefault(),
                    DipendenteId = model.PraticheAziendaNuovaRichiesta_DipendenteId,
                    View = _tiporichiesta.View,
                    DescrizioneStato = "Bozza",
                    DescrizioneTipoRichiesta = _tiporichiesta.Descrizione,
                    NoteTipoRichiesta = _tiporichiesta.Note,
                    IbanAziendaRequired = _tiporichiesta.IbanAziendaRequired.GetValueOrDefault(),
                    IbanDipendenteRequired = _tiporichiesta.IbanDipendenteRequired.GetValueOrDefault(),
                    IsTipoRichiestaDipendente = _tiporichiesta.IsTipoRichiestaDipendente.GetValueOrDefault(),
                    IbanTitolareRequired = _tiporichiesta.IbanTitolareRequired.GetValueOrDefault()
                };

                var _dataModel = CreateInstance(_tiporichiesta.Classe);
                SetProperty(_dataModel, "AziendaId", outModel.AziendaId);
                SetProperty(_dataModel, "DipendenteId", outModel.DipendenteId);
                SetProperty(_dataModel, "TipoRichiestaId", _tiporichiesta.TipoRichiestaId);
                SetProperty(_dataModel, "TipoRichiesta", _tiporichiesta);

                SetProperty(_dataModel, "CodiceFiscale", IsInRole(new Roles[] { Roles.Dipendente }) ? User.Identity.Name : unitOfWork.DipendenteRepository.Get(x => x.DipendenteId == model.PraticheAziendaNuovaRichiesta_DipendenteId).FirstOrDefault()?.CodiceFiscale);

                foreach (var item in _dataModel.GetType().GetProperties())
                {
                    foreach (var _attributes in item.GetCustomAttributes(true))
                    {
                        if (_attributes is VerificaTipoRichiestaUnivocoCodiceFiscaleValidator)
                        {
                            var _attribute = ((VerificaTipoRichiestaUnivocoCodiceFiscaleValidator)_attributes);

                            if (item.Name == _attribute.NomeCampo)
                            {
                                var _v = item.GetValue(_dataModel);

                                if (PraticheAziendaUtility.VerificaTipoRichiestaUnivocoCodiceFiscale(model.PraticheAziendaNuovaRichiesta_AziendaId.GetValueOrDefault(), _tiporichiesta.TipoRichiestaId, _v?.ToString(), 0, _attribute.NomeCampo, _attribute.Unica))
                                {
                                    // _errorsUnivocoCodiceFiscale.Add($"Per il Codice Fiscale <strong>{_v}</strong> e già stato fatto una richiesta");
                                    throw new Exception($"Per il Codice Fiscale {_v} e già stata presentata una richiesta");
                                    //throw new Exception(_attribute.ErrorMessage);
                                }
                            }
                        }
                    }
                }

                outModel.DataModel = _dataModel;

                return AjaxView("PraticheAziendaContainer", outModel);
            }
            catch (Exception ex)
            {
                if (Request.IsAjaxRequest())
                {
                    throw;
                }
                return AjaxView("Error", new HandleErrorInfo(ex, "PraticheController", "Apri"));
                //return JsonResultFalse(ex.Message);
            }
        }

        #endregion

        public void VerificaMaxRichiesteAzienda(TipoRichiesta tiporichiesta, int? aziendaId = null, int? dipendenteId = null, int? richiestaId = null)
        {
            try
            {
                if (tiporichiesta.MaxRichiesteAnno.HasValue)
                {
                    if (tiporichiesta.MaxRichiesteAnno == 0)
                    {
                        throw new Exception("Non e possibile effetuare questo tipo di richiesta");
                    }

                    //verificare l'univocità della richiesta, per l'anno corrente
                    //dovrà essere inserita massimo N richieste.
                    IEnumerable<PraticheRegionaliImprese> _richieste = null;

                    //check dipendente
                    if (tiporichiesta.IsTipoRichiestaDipendente.GetValueOrDefault())
                    {
                        _richieste = unitOfWork.PraticheRegionaliImpreseRepository.Get(xx =>
                        (richiestaId.HasValue ? xx.PraticheRegionaliImpreseId != richiestaId : true)
                        && (xx.DipendenteId == dipendenteId)
                        && (xx.TipoRichiestaId == tiporichiesta.TipoRichiestaId)
                        && ((xx.StatoPraticaId == (int)SediinPraticheRegionaliEnums.StatoPratica.Confermata)
                        || (xx.StatoPraticaId == (int)SediinPraticheRegionaliEnums.StatoPratica.Inviata)
                        || (xx.StatoPraticaId == (int)SediinPraticheRegionaliEnums.StatoPratica.InviataRevisionata)));
                    }
                    else
                    {
                        _richieste = unitOfWork.PraticheRegionaliImpreseRepository.Get(xx =>
                        (richiestaId.HasValue ? xx.PraticheRegionaliImpreseId != richiestaId : true)
                        && (xx.AziendaId == aziendaId)
                        && (xx.TipoRichiestaId == tiporichiesta.TipoRichiestaId)
                        && ((xx.StatoPraticaId == (int)SediinPraticheRegionaliEnums.StatoPratica.Confermata)
                        || (xx.StatoPraticaId == (int)SediinPraticheRegionaliEnums.StatoPratica.Inviata)
                        || (xx.StatoPraticaId == (int)SediinPraticheRegionaliEnums.StatoPratica.InviataRevisionata)));
                    }

                    //check
                    if (_richieste?.Count() > 0)
                    {
                        //una tantum
                        if (tiporichiesta.UnaTantum.GetValueOrDefault())
                        {
                            throw new Exception("Per questo tipo di richiesta, il contributo è previsto Una Tantum");
                        }

                        //annuale
                        var _anno = DateTime.Now.Year;
                        if (tiporichiesta.MaxRichiesteAnno.GetValueOrDefault() > 0 && _richieste.Where(x => x.DataInvio.GetValueOrDefault().Year == _anno).Count() >= tiporichiesta.MaxRichiesteAnno.GetValueOrDefault())
                        {
                            throw new Exception("Per questo tipo di richiesta, ha già raggiunto il limite massimo annuale");
                        }


                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public void VerificaTettoMassimoAnnuale(bool? isTipoRichiestaDipendente, int? aziendaId = null, int? dipendenteId = null, int? richiestaId = null, decimal? importoRichiesto = null, DateTime? dataInvio = null)
        {
            try
            {
                var _data = dataInvio.HasValue ? dataInvio : DateTime.Now.Date;
                var _praticheRegionaliImprese = isTipoRichiestaDipendente.GetValueOrDefault() ? Roles.Dipendente.ToString() : Roles.Azienda.ToString();

                var _contatoreAnnuale = unitOfWork.ContatoreAnnualeRepository.Get(x => (x.DataInizio <= _data
                && x.DataFine >= _data)
                && (x.PraticheRegionaliImprese == _praticheRegionaliImprese)).FirstOrDefault();

                if (_contatoreAnnuale == null)
                {
                    throw new Exception("Contatore annuale non impostato per data richiesta");
                }

                //solo richieste confermate
                IEnumerable<PraticheRegionaliImprese> _richieste = null;

                if (isTipoRichiestaDipendente.GetValueOrDefault())
                {
                    _richieste = unitOfWork.PraticheRegionaliImpreseRepository.Get(xx =>
                    (richiestaId.HasValue ? xx.PraticheRegionaliImpreseId != richiestaId : true)
                    && (xx.DipendenteId == dipendenteId)
                    && ((xx.StatoPraticaId == (int)SediinPraticheRegionaliEnums.StatoPratica.Confermata) &&
                    (_contatoreAnnuale.DataInizio <= xx.DataInvio && _contatoreAnnuale.DataFine >= xx.DataInvio)));
                }
                else
                {
                    _richieste = unitOfWork.PraticheRegionaliImpreseRepository.Get(xx =>
                    (richiestaId.HasValue ? xx.PraticheRegionaliImpreseId != richiestaId : true)
                    && (xx.AziendaId == aziendaId && xx.DipendenteId == null)
                    && ((xx.StatoPraticaId == (int)SediinPraticheRegionaliEnums.StatoPratica.Confermata) &&
                    (_contatoreAnnuale.DataInizio <= xx.DataInvio && _contatoreAnnuale.DataFine >= xx.DataInvio)));
                }

                var _totaleImportoContributo = 0m;

                if (_richieste?.Count() != 0)
                {
                    _totaleImportoContributo = _richieste.Sum(x => x.ImportoContributo).GetValueOrDefault();
                }

                _totaleImportoContributo += importoRichiesto.GetValueOrDefault();

                if (_totaleImportoContributo > _contatoreAnnuale.TettoMassimoLordo.GetValueOrDefault())
                {
                    throw new Exception("Tetto massimo annuale superato");
                }
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult ApriRichiesta(int id)
        {
            try
            {
                var _richiesta = unitOfWork.PraticheRegionaliImpreseRepository.Get(xx => xx.PraticheRegionaliImpreseId == id).FirstOrDefault();

                if (_richiesta == null)
                {
                    throw new Exception("Richiesta non trovata");
                }

                CheckUserAbilitatoRichiesta(_richiesta);

                var _tiporichiesta = GetTipoRichieste().FirstOrDefault(xx => xx.TipoRichiestaId == _richiesta.TipoRichiestaId);

                if (_tiporichiesta == null)
                {
                    throw new Exception("Tipo richiesta non valida");
                }

                #region check se la richiesta e editabile

                var readOnly = _richiesta?.StatoPratica.ReadOnly;

                if (!readOnly.GetValueOrDefault())
                {
                    if (/*_richiesta.StatoPraticaId == (int)SediinPraticheRegionaliEnums.StatoPratica.Bozza ||*/ _richiesta.StatoPraticaId == (int)SediinPraticheRegionaliEnums.StatoPratica.Revisione)
                    {
                        readOnly = IsInRole(Roles.Admin);
                    }
                }

                ////21/02/2024
                //if (IsInRole(Roles.Admin) && _richiesta.StatoPraticaId == (int)SediinPraticheRegionaliEnums.StatoPratica.Inviata || _richiesta.StatoPraticaId == (int)SediinPraticheRegionaliEnums.StatoPratica.InviataRevisionata)
                //{
                //    readOnly = false;
                //}

                #endregion

                var _dataModel = CreateModelDatiRichiesta(_tiporichiesta, _richiesta.DatiPratica);

                var _childClass = CreateModelDatiRichiestaChildClass(_tiporichiesta, _richiesta.DatiPratica, _richiesta.ChildClassRowCount);

                SetProperty(_dataModel, "ChildClass", _childClass);

                SetProvicia(_dataModel, "ProvinciaId", "Provincia");
                SetRegione(_dataModel, "RegioneId", "Regione");
                SetComune(_dataModel, "ComuneId", "Comune");
                SetLocalita(_dataModel, "LocalitaId", "Localita");

                SetProperty(_dataModel, "Iban", _richiesta.Iban);
                SetProperty(_dataModel, "StatoPraticaId", _richiesta.StatoPraticaId);
                SetProperty(_dataModel, "RichiestaId", _richiesta.PraticheRegionaliImpreseId);
                SetProperty(_dataModel, "DipendenteId", _richiesta.DipendenteId);
                SetProperty(_dataModel, "AziendaId", _richiesta.AziendaId);
                SetProperty(_dataModel, "TipoRichiestaId", _tiporichiesta.TipoRichiestaId);
                SetProperty(_dataModel, "TipoRichiesta", _tiporichiesta);
                SetProperty(_dataModel, "ReadOnly", readOnly);

                SetProperty(_dataModel, "ImportoRichiesto", _richiesta.ImportoRichiesto);
                SetProperty(_dataModel, "AliquoteIRPEF", _richiesta.AliquoteIRPEF);
                SetProperty(_dataModel, "PercentualeContributo", _richiesta.PercentualeContributo);
                SetProperty(_dataModel, "ImportoIRPEF", _richiesta.ImportoIRPEF);
                SetProperty(_dataModel, "ImportoContributo", _richiesta.ImportoContributo);
                SetProperty(_dataModel, "ImportoContributoNetto", _richiesta.ImportoContributoNetto);
                SetProperty(_dataModel, "ContributoFisso", _richiesta.ContributoFisso);
                SetProperty(_dataModel, "ContributoImportoMinimo", _richiesta.ContributoImportoMinimo);
                SetProperty(_dataModel, "ContributoImportoMassimo", _richiesta.ContributoImportoMassimo);

                var _liquidataOinLiquidazione = _richiesta.LiquidazionePraticheRegionali?.Where(x => x.Liquidazione.StatoLiquidazioneId != (int)SediinPraticheRegionaliEnums.StatoLiqidazione.Annullata)?.Count() > 0;

                var _descrizioneStato = _richiesta?.StatoPratica?.Descrizione;

                //contaner di base
                PraticheAziendaContainer outModel = new PraticheAziendaContainer
                {
                    AziendaId = _richiesta.AziendaId,
                    RichiestaId = _richiesta.PraticheRegionaliImpreseId,
                    TipoRichiestaId = _richiesta.TipoRichiestaId,
                    StatoId = _richiesta.StatoPraticaId,
                    ProtocolloId = _richiesta.ProtocolloId,
                    DataModel = _dataModel,
                    View = _tiporichiesta.View,
                    DescrizioneStato = _descrizioneStato,
                    DescrizioneTipoRichiesta = _tiporichiesta.Descrizione,
                    NoteTipoRichiesta = _tiporichiesta.Note,
                    IbanAziendaRequired = _tiporichiesta.IbanAziendaRequired.GetValueOrDefault(),
                    IbanDipendenteRequired = _tiporichiesta.IbanDipendenteRequired.GetValueOrDefault(),
                    ReadOnly = readOnly,
                    StoricoStatoPratica = _richiesta.StatoPraticaStorico,
                    DipendenteId = _richiesta.DipendenteId,
                    ChildClassRowCount = _richiesta.ChildClassRowCount,
                    AliquoteIRPEF = _richiesta.AliquoteIRPEF,
                    ImportoRichiesto = _richiesta.ImportoRichiesto,
                    ImportoContributo = _richiesta.ImportoContributo,
                    ImportoContributoNetto = _richiesta.ImportoContributoNetto,
                    ImportoIRPEF = _richiesta.ImportoIRPEF,
                    Iban = _richiesta.Iban,
                    LiquidataOinLiquidazione = _liquidataOinLiquidazione,
                    PraticheRegionaliImprese = _richiesta,
                    Responsabilita = _richiesta.Responsabilita.GetValueOrDefault(),
                    IsTipoRichiestaDipendente = _tiporichiesta.IsTipoRichiestaDipendente.GetValueOrDefault(),
                    IbanTitolareRequired = _tiporichiesta.IbanTitolareRequired.GetValueOrDefault()
                };

                return AjaxView("PraticheAziendaContainer", outModel);
            }
            catch (Exception ex)
            {
                if (Request.IsAjaxRequest())
                {
                    throw;
                }
                return AjaxView("Error", new HandleErrorInfo(ex, "PraticheController", "Apri"));
            }
        }

        public ActionResult ContatoreAnnuale(int? aziendaId, int? dipendenteId)
        {
            if (aziendaId == null && dipendenteId == null)
            {
                return null;
            }

            var _isDipendete = dipendenteId != null;

            decimal? getimporto(DateTime? datainizio, DateTime? datafine)
            {
                try
                {
                    if (_isDipendete)
                    {
                        return unitOfWork.PraticheRegionaliImpreseRepository.Get(xx =>
                          (xx.DipendenteId == dipendenteId)
                         && ((xx.StatoPraticaId == (int)SediinPraticheRegionaliEnums.StatoPratica.Confermata) &&
                         (datainizio <= xx.DataInvio && datafine >= xx.DataInvio)))
                            .Sum(x => x.ImportoContributo).GetValueOrDefault();
                    }
                    else
                    {
                        return unitOfWork.PraticheRegionaliImpreseRepository.Get(xx =>
                          (xx.AziendaId == aziendaId && xx.DipendenteId == null)
                         && ((xx.StatoPraticaId == (int)SediinPraticheRegionaliEnums.StatoPratica.Confermata) &&
                         (datainizio <= xx.DataInvio && datafine >= xx.DataInvio)))
                            .Sum(x => x.ImportoContributo).GetValueOrDefault();
                    }

                }
                catch
                {
                    return 0m;
                }
            }

            var _contatore = from x in unitOfWork.ContatoreAnnualeRepository.Get(c => _isDipendete ? c.PraticheRegionaliImprese == Roles.Dipendente.ToString() : c.PraticheRegionaliImprese == Roles.Azienda.ToString())
                             select new PraticheAzienda_ContatoreAnnuale
                             {
                                 DataFine = x.DataFine,
                                 DataInizio = x.DataInizio,
                                 ImportoRichieste = getimporto(x.DataInizio, x.DataFine),
                                 TettoMassimo = x.TettoMassimoLordo
                             };


            return PartialView("~/Areas/Backend/Views/Pratiche/ContatoreAnnuale.cshtml", _contatore.ToList());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SalvaRichiesta(PraticheAziendaContainer container, PraticheAziendaAllegati upload, PraticheAziendaRequisiti requisiti, PraticheAziendaRichidenti richiedenti, PraticheAziendaDpr dpr, FormCollection form) //, int iichiestaId, int aziendaId, int tipoRichiestaId)
        {
            try
            {
                var _tiporichiesta = GetTipoRichieste().FirstOrDefault(xx => xx.TipoRichiestaId == container.TipoRichiestaId) ?? throw new Exception("Tipo richiesta non valida");

                //se azienda, settare aziendaid
                var _aziendaId = container.AziendaId;

                if (IsInRole(new Roles[] { Roles.Azienda }))
                {
                    _aziendaId = GetAziendaId.Value;
                }

                //se dipendente, setare dipendenteId
                int? _dipendenteId = container.DipendenteId;
                if (IsInRole(new Roles[] { Roles.Dipendente }))
                {
                    _dipendenteId = GetDipendenteId.Value;
                }

                int? _sportelloId = null;
                if (IsInRole(new Roles[] { Roles.Sp_CAF, Roles.Sp_Consulente, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebinter }))
                {
                    //    _dipendenteId = container.DipendenteId;
                    _sportelloId = GetSportelloId.Value;
                }

                VerificaMaxRichiesteAzienda(_tiporichiesta, _aziendaId, _dipendenteId, container.RichiestaId);
                VerificaTettoMassimoAnnuale(_tiporichiesta.IsTipoRichiestaDipendente, _aziendaId, _dipendenteId, container.RichiestaId);

                //get pratica se gia esiste in tb PraticheRegionaliImpreseRepository
                var _richiesta = unitOfWork.PraticheRegionaliImpreseRepository.Get(xx => xx.PraticheRegionaliImpreseId == container.RichiestaId).FirstOrDefault();

                //check stato richiesta
                if (_richiesta != null)
                {
                    if (_richiesta.StatoPraticaId == (int)SediinPraticheRegionaliEnums.StatoPratica.Annullata
                        && container.Azione != SediinPraticheRegionaliEnums.AzioniPratica.RimettiComeInviata.ToString())
                    {
                        throw new Exception("Richiesta non modificabile");
                    }

                    if (IsInRole(new Roles[] { Roles.Admin, Roles.Super, Roles.Sp_Ebinter }) && container.Azione == SediinPraticheRegionaliEnums.AzioniPratica.RimettiComeInviata.ToString())
                    {
                        _richiesta.StatoPraticaId = (int)SediinPraticheRegionaliEnums.StatoPratica.Inviata;
                        unitOfWork.Save(false);
                        return JsonResultTrue(_richiesta.PraticheRegionaliImpreseId, "Stato pratica aggiornato");
                    }
                }

                CheckUserAbilitatoRichiesta(_richiesta);

                //crea modello
                var _DatiPratica = CreateInstance(_tiporichiesta.Classe);

                //setta valori del model
                foreach (var item in _DatiPratica.GetType().GetProperties())
                {
                    Reflection.SetValue(_DatiPratica, item.Name, form[item.Name]);
                }

                //modello child
                List<string> _errorsEventi = null;

                //errors UnivocoCodiceFiscale
                List<string> _errorsUnivocoCodiceFiscale = new List<string>();

                Dictionary<string, string> _dicDatiPraticaEventi = new Dictionary<string, string>();

                if (container.ChildClassRowCount > 0)
                {
                    for (int i = 0; i < container.ChildClassRowCount; i++)
                    {
                        var _DatiPraticaEventiModel = CreateInstance(_tiporichiesta.ChildClass);

                        foreach (var item in _DatiPraticaEventiModel.GetType().GetProperties())
                        {
                            var _n = "ChildClass" + "[" + i + "]." + item.Name;
                            var _v = form[_n];

                            //UnivocoCodiceFiscale
                            if (container.Azione == SediinPraticheRegionaliEnums.AzioniPratica.Invia.ToString()
                                || container.Azione == SediinPraticheRegionaliEnums.AzioniPratica.InviaRevisionata.ToString()
                                || container.Azione == SediinPraticheRegionaliEnums.AzioniPratica.Conferma.ToString())
                            {
                                foreach (var _attributes in item.GetCustomAttributes(true))
                                {
                                    if (_attributes is VerificaTipoRichiestaUnivocoCodiceFiscaleValidator)
                                    {
                                        var _attribute = ((VerificaTipoRichiestaUnivocoCodiceFiscaleValidator)_attributes);

                                        if (item.Name == _attribute.NomeCampo)
                                        {
                                            if (PraticheAziendaUtility.VerificaTipoRichiestaUnivocoCodiceFiscale(_aziendaId, _tiporichiesta.TipoRichiestaId, _v?.ToString(), _richiesta != null ? _richiesta.PraticheRegionaliImpreseId : 0, _attribute.NomeCampo, _attribute.Unica))
                                            {
                                                _errorsUnivocoCodiceFiscale.Add($"Per il Codice Fiscale <strong>{_v}</strong> e già stata presentata una richiesta");
                                                //   throw new Exception($"Per il Codice Fiscale {_v} e già stato fatto una richiesta");
                                                //throw new Exception(_attribute.ErrorMessage);
                                            }
                                        }
                                    }
                                }
                            }

                            _dicDatiPraticaEventi.Add(_n, _v);
                            Reflection.SetValue(_DatiPraticaEventiModel, item.Name, _v);
                        }

                        _errorsEventi = container.Azione == SediinPraticheRegionaliEnums.AzioniPratica.Bozza.ToString()
                            || container.Azione == SediinPraticheRegionaliEnums.AzioniPratica.BozzaRevisionata.ToString()
                            ? null : IsValidModel(new object[] { _DatiPraticaEventiModel });
                    }
                }

                //non validare la request se viene inviata come Azione.Bozza 
                List<string> _errors = container.Azione == SediinPraticheRegionaliEnums.AzioniPratica.Bozza.ToString()
                    || container.Azione == SediinPraticheRegionaliEnums.AzioniPratica.BozzaRevisionata.ToString()
                    ? null : IsValidModel(new object[] { _DatiPratica, upload });

                if (_errorsUnivocoCodiceFiscale?.Count() > 0)
                {
                    if (_errors == null)
                    {
                        _errors = new List<string>();
                    }

                    _errors.AddRange(_errorsUnivocoCodiceFiscale);
                }

                if (_errorsEventi?.Count() > 0)
                {
                    if (_errors == null)
                    {
                        _errors = new List<string>();
                    }

                    _errors.AddRange(_errorsEventi);
                }

                if (_errors?.Count() > 0)
                {
                    throw new Exception(ErrorsToString(_errors));
                }

                decimal? _importoRichiesto = IsInRole(Roles.Admin) ? container.ImportoRichiesto.GetValueOrDefault() : _richiesta?.ImportoRichiesto.GetValueOrDefault();

                decimal? _percentuale = IsInRole(Roles.Admin) ? container.PercentualeContributo : (_richiesta != null ? _richiesta.PercentualeContributo : null);

                decimal? _importoRichiestoFromModel = null;

                var _datiPratica = CreateDatiPratica(_DatiPratica, ref _importoRichiestoFromModel);//, ref _hashContributoColumn);

                //if (_importoRichiestoFromModel.GetValueOrDefault() > 0)
                //{
                //    _importoRichiesto = _importoRichiestoFromModel;
                //}

                //calcola
                //se requisiti Max. checkbox da selezionare (esclusi obblicatori) e impostato a 1, prendere importo da TipoRichiestaRequisiti
                if (_tiporichiesta.RequisitiMassimo == 1 && requisiti?.PraticheRequisiti?.Where(x => x.Selectedt == true).Count() == 1)
                {
                    var _requisitiId = requisiti?.PraticheRequisiti.FirstOrDefault(x => x.Selectedt == true).RequisitiId;
                    var _requisito = unitOfWork.TipoRichiestaRequisitiRepository.Get(x => x.RequisitiId == _requisitiId && x.TipoRichiestaId == _tiporichiesta.TipoRichiestaId).FirstOrDefault();
                    if (_requisito.ContributoImporto.GetValueOrDefault() > 0)
                    {
                        if (_importoRichiesto > 0)
                        {
                            _importoRichiesto = _importoRichiesto / 100 * _requisito.ContributoPercentuale.GetValueOrDefault();

                            if (_importoRichiesto > _requisito.ContributoImporto)
                            {
                                _importoRichiesto = _requisito.ContributoImporto;
                            }
                        }
                        else
                        {
                            _importoRichiesto = _requisito.ContributoImporto;
                            _percentuale = _requisito.ContributoPercentuale.GetValueOrDefault();
                        }
                    }
                }

                var _calcoloContributo = new CalcolaImportoRimborsatoModel();

                if (_richiesta != null)
                {
                    if (container.Azione == SediinPraticheRegionaliEnums.AzioniPratica.Conferma.ToString()
                        || container.Azione == SediinPraticheRegionaliEnums.AzioniPratica.InviaRevisionata.ToString()
                        || container.Azione == SediinPraticheRegionaliEnums.AzioniPratica.Invia.ToString())
                    {
                        _calcoloContributo = PraticheAziendaUtility.CalcolaImportoRimborsatoContributo(_tiporichiesta.TipoRichiestaId, _importoRichiesto, _percentuale);
                    }
                }
                else
                {
                    _calcoloContributo.PercentualeContributo = _percentuale;
                    _calcoloContributo.ImportoRichiesto = _importoRichiesto;
                }

                var _aliquoteIRPEF = _calcoloContributo.AliquoteIRPEF.GetValueOrDefault();

                var _importoIREF = _calcoloContributo.ImportoIRPEF.GetValueOrDefault();

                var _importoContributoNetto = _calcoloContributo.ImportoContributoNetto.GetValueOrDefault();

                var _importoContributo = _calcoloContributo.ImportoContributo.GetValueOrDefault();

                _percentuale = _calcoloContributo.PercentualeContributo;

                var _contributoFisso = _calcoloContributo.ContributoFisso.GetValueOrDefault();

                if (_importoRichiesto.GetValueOrDefault() == 0 && _contributoFisso > 0)
                {
                    _importoRichiesto = _contributoFisso;
                }

                //check TettoMassimoAnnuale
                if (_richiesta != null)
                {
                    if (container.Azione == SediinPraticheRegionaliEnums.AzioniPratica.Conferma.ToString())
                    {
                        if (_importoRichiesto.GetValueOrDefault() <= 0)
                        {
                            throw new Exception("Inserire Importo richiesto maggiore a 0");
                        }

                        if (_percentuale.GetValueOrDefault() < 0)
                        {
                            throw new Exception("Inserire una percentuale maggiore a 0");
                        }

                        //if (_percentuale == null)
                        //{
                        //    _percentuale = 100;
                        //}

                        VerificaTettoMassimoAnnuale(_tiporichiesta.IsTipoRichiestaDipendente, _aziendaId, _dipendenteId, _richiesta.PraticheRegionaliImpreseId, _importoContributo, _richiesta.DataInvio.Value.Date);
                    }
                }

                var _contributoMin = _calcoloContributo.ContributoImportoMinimo.GetValueOrDefault();

                var _contributoMax = _calcoloContributo.ContributoImportoMassimo.GetValueOrDefault();

                //allegati da eliminare da file system dopo il salvataggio, se pratica esiste
                List<string> _filesToDelete = new List<string>();

                //cancella dati pratica
                if (_richiesta != null && container.Azione != SediinPraticheRegionaliEnums.AzioniPratica.Conferma.ToString())// && container.Azione != SediinPraticheRegionaliEnums.AzioniPratica.Conferma.ToString())
                {
                    if (_richiesta.DatiPratica?.Count() > 0)
                    {
                        foreach (var item in _richiesta.DatiPratica.ToList())
                        {
                            unitOfWork.PraticheRegionaliImpreseDatiPraticaRepository.Delete(item.PraticheRegionaliImpreseDatiPraticaId);
                        }
                    }

                    //allegati da eliminari
                    var _praticheRegionaliImpreseAllegatiId = upload?.File?.Select(x => x.PraticheRegionaliImpreseAllegatiId);

                    var _allegatidaeliminari = _richiesta.Allegati?.Where(d => !_praticheRegionaliImpreseAllegatiId.Contains(d.PraticheRegionaliImpreseAllegatiId));

                    if (_allegatidaeliminari != null && _allegatidaeliminari?.Count() > 0)
                    {
                        try
                        {
                            foreach (var item in _allegatidaeliminari?.ToList())
                            {
                                _filesToDelete.Add(item.Filename);
                                unitOfWork.PraticheRegionaliImpreseAllegatiRepository.Delete(item.PraticheRegionaliImpreseAllegatiId);
                            }
                        }
                        catch
                        {
                        }
                    }

                    //requisiti
                    var _requisiti = _richiesta.Requisiti;
                    if (_requisiti != null)
                    {
                        foreach (var item in _requisiti.ToList())
                        {
                            unitOfWork.PraticheRegionaliImpreseRequisitiRepository.Delete(item.PraticheRegionaliImpreseRequisitiId);
                        }
                    }

                    //richiedenti
                    var _richiedenti = _richiesta.Richidenti;
                    if (_richiedenti != null)
                    {
                        foreach (var item in _richiedenti.ToList())
                        {
                            unitOfWork.PraticheRegionaliImpreseRichidenteRepository.Delete(item.PraticheRegionaliImpreseRichidenteId);
                        }
                    }

                    //dpr
                    var _dpr = _richiesta.Dpr;
                    if (_dpr != null)
                    {
                        foreach (var item in _dpr.ToList())
                        {
                            unitOfWork.PraticheRegionaliImpreseDprRepository.Delete(item.PraticheRegionaliImpreseDprId);
                        }
                    }
                }

                //setta statoid della pratica
                var _statoPraticaId = _richiesta != null ? _richiesta.StatoPraticaId : 1;

                if (container.Azione == SediinPraticheRegionaliEnums.AzioniPratica.Bozza.ToString())
                {
                    _statoPraticaId = (int)SediinPraticheRegionaliEnums.StatoPratica.Bozza;
                }

                //imposta data invio
                DateTime? _dataInvio = null;
                if (_richiesta != null)
                {
                    _dataInvio = _richiesta.DataInvio;
                }

                string _usernameInvio = _richiesta != null ? _richiesta.UsernameInvio : null;

                string _ProtocolloId = _richiesta?.ProtocolloId;

                if (container.Azione == SediinPraticheRegionaliEnums.AzioniPratica.Invia.ToString())
                {
                    VerificaCoperturaAzienda(_aziendaId);

                    _dataInvio = DateTime.Now;
                    _usernameInvio = User.Identity.Name;

                    _statoPraticaId = (int)SediinPraticheRegionaliEnums.StatoPratica.Inviata;
                    if (string.IsNullOrWhiteSpace(_ProtocolloId))
                    {
                        _ProtocolloId = $"{DateTime.Now.Year}.{_richiesta.PraticheRegionaliImpreseId.ToString().PadLeft(7, '0')}";
                        //_ProtocolloId = $"{DateTime.Now.Year}{DateTime.Now.Month}.{_richiesta.PraticheRegionaliImpreseId.ToString().PadLeft(7, '0')}";
                    }
                }

                if (container.Azione == SediinPraticheRegionaliEnums.AzioniPratica.InviaRevisionata.ToString())
                {
                    VerificaCoperturaAzienda(_aziendaId);

                    _statoPraticaId = (int)SediinPraticheRegionaliEnums.StatoPratica.InviataRevisionata;
                }

                //imposta data conferma
                DateTime? _dataConferma = null;
                if (_richiesta != null)
                {
                    _dataConferma = _richiesta.DataConferma;
                }

                string _usernameConferma = _richiesta != null ? _richiesta.UsernameConferma : null;

                if (container.Azione == SediinPraticheRegionaliEnums.AzioniPratica.Conferma.ToString())
                {
                    if (User.IsInRole(IdentityHelper.Roles.Super.ToString()))
                    {
                        throw new Exception("Utente non abilitato");
                    }

                    _dataConferma = DateTime.Now;
                    _usernameConferma = User.Identity.Name;

                    VerificaCoperturaAzienda(_aziendaId);

                    _statoPraticaId = (int)SediinPraticheRegionaliEnums.StatoPratica.Confermata;
                }

                //inserimento/aggiornamento della pratica
                PraticheRegionaliImprese praticheRegionaliImprese = new PraticheRegionaliImprese
                {
                    PraticheRegionaliImpreseId = container.RichiestaId,
                    AziendaId = _richiesta != null ? _richiesta.AziendaId : _aziendaId,
                    DipendenteId = _richiesta != null ? _richiesta.DipendenteId : _dipendenteId,
                    SportelloId = _richiesta != null ? _richiesta.SportelloId : _sportelloId,
                    //unitOfWork.DipendenteRepository.Get(x => x.DipendenteId == _dipendenteId).FirstOrDefault()?.SportelloId, // User.IsInRole(IdentityHelper.Roles.ConsulenteCs.ToString()) ? GetConsulenteCsId : (_richiesta != null ? _richiesta.SportelloId : null),
                    //SportelloId = _richiesta != null ? _richiesta.SportelloId : _consulenteId,
                    DataInserimento = _richiesta != null ? _richiesta.DataInserimento : DateTime.Now,
                    DataInvio = _dataInvio,
                    StatoPraticaId = _statoPraticaId,
                    TipoRichiestaId = _richiesta != null ? _richiesta.TipoRichiestaId : container.TipoRichiestaId,
                    UserInserimento = _richiesta != null ? _richiesta.UserInserimento : User.Identity.Name,
                    RuoloUserInserimento = _richiesta != null ? _richiesta.RuoloUserInserimento : GetUserRole(),
                    ProtocolloId = _ProtocolloId,
                    ChildClassRowCount = container.ChildClassRowCount,
                    Iban = container.Iban,
                    DataConferma = _dataConferma,
                    UsernameInvio = _usernameInvio,
                    UsernameConferma = _usernameConferma,
                    Responsabilita = container.Responsabilita.GetValueOrDefault(),
                    AliquoteIRPEF = _aliquoteIRPEF,
                    PercentualeContributo = _percentuale,
                    ImportoContributo = _importoContributo,
                    ImportoIRPEF = _importoIREF,
                    ImportoContributoNetto = _importoContributoNetto,
                    ImportoRichiesto = _importoRichiesto,
                    ContributoImportoMinimo = _contributoMin,
                    ContributoImportoMassimo = _contributoMax,
                    ContributoFisso = _contributoFisso,
                };

                unitOfWork.PraticheRegionaliImpreseRepository.InsertOrUpdate(praticheRegionaliImprese);

                //commit se nuova
                if (container.RichiestaId == 0)
                {
                    unitOfWork.Save(false);
                }

                //storico stato pratica
                var _storicoStatoPratica = _richiesta?.StatoPraticaStorico?.OrderByDescending(d => d.PraticheRegionaliImpreseStatoPraticaStoricoId).FirstOrDefault();

                var _insertStorico = _storicoStatoPratica == null || _storicoStatoPratica.StatoPraticaId != _statoPraticaId;

                if (_insertStorico)
                {
                    PraticheRegionaliImpreseStatoPraticaStorico praticheRegionaliImpreseStatoPratica = new PraticheRegionaliImpreseStatoPraticaStorico
                    {
                        StatoPraticaId = _statoPraticaId,
                        DataInserimento = DateTime.Now,
                        PraticheRegionaliImpreseId = praticheRegionaliImprese.PraticheRegionaliImpreseId,
                        UserName = User.Identity.Name,
                        UserRuolo = GetUserRole()
                    };

                    unitOfWork.PraticheRegionaliImpreseStatoPraticaStoricoRepository.Insert(praticheRegionaliImpreseStatoPratica);
                }

                //insert dati pratica

                if (container.Azione != SediinPraticheRegionaliEnums.AzioniPratica.Conferma.ToString())
                {
                    if (_datiPratica?.Count() > 0)
                    {
                        foreach (var item in _datiPratica)
                        {
                            item.PraticheRegionaliImpreseId = praticheRegionaliImprese.PraticheRegionaliImpreseId;
                            unitOfWork.PraticheRegionaliImpreseDatiPraticaRepository.Insert(item);
                        }
                    }

                    if (_dicDatiPraticaEventi?.Count() > 0)
                    {
                        foreach (var item in _dicDatiPraticaEventi)
                        {
                            PraticheRegionaliImpreseDatiPratica _PraticheRegionaliImpreseDatiPratica = new PraticheRegionaliImpreseDatiPratica();
                            _PraticheRegionaliImpreseDatiPratica.PraticheRegionaliImpreseId = praticheRegionaliImprese.PraticheRegionaliImpreseId;
                            _PraticheRegionaliImpreseDatiPratica.Nome = item.Key;
                            _PraticheRegionaliImpreseDatiPratica.Valore = item.Value;
                            unitOfWork.PraticheRegionaliImpreseDatiPraticaRepository.Insert(_PraticheRegionaliImpreseDatiPratica);
                        }
                    }

                    //insert allegati pratica
                    var _allegati = CreateAllegati(upload, praticheRegionaliImprese.PraticheRegionaliImpreseId);

                    if (_allegati?.Count() > 0)
                    {
                        foreach (var item in _allegati)
                        {
                            if (item.TipoRichiestaAllegatiId == 0)
                            {
                                continue;
                            }

                            item.PraticheRegionaliImpreseId = praticheRegionaliImprese.PraticheRegionaliImpreseId;
                            unitOfWork.PraticheRegionaliImpreseAllegatiRepository.Insert(item);
                        }
                    }

                    if (requisiti?.PraticheRequisiti?.Count() > 0)
                    {
                        foreach (var item in requisiti.PraticheRequisiti)
                        {
                            if (item.Selectedt != true)
                            {
                                continue;
                            }
                            PraticheRegionaliImpreseRequisiti _praticheRegionaliImpreseRequisiti = new PraticheRegionaliImpreseRequisiti();
                            _praticheRegionaliImpreseRequisiti.PraticheRegionaliImpreseId = praticheRegionaliImprese.PraticheRegionaliImpreseId;
                            _praticheRegionaliImpreseRequisiti.RequisitiId = item.RequisitiId;
                            unitOfWork.PraticheRegionaliImpreseRequisitiRepository.Insert(_praticheRegionaliImpreseRequisiti);
                        }
                    }

                    if (richiedenti?.PraticheRichiedenti?.Count() > 0)
                    {
                        foreach (var item in richiedenti?.PraticheRichiedenti)
                        {
                            if (item.Nominativo == null && item.CodiceFiscale == null)
                            {
                                continue;
                            }

                            PraticheRegionaliImpreseRichidente _praticheRegionaliImpreseRichidente = new PraticheRegionaliImpreseRichidente();
                            _praticheRegionaliImpreseRichidente.PraticheRegionaliImpreseId = praticheRegionaliImprese.PraticheRegionaliImpreseId;
                            _praticheRegionaliImpreseRichidente.CodiceFiscale = item.CodiceFiscale;
                            _praticheRegionaliImpreseRichidente.Nominativo = item.Nominativo;
                            unitOfWork.PraticheRegionaliImpreseRichidenteRepository.Insert(_praticheRegionaliImpreseRichidente);
                        }
                    }

                    if (dpr?.PraticheDpr?.Count() > 0)
                    {
                        foreach (var item in dpr?.PraticheDpr)
                        {
                            if (item.Selectedt != true)
                            {
                                continue;
                            }
                            PraticheRegionaliImpreseDpr _praticheRegionaliImpreseDpr = new PraticheRegionaliImpreseDpr();
                            _praticheRegionaliImpreseDpr.PraticheRegionaliImpreseId = praticheRegionaliImprese.PraticheRegionaliImpreseId;
                            _praticheRegionaliImpreseDpr.DichiarazioniDPRId = item.DprId;
                            unitOfWork.PraticheRegionaliImpreseDprRepository.Insert(_praticheRegionaliImpreseDpr);
                        }
                    }
                }

                //commit tutto
                unitOfWork.Save(false);

                //aggiorna lista ricerca
                UpdateListRicerca(praticheRegionaliImprese.PraticheRegionaliImpreseId);

                //avvisa admin che ce una nuova richiesta
                if ((!User.IsInRole(Roles.Admin.ToString()) && ((_statoPraticaId == (int)SediinPraticheRegionaliEnums.StatoPratica.Inviata || _statoPraticaId == (int)SediinPraticheRegionaliEnums.StatoPratica.InviataRevisionata))) ||
                    (!User.IsInRole(Roles.Super.ToString()) && ((_statoPraticaId == (int)SediinPraticheRegionaliEnums.StatoPratica.Inviata || _statoPraticaId == (int)SediinPraticheRegionaliEnums.StatoPratica.InviataRevisionata))))
                {
                    AvvisaAdmin($"<strong>Una nuova richiesta:</strong><br/>{(_richiesta.TipoRichiesta?.IsTipoRichiestaDipendente == true ? "Prestazioni Regionali Dipendenti" : "Prestazioni Regionali Azienda")}<br/>{_richiesta.TipoRichiesta?.Descrizione} {_richiesta.TipoRichiesta?.Anno}");
                }

                if (_statoPraticaId == (int)SediinPraticheRegionaliEnums.StatoPratica.Confermata)
                {
                    ConfermaRichiestaMail(_richiesta);

                    AvvisaUtente(_richiesta.PraticheRegionaliImpreseId, "<strong>Informazione:</strong><br/>La sua Richiesta " + _richiesta.TipoRichiesta.Descrizione + " e stata confermata con Protocollo: " + _richiesta.ProtocolloId);
                }

                //cancella allegati eliminati
                Task.Run(() => DeleteFiles(_filesToDelete, praticheRegionaliImprese.PraticheRegionaliImpreseId));

                return JsonResultTrue(praticheRegionaliImprese.PraticheRegionaliImpreseId, "Richiesta " + (container.RichiestaId == 0 ? "inserita" : "aggiornata"));
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpPost]
        [Authorize(Roles = new Roles[] { Roles.Admin, Roles.Super })]
        public ActionResult RimettiInBozzaRichiesta(int richiestaId)
        {
            try
            {
                var _r = unitOfWork.PraticheRegionaliImpreseRepository.Get(xx => xx.PraticheRegionaliImpreseId == richiestaId).FirstOrDefault() ?? throw new Exception("Richiesta non trovata");
                
                    _r.StatoPraticaId = (int)SediinPraticheRegionaliEnums.StatoPratica.Bozza;
                    unitOfWork.Save(false);
                    return JsonResultTrue("Stato pratica aggiornato");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [HttpPost]
        [Authorize(Roles = new Roles[] { Roles.Admin, Roles.Super })]
        public ActionResult EliminaRichiesta(int richiestaId)
        {
            try
            {
                var _r = unitOfWork.PraticheRegionaliImpreseRepository.Get(xx => xx.PraticheRegionaliImpreseId == richiestaId).FirstOrDefault() ?? throw new Exception("Richiesta non trovata");

                if (_r.StatoPraticaId != (int)SediinPraticheRegionaliEnums.StatoPratica.Bozza)
                {
                    throw new Exception("La richiesta non po essere eliminata");
                }
                else if (_r.StatoPraticaId == (int)SediinPraticheRegionaliEnums.StatoPratica.Bozza)
                {
                    unitOfWork.PraticheRegionaliImpreseRepository.Delete(_r);
                    unitOfWork.Save(false);

                    return JsonResultTrue("Richiesta eliminata");
                }

                throw new Exception("Richiesta non valida");
            }
            catch (Exception)
            {
                throw;
            }
        }

        [ChildActionOnly]
        public ActionResult AnagraficaDipendente(int? dipendenteId, string iban, bool? ibanRequired, bool? readOnly)
        {
            var _dipendente = unitOfWork.DipendenteRepository.Get(x => x.DipendenteId == dipendenteId).FirstOrDefault();

            var model = Reflection.CreateModel<DipendentePrestazioniRegionaliViewModel>(unitOfWork.DipendenteRepository.Get(xx => xx.DipendenteId == dipendenteId).FirstOrDefault());

            if (!string.IsNullOrEmpty(iban))
            {
                model.Iban = iban;
            }

            model.Comunenascita = _dipendente.ComuneNascita.DENCOM;
            model.Indirizzoresidenza = _dipendente.Indirizzo;
            model.Comuneresidenza = _dipendente.Comune.DENCOM;
            model.Provinciaresidenza = _dipendente.Provincia.SIGPRO;
            model.Capresidenza = _dipendente.Localita.CAP;
            model.EMail = _dipendente.Email;
            model.Telefono = _dipendente.Cellulare;

            model.ReadOnly = readOnly;
            model.IbanRequired = ibanRequired.GetValueOrDefault();

            return AjaxView("AnagraficaDipendente", model);
        }

        [ChildActionOnly]
        public ActionResult AnagraficaAzienda(int? aziendaId, string iban, bool? ibanRequired, bool? ibanTitolareRequired, bool? readOnly)
        {
            var _azienda = unitOfWork.AziendaRepository.Get(xx => xx.AziendaId == aziendaId).FirstOrDefault();

            var model = Reflection.CreateModel<AziendaPrestazioniRegionaliViewModel>(_azienda);

            if (!string.IsNullOrEmpty(iban))
            {
                model.Iban = iban;
            }

            model.EMailazienda = _azienda.Email;
            if (!string.IsNullOrEmpty(model.EMailazienda))
            {
                model.EMailazienda = _azienda.Pec;
            }

            model.Indirizzoazienda = _azienda.Indirizzo;
            model.Comuneazienda = _azienda.Comune?.DENCOM;
            model.Provinciaazienda = _azienda.Provincia?.DENPRO;
            model.Capazienda = _azienda.Localita?.CAP;
            model.Telefonoazienda = _azienda.RappresentanteCellulare;
            model.Tipoattivita = _azienda.AttivitaEconomica;
            model.NomeTitolare = _azienda.NomeTitolare;
            model.CognomeTitolare = _azienda.CognomeTitolare;

            model.ReadOnly = readOnly;
            model.IbanRequired = ibanRequired.GetValueOrDefault();
            model.IbanTitolare = ibanTitolareRequired.GetValueOrDefault();
            model.AziendaCoperta = true;
            

            if (_azienda.Copertura == null || _azienda.Copertura.Count() == 0 || _azienda.Copertura.FirstOrDefault()?.Coperto == false)
            {
                model.AziendaCoperta = false;
            }

            return AjaxView("AnagraficaAzienda", model);
        }

        [ChildActionOnly]
        public ActionResult AllegatiRichiesta(int? tipoRichiestaId, int? richiestaId, bool? readOnly)
        {
            PraticheAziendaAllegati model = new PraticheAziendaAllegati();

            if (richiestaId.GetValueOrDefault() != 0)
            {
                var _richiesta = unitOfWork.PraticheRegionaliImpreseRepository.Get(xx => xx.PraticheRegionaliImpreseId == richiestaId).FirstOrDefault();
                model.RichiestaId = _richiesta.PraticheRegionaliImpreseId;
                model.RichiestaAllegati = _richiesta?.Allegati?.ToList();
                model.ReadOnly = readOnly;
            }

            model.TipoRichiestaAllegati = GetTipoRichiestaAllegati(tipoRichiestaId.Value);
            return PartialView("AllegatiRichiesta", model);
        }

        [ChildActionOnly]
        public ActionResult RequisitiRichiesta(int? tipoRichiestaId, int? richiestaId, bool? readOnly)
        {
            var _tiporichiesta = GetTipoRichieste().FirstOrDefault(x => x.TipoRichiestaId == tipoRichiestaId.GetValueOrDefault());

            PraticheAziendaRequisiti model = new PraticheAziendaRequisiti();
            model.RequisitiMinimo = _tiporichiesta.RequisitiMinimo.GetValueOrDefault();
            model.RequisitiMassimo = _tiporichiesta.RequisitiMassimo.GetValueOrDefault();

            if (richiestaId.GetValueOrDefault() != 0)
            {
                var _richiesta = unitOfWork.PraticheRegionaliImpreseRepository.Get(xx => xx.PraticheRegionaliImpreseId == richiestaId).FirstOrDefault();
                model.RichiestaId = _richiesta.PraticheRegionaliImpreseId;
                model.RequisitiSelected = _richiesta.Requisiti.Select(x => x.RequisitiId).ToList();
                model.ReadOnly = readOnly;
            }

            model.Requisiti = unitOfWork.TipoRichiestaRequisitiRepository.Get(x => x.TipoRichiestaId == tipoRichiestaId)?.ToList();
            return PartialView("RequisitiRichiesta", model);
        }

        [ChildActionOnly]
        public ActionResult DprRichiesta(int? tipoRichiestaId, int? richiestaId, bool? readOnly)
        {
            var _tiporichiesta = GetTipoRichieste().FirstOrDefault(x => x.TipoRichiestaId == tipoRichiestaId.GetValueOrDefault());

            PraticheAziendaDpr model = new PraticheAziendaDpr();
            model.DprMinimo = _tiporichiesta.DprMinimo.GetValueOrDefault();
            model.DprMassimo = _tiporichiesta.DprMassimo.GetValueOrDefault();

            if (richiestaId.GetValueOrDefault() != 0)
            {
                var _richiesta = unitOfWork.PraticheRegionaliImpreseRepository.Get(xx => xx.PraticheRegionaliImpreseId == richiestaId).FirstOrDefault();
                model.RichiestaId = _richiesta.PraticheRegionaliImpreseId;
                model.DprSelected = _richiesta.Dpr.Select(x => x.DichiarazioniDPRId).ToList();
                model.ReadOnly = readOnly;
            }

            model.Dpr = unitOfWork.TipoRichiestaDichirazioniDPRRepository.Get(x => x.TipoRichiestaId == tipoRichiestaId)?.ToList();
            return PartialView("DprRichiesta", model);
        }

        [ChildActionOnly]
        public ActionResult RichiedentiRichiesta(int? tipoRichiestaId, int? richiestaId, bool? readOnly)
        {
            var _tiporichiesta = GetTipoRichieste().FirstOrDefault(x => x.TipoRichiestaId == tipoRichiestaId.GetValueOrDefault());

            if (_tiporichiesta.RichiedenteMinimo.GetValueOrDefault() != 0
                || _tiporichiesta.RichiedenteMassimo.GetValueOrDefault() != 0)
            {
                PraticheAziendaRichidenti model = new PraticheAziendaRichidenti();
                model.ReadOnly = readOnly;
                model.RichiedenteMinimo = _tiporichiesta.RichiedenteMinimo;
                model.RichiedenteMassimo = _tiporichiesta.RichiedenteMassimo;
                model.Titolo = _tiporichiesta.RichiedenteTestoTitolo;

                model.PraticheRichiedenti = new List<PraticheRichidente>();

                if (richiestaId.GetValueOrDefault() != 0)
                {
                    var _richiesta = unitOfWork.PraticheRegionaliImpreseRepository.Get(xx => xx.PraticheRegionaliImpreseId == richiestaId).FirstOrDefault();
                    model.RichiestaId = _richiesta.PraticheRegionaliImpreseId;
                    model.PraticheRichiedenti = _richiesta?.Richidenti?.Select(x => new PraticheRichidente
                    {
                        CodiceFiscale = x.CodiceFiscale,
                        Nominativo = x.Nominativo
                    }).ToList();
                }

                return PartialView("RichiedentiRichiesta", model);
            }

            return null;
        }

        public ActionResult DownloadAllegato(int richiestaId, string allegato)
        {
            try
            {
                var _uploadFolder = GetUploadFolder(PathPraticheAzienda, richiestaId);

                var _allegato = unitOfWork.PraticheRegionaliImpreseAllegatiRepository.Get(xx => xx.PraticheRegionaliImpreseId == richiestaId && xx.Filename.StartsWith(allegato))?.FirstOrDefault();

                if (_allegato == null || !System.IO.File.Exists(Path.Combine(_uploadFolder, _allegato.Filename)))
                {
                    throw new Exception("Allegato non trovato");
                }

                var mimeType = System.Web.MimeMapping.GetMimeMapping(_allegato.Filename);
                return File(Path.Combine(_uploadFolder, _allegato.Filename), mimeType, _allegato.FilenameOriginale);
            }
            catch (Exception ex)
            {
                return Content(ex.Message);
            }
        }

        public string GetNoteTipoRichiesta(int? id = null)
        {
            if (id.GetValueOrDefault() == 0)
            {
                return null;
            }
            return GetTipoRichieste().FirstOrDefault(x => x.TipoRichiestaId == id)?.Note;
        }

        [ChildActionOnly]
        public ActionResult Azioni(int? richiestaId, int? tipoRichiestaId, int? statoId, bool? liquidataOinLiquidazione)
        {
            var _role = GetUserRole();
            PraticheAziendaAzioni model = new PraticheAziendaAzioni();
            model.TipoRichiestaId = tipoRichiestaId;
            model.RichiestaId = richiestaId;
            model.StatoId = statoId;
            model.Azioni = unitOfWork.AzioniPraticaRepository.Get(xx => xx.StatoPraticaId == statoId && xx.TipoRichiestaId == tipoRichiestaId);
            model.AzioniRuolo = unitOfWork.AzioniRuoloRepository.Get(xx => xx.Ruolo == _role && xx.StatoPraticaId == statoId);
            model.LiquidataOinLiquidazione = liquidataOinLiquidazione;

            return PartialView(model);
        }

        [Authorize(Roles = new Roles[] { Roles.Admin })]
        public ActionResult Revisione(int richiestaId)
        {
            PraticheAziendaRevisione_Annulla model = new PraticheAziendaRevisione_Annulla
            {
                RichiestaId = richiestaId,
                StatoPratica = SediinPraticheRegionaliEnums.StatoPratica.Revisione,
                Motivazioni = unitOfWork.MotivazioniRepository.Get(xx => xx.StatoPraticaId == (int)SediinPraticheRegionaliEnums.StatoPratica.Revisione)
            };

            return PartialView("RevisioneAnnulla", model);
        }

        public ActionResult Annulla(int richiestaId)
        {
            PraticheAziendaRevisione_Annulla model = new PraticheAziendaRevisione_Annulla
            {
                RichiestaId = richiestaId,
                StatoPratica = SediinPraticheRegionaliEnums.StatoPratica.Annullata,
                Motivazioni = unitOfWork.MotivazioniRepository.Get(xx => xx.StatoPraticaId == (int)SediinPraticheRegionaliEnums.StatoPratica.Annullata)
            };

            return PartialView("RevisioneAnnulla", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Revisione_Annulla(PraticheAziendaRevisione_Annulla model)
        {
            try
            {
                var _ragionesociale = Sediin.PraticheRegionali.Utils.ConfigurationProvider.Instance.GetConfiguration().RagioneSociale.Nome;
                var _richiesta = unitOfWork.PraticheRegionaliImpreseRepository.Get(xx => xx.PraticheRegionaliImpreseId == model.RichiestaId).FirstOrDefault();

                if (_richiesta.StatoPraticaId == (int)SediinPraticheRegionaliEnums.StatoPratica.Confermata)
                {
                    throw new Exception("Non è possibile cambiare lo stato della richiesta");
                }

                CheckUserAbilitatoRichiesta(_richiesta);

                //update stato richiesta
                _richiesta.StatoPraticaId = (int)Enum.Parse(typeof(SediinPraticheRegionaliEnums.StatoPratica), model.StatoPratica.ToString(), true);
                var _statoEnum = (SediinPraticheRegionaliEnums.StatoPratica)_richiesta.StatoPraticaId;

                unitOfWork.PraticheRegionaliImpreseRepository.Update(_richiesta);

                //inserimento in storico stato
                PraticheRegionaliImpreseStatoPraticaStorico praticheRegionaliImpreseStatoPratica = new PraticheRegionaliImpreseStatoPraticaStorico
                {
                    StatoPraticaId = _richiesta.StatoPraticaId,
                    DataInserimento = DateTime.Now,
                    PraticheRegionaliImpreseId = _richiesta.PraticheRegionaliImpreseId,
                    UserName = User.Identity.Name,
                    UserRuolo = GetUserRole(),
                    Note = model.Note,
                    MotivazioniId = model.MotivazioneId
                };

                unitOfWork.PraticheRegionaliImpreseStatoPraticaStoricoRepository.Insert(praticheRegionaliImpreseStatoPratica);

                unitOfWork.Save(false);

                var _template = "RichiestaAnnullata";

                var _t1 = "Richiesta annullata";
                var _t2 = _ragionesociale + " - Avviso Richiesta annullata";
                var _t3 = "Richiesta " + _richiesta.TipoRichiesta.Descrizione + " è stata annullata";

                if (_statoEnum == SediinPraticheRegionaliEnums.StatoPratica.Revisione)
                {
                    _template = "RichiestaInRevisione";
                    _t1 = "Richiesta in revisione";
                    _t2 = _ragionesociale + " - Avviso Richiesta in revisione";
                    _t3 = "Richiesta " + _richiesta.TipoRichiesta.Descrizione + " è in revisione, verificare la correttezza dei dati inseriti";
                }

                UpdateListRicerca(_richiesta.PraticheRegionaliImpreseId);

                if (User.IsInRole(IdentityHelper.Roles.Admin.ToString()) || User.IsInRole(IdentityHelper.Roles.Super.ToString()))
                {
                    //invia mail
                    var _email = GetEmailAddressFromRichiesta(_richiesta);
                    var _nome = GetNominativoFromRichiesta(_richiesta);
                    var motivazioni = unitOfWork.MotivazioniRepository.Get(xx => xx.MotivazioniId == model.MotivazioneId)?.FirstOrDefault();

                    var _body = RenderTemplate("PraticheAzienda/" + _template, new PraticheAziendaMail
                    {
                        Nominativo = _nome,
                        Descrizione = motivazioni?.Motivazione,
                        Note = praticheRegionaliImpreseStatoPratica.Note,
                        Protocollo = _richiesta.ProtocolloId,
                    });

                    AvvisoMail(_email, _nome, _t2, _body);
                    AvvisaUtente(_richiesta.PraticheRegionaliImpreseId, "<strong>Informazione:</strong><br/>" + _t3);
                }

                return JsonResultTrue(_t1);
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        [@Authorize(Roles = new Roles[] { Roles.Admin })]
        public ActionResult VisualizzaBudget()
        {
            return AjaxView("VisualizzaBudget", GetImportiRichiestaPerTipoRichiesta());
        }

        [Authorize(Roles = new Roles[] { Roles.Admin })]
        internal IEnumerable<VisualizzaBudgetViewModel> GetImportiRichiestaPerTipoRichiesta()
        {
            UnitOfWork _unitOfWork = new UnitOfWork();
            var _tipoRicieste = _unitOfWork.TipoRichiestaRepository.Get();

            var model = (from t in _tipoRicieste
                         select new VisualizzaBudgetViewModel
                         {
                             TipoRichiesta = t,
                             ImportoRichiestoRevisione = t.PraticheRegionaliImprese
                             .Where(x => x.StatoPraticaId == (int)SediinPraticheRegionaliEnums.StatoPratica.Revisione)
                             .Sum(x => x.ImportoContributoNetto),
                             ImportoRichiestoConfermato = t.PraticheRegionaliImprese
                             .Where(x => x.StatoPraticaId == (int)SediinPraticheRegionaliEnums.StatoPratica.Confermata)
                             .Sum(x => x.ImportoContributoNetto),
                             ImportoRichiestoBozza = t.PraticheRegionaliImprese
                             .Where(x => x.StatoPraticaId == (int)SediinPraticheRegionaliEnums.StatoPratica.Bozza)
                             .Sum(x => x.ImportoContributoNetto),
                             ImportoRichiesto = t.PraticheRegionaliImprese
                             .Where(x => x.StatoPraticaId == (int)SediinPraticheRegionaliEnums.StatoPratica.Inviata
                             || x.StatoPraticaId == (int)SediinPraticheRegionaliEnums.StatoPratica.InviataRevisionata
                             ).Sum(x => x.ImportoContributoNetto)
                         });

            return model;

        }

        [HttpPost]
        public ActionResult ModificaDataInvio(int id, string dataInvio)
        {
            try
            {
                var _p = unitOfWork.PraticheRegionaliImpreseRepository.Get(x => x.PraticheRegionaliImpreseId == id).FirstOrDefault();
                if (_p == null)
                {
                    return JsonResultFalse("Impossibile modificare");
                }
                else
                {
                    _p.DataInvio = null;
                    if (!string.IsNullOrWhiteSpace(dataInvio))
                    {
                        _p.DataInvio = DateTime.Parse(dataInvio);
                    }

                    unitOfWork.Save();
                }
                return JsonResultTrue("Data Invio modificata correttamente");
            }
            catch (Exception ex)
            {
                return JsonResultFalse("Impossibile modificare: " + ex.Message);
            }
        }

        #region richiesta calcoli e controlli

        [HttpPost]
        public ActionResult CalcolaImportoRimborsatoContributo(int tipoRichiestaId, string importo, string percentuale)
        {
            try
            {

                decimal.TryParse(importo, out decimal _importo);

                decimal? _percentuale = null;

                if (!string.IsNullOrWhiteSpace(percentuale))
                {
                    _percentuale = Convert.ToDecimal(percentuale);
                }

                var _importiCalcolati = PraticheAziendaUtility.CalcolaImportoRimborsatoContributo(tipoRichiestaId, _importo, _percentuale);

                var _htmlCalcoli = PartialView("~/Areas/Backend/Views/Pratiche/ImportoCalcolati.cshtml").RenderViewToString(_importiCalcolati);

                return Json(new { html = _htmlCalcoli, importiCalcolati = _importiCalcolati }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult VerificaTipoRichiestaUnivocoCodiceFiscale(int aziendaId, int tipoRichiestaId, string codiceFiscale, int richiestaId, string nomeCampo, bool unica)
        {
            var _exists = PraticheAziendaUtility.VerificaTipoRichiestaUnivocoCodiceFiscale(aziendaId, tipoRichiestaId, codiceFiscale, richiestaId, nomeCampo, unica);
            return Json(new { isValid = !_exists, message = _exists ? "Per il Codice Fiscale e già stata presentata una richiesta" : "" }, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region richiesta helper

        private ICollection<PraticheRegionaliImpreseDatiPratica> CreateDatiPratica<T>(T o, ref decimal? _importoDaRimborsare)//, ref bool? hashImportoColumn)
        {
            try
            {
                List<PraticheRegionaliImpreseDatiPratica> _list = new List<PraticheRegionaliImpreseDatiPratica>();

                if (o == null)
                {
                    return _list;
                }

                foreach (var item in o.GetType().GetProperties())
                {
                    try
                    {
                        var _excludet = false;
                        foreach (var _attributes in item.GetCustomAttributes(true))
                        {
                            if (_attributes is NotMappedAttribute)
                            {
                                _excludet = true;
                                break;
                            }
                        }

                        foreach (var _attributes in item.GetCustomAttributes(true))
                        {
                            if (_attributes is PraticheAzienda_ImportoContributoRichiestoAttribute)
                            {
                                //hashImportoColumn = true;
                                if (o != null && item.GetValue(o) != null)
                                {

                                    decimal.TryParse(item.GetValue(o).ToString(), out decimal a);
                                    if (!a.Equals(decimal.Zero))
                                    {
                                        if (!_importoDaRimborsare.HasValue)
                                        {
                                            _importoDaRimborsare = 0;
                                        }
                                        _importoDaRimborsare += a;
                                    }
                                }
                            }
                        }

                        if (_excludet)
                        {
                            continue;
                        }

                        _list.Add(new PraticheRegionaliImpreseDatiPratica
                        {
                            Nome = item.Name,
                            Valore = o != null && item.GetValue(o) != null ? item.GetValue(o).ToString() : null
                        });
                    }
                    catch
                    {
                    }
                }

                return _list;
            }
            catch (Exception)
            {
                throw;
            }
        }

        private ICollection<PraticheRegionaliImpreseAllegati> CreateAllegati(PraticheAziendaAllegati upload, int praticheRegionaliImpreseId)
        {
            try
            {
                List<PraticheRegionaliImpreseAllegati> _list = new List<PraticheRegionaliImpreseAllegati>();

                if (upload == null || upload?.File == null || upload?.File?.Count() == 0)
                {
                    return _list;
                }

                var cartellaServer = GetUploadFolder(PathPraticheAzienda, praticheRegionaliImpreseId);

                foreach (var item in upload.File)
                {
                    if (item.PraticheRegionaliImpreseAllegatiId.GetValueOrDefault() != 0)
                    {
                        continue;
                    }

                    var filename = Savefile(cartellaServer, item.Base64);

                    _list.Add(new PraticheRegionaliImpreseAllegati
                    {
                        Filename = filename,
                        FilenameOriginale = item.NomeFile,
                        TipoRichiestaAllegatiId = item.TipoRichiestaAllegatiId,
                        PraticheRegionaliImpreseId = praticheRegionaliImpreseId,
                    });
                }

                return _list;
            }
            catch (Exception)
            {
                throw;
            }

        }

        private object CreateModelDatiRichiestaChildClass(TipoRichiesta tiporichiesta, IEnumerable<PraticheRegionaliImpreseDatiPratica> datiRichiesta, int? childClassRowCount = 0)
        {
            if (childClassRowCount.GetValueOrDefault() == 0)
            {
                return null;
            }

            IList _listDatiPraticaEventi = CreateInstanceList(tiporichiesta.ChildClass);

            for (int i = 0; i < childClassRowCount; i++)
            {
                var model = CreateInstance(tiporichiesta.ChildClass);

                foreach (var item in model.GetType().GetProperties())
                {
                    var dati = datiRichiesta.FirstOrDefault(x => x.Nome == "ChildClass" + "[" + i + "]." + item.Name);

                    Reflection.SetValue(model, dati?.Nome.Replace("ChildClass" + "[" + i + "].", ""), dati?.Valore);
                }

                _listDatiPraticaEventi.Add(model);
            }

            return _listDatiPraticaEventi;
        }

        private object CreateModelDatiRichiesta(TipoRichiesta tiporichiesta, IEnumerable<PraticheRegionaliImpreseDatiPratica> datiRichiesta)
        {
            var model = CreateInstance(tiporichiesta.Classe);

            foreach (var item in model.GetType().GetProperties())
            {

                //if (typeof(T).Name != item.DeclaringType.Name)
                //    continue;


                var dati = datiRichiesta.FirstOrDefault(x => x.Nome == item?.Name);

                Reflection.SetValue(model, dati?.Nome, dati?.Valore);
            }

            return model;
        }

        private IList CreateInstanceList(string classe)
        {
            if (string.IsNullOrWhiteSpace(classe))
            {
                return default;
            }

            var model = CreateInstance(classe);

            // Create a list of the required type and cast to IList
            Type genericListType = typeof(List<>);
            Type concreteListType = genericListType.MakeGenericType(model.GetType());
            IList _list = Activator.CreateInstance(concreteListType) as IList;

            return _list;
        }

        private object CreateInstance(string classe)
        {
            //var x =  typeof(PraticheAzienda_IncrementoMantenimentoOccupazione.Richiedente).AssemblyQualifiedName;
            if (string.IsNullOrWhiteSpace(classe))
            {
                return default;
            }

            Type t = Type.GetType(classe);
            return Activator.CreateInstance(t);
        }

        private void SetProvicia(object model, string propId, string prop)
        {
            try
            {
                var _id = model.GetType().GetProperty(propId)?.GetValue(model);

                if (_id != null)
                {
                    var _p = ParseInt(_id);

                    var _o = unitOfWork.ProvinceRepository.Get(xx => xx.ProvinciaId == _p).FirstOrDefault();

                    model.GetType().GetProperty(prop)?.SetValue(model, _o);

                }
            }
            catch
            {

            }
        }

        private void SetComune(object model, string propId, string prop)
        {
            try
            {
                var _id = model.GetType().GetProperty(propId)?.GetValue(model);

                if (_id != null)
                {
                    var _p = ParseInt(_id);

                    var _o = unitOfWork.ComuniRepository.Get(xx => xx.ComuneId == _p).FirstOrDefault();

                    model.GetType().GetProperty(prop)?.SetValue(model, _o);

                }
            }
            catch
            {

            }
        }

        private void SetRegione(object model, string propId, string prop)
        {
            try
            {
                var _id = model.GetType().GetProperty(propId)?.GetValue(model);

                if (_id != null)
                {
                    var _p = ParseInt(_id);

                    var _o = unitOfWork.RegioniRepository.Get(xx => xx.RegioneId == _p).FirstOrDefault();

                    model.GetType().GetProperty(prop)?.SetValue(model, _o);

                }
            }
            catch
            {

            }
        }

        private void SetLocalita(object model, string propId, string prop)
        {
            try
            {
                var _id = model.GetType().GetProperty(propId)?.GetValue(model);

                if (_id != null)
                {
                    var _p = ParseInt(_id);

                    var _o = unitOfWork.LocalitaRepository.Get(xx => xx.LocalitaId == _p).FirstOrDefault();

                    model.GetType().GetProperty(prop)?.SetValue(model, _o);
                }
            }
            catch
            {

            }
        }

        private void SetProperty(object model, string prop, object value)
        {
            try
            {
                model.GetType().GetProperty(prop)?.SetValue(model, value);
            }
            catch
            {

            }
        }

        private void DeleteFiles(List<string> filesToDelete, int praticheRegionaliImpreseId)
        {
            var cartellaServer = GetUploadFolder(PathPraticheAzienda, praticheRegionaliImpreseId);

            foreach (var filename in filesToDelete)
            {
                try
                {
                    System.IO.File.Delete(System.IO.Path.Combine(cartellaServer, filename));
                }
                catch
                {
                }
            }
        }

        private List<TipoRichiestaAllegati> GetTipoRichiestaAllegati(int id)
        {
            return unitOfWork.TipoRichiestaAllegatiRepository.Get(xx => xx.TipoRichiestaId == id).ToList();
        }

        private List<StatoPratica> GetStatoPratica()
        {
            return unitOfWork.StatoPraticaRepository.Get().ToList();
        }

        private List<StatoLiquidazione> GetStatoLiquidazione()
        {
            var _l = unitOfWork.StatoLiquidazioneRepository.Get(x => x.StatoLiquidazioneId != (int)SediinPraticheRegionaliEnums.StatoLiqidazione.Annullata).ToList();

            _l.Add(new StatoLiquidazione { Descrizione = "Da liquidare", StatoLiquidazioneId = 0, Ordine = 3 });

            return _l;

        }

        public List<TipoRichiesta> GetTipoRichieste()
        {
            using (var _unitofwork = new UnitOfWork())
            {
                var result = new List<TipoRichiesta>();

                if (IsInRole(new Roles[] { Roles.Azienda }))
                {
                    result = _unitofwork.TipoRichiestaRepository.Get(x => x.IsTipoRichiestaDipendente != true).OrderBy(o => o.Descrizione).ToList();
                }
                else if (IsInRole(new Roles[] { Roles.Dipendente, Roles.Sp_CAF }))
                {
                    result = _unitofwork.TipoRichiestaRepository.Get(x => x.IsTipoRichiestaDipendente == true).OrderBy(o => o.Descrizione).ToList();
                }
                else
                {
                    result = _unitofwork.TipoRichiestaRepository.Get().OrderBy(o => o.Descrizione).ToList();
                }

                if (result != null)
                {
                    foreach (var item in result.OrderBy(o => o.Descrizione).ToList())
                    {
                        item.Descrizione = item.Descrizione + " - " + item.Modulo + " (" + item.Anno + ")";
                    }
                }

                return result.ToList();
            }
        }

        public void VerificaCoperturaAzienda(int aziendaId)
        {
            try
            {
                using (var _unitofwork = new UnitOfWork())
                {
                    var _az = _unitofwork.CoperturaRepository.Get(x => x.AziendaId == aziendaId).FirstOrDefault();
                    if (_az == null || _az.Coperto == false)
                    {
                        throw new Exception("L'azienda non risulta in regola con i contributi");
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private void CheckUserAbilitatoRichiesta(PraticheRegionaliImprese richiesta)
        {
            if (richiesta != null)
            {
                if (IsInRole(new Roles[] { Roles.Azienda }))
                {
                    if (richiesta.AziendaId != GetAziendaId || richiesta.DipendenteId != null)
                    {
                        throw new Exception("Azienda non è abilitata a visualizze la richiesta");
                    }
                }

                if (IsInRole(new Roles[] { Roles.Dipendente }))
                {
                    if (richiesta.DipendenteId != GetDipendenteId)
                    {
                        throw new Exception("Dipendente non abilitato a visualizzare la richiesta");
                    }
                }

                if (IsInRole(new Roles[] { Roles.Sp_CAF, Roles.Sp_Consulente, Roles.Sp_Datoriale, Roles.Sp_Sindacale, Roles.Sp_Ebinter }))
                {
                    if (richiesta.SportelloId != GetSportelloId)
                    {
                        throw new Exception("Dipendente non abilitato a visualizzare la richiesta");
                    }
                }
            }
        }

        public string GetNominativoFromRichiesta(PraticheRegionaliImprese richiesta)
        {
            var _nome = "";

            if (richiesta.SportelloId.HasValue)
            {
                _nome = richiesta.Sportello?.Nome + " " + richiesta.Sportello?.Cognome;
            }
            if (richiesta.DipendenteId.HasValue && !richiesta.SportelloId.HasValue)
            {
                _nome = richiesta.Dipendente?.Nome + " " + richiesta.Dipendente?.Cognome;
            }
            if (!richiesta.DipendenteId.HasValue && !richiesta.SportelloId.HasValue)
            {
                _nome = richiesta.Azienda?.RagioneSociale;
            }

            return _nome;
        }

        public string GetEmailAddressFromRichiesta(PraticheRegionaliImprese richiesta)
        {
            var _email = "";

            if (richiesta.SportelloId.HasValue)
            {
                _email = richiesta.Sportello?.Email;
            }
            if (richiesta.DipendenteId.HasValue && !richiesta.SportelloId.HasValue)
            {
                _email = richiesta.Dipendente?.Email;
            }
            if (!richiesta.DipendenteId.HasValue && !richiesta.SportelloId.HasValue)
            {
                _email = richiesta.Azienda?.Email;
            }

            return _email;
        }

        private void AvvisoMail(string email, string nome, string subject, string body)
        {
            try
            {
                Task.Run(() =>
                {
                    //Invia una mail che la richiesta e in revisione
                    SendMailAsync(new WebUI.Models.SimpleMailMessage
                    {
                        ToEmail = email,
                        ToName = nome,
                        Subject = subject,
                        Body = body
                    });
                });
            }
            catch
            {

            }
        }

        private void UpdateListRicerca(int richiestaId)
        {
            try
            {
                var richiesta = unitOfWork.PraticheRegionaliImpreseRepository.Get(x => x.PraticheRegionaliImpreseId == richiestaId).FirstOrDefault();

                List<string> _usernames = new List<string>
                {
                    richiesta.UserInserimento
                };

                //update list ricerca
                IHubContext context = GlobalHost.ConnectionManager.GetHubContext<SediinPraticheRegionaliHub>();

                if (richiesta.DipendenteId == null && richiesta.Azienda != null)
                {
                    _usernames.Add(richiesta.Azienda?.MatricolaInps);
                }

                if (richiesta.DipendenteId == null && richiesta.SportelloId != null)
                {
                    _usernames.Add(richiesta.Sportello?.CodiceFiscalePIva);
                }

                if (richiesta.DipendenteId != null)
                {
                    _usernames.Add(richiesta.Dipendente?.CodiceFiscale);
                }

                foreach (var item in _usernames.Distinct())
                {
                    context.Clients.All.onUpdateListRicerca(item);
                }
            }
            catch
            {
            }
        }

        private void AvvisaAdmin(string message)
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<SediinPraticheRegionaliHub>();
            context.Clients.All.onAvvisaAdmin(message);
        }

        private void AvvisaUtente(int richiestaId, string message)
        {
            try
            {
                IHubContext context = GlobalHost.ConnectionManager.GetHubContext<SediinPraticheRegionaliHub>();

                var richiesta = unitOfWork.PraticheRegionaliImpreseRepository.Get(x => x.PraticheRegionaliImpreseId == richiestaId).FirstOrDefault();

                //List<string> _usernames = new List<string>
                //{
                //    //richiesta.UserInserimento?.ToUpper()
                //};

                var _username = "";

                if (richiesta.SportelloId.HasValue)
                {
                    _username = richiesta.Sportello?.CodiceFiscalePIva?.ToUpper();
                    //_usernames.Add(richiesta.Sportello?.CodiceFiscalePIva?.ToUpper());
                }
                if (richiesta.DipendenteId.HasValue && !richiesta.SportelloId.HasValue)
                {
                    _username = richiesta.Dipendente?.CodiceFiscale?.ToUpper();
                    //_usernames.Add(richiesta.Dipendente?.CodiceFiscale?.ToUpper());
                }
                if (!richiesta.DipendenteId.HasValue && !richiesta.SportelloId.HasValue)
                {
                    _username = richiesta.Azienda?.MatricolaInps?.ToUpper();
                    //_usernames.Add(richiesta.Azienda?.MatricolaInps?.ToUpper());
                }

                context.Clients.All.onAvvisoUtente(_username, message);

                //foreach (var item in _usernames.Distinct())
                //{
                //    context.Clients.All.onAvvisoUtente(item, message);
                //}
            }
            catch
            {
            }
        }

        public void ConfermaRichiestaMail(PraticheRegionaliImprese richiesta)
        {
            try
            {
                //invia mail
                var _email = GetEmailAddressFromRichiesta(richiesta);
                var _nome = GetNominativoFromRichiesta(richiesta);

                var _body = RenderTemplate("PraticheAzienda/RichiestaConfermata", new PraticheAziendaMail
                {
                    Nominativo = _nome,
                    Descrizione = richiesta.ProtocolloId,
                    Protocollo = richiesta.ProtocolloId
                });

                AvvisoMail(_email, _nome, Sediin.PraticheRegionali.Utils.ConfigurationProvider.Instance.GetConfiguration().RagioneSociale.Nome + " - Avviso Richiesta confermata", _body);
            }
            catch
            {
            }
        }

        #endregion

        #region helper

        internal JsonResult JsonResultTrue(int? richiestaId, string message)
        {

            return Json(new
            {
                richiestaId,
                isValid = true,
                message = message,
            }, JsonRequestBehavior.AllowGet);
        }

        internal JsonResult JsonResultFalse(int? richiestaId, string message)
        {
            return Json(new
            {
                richiestaId,
                isValid = false,
                message,
            }, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}