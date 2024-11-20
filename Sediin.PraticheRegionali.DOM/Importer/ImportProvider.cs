using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Sediin.PraticheRegionali.DOM.DAL;
using Sediin.PraticheRegionali.DOM.Entitys;
using Sediin.PraticheRegionali.DOM.Models;
using Newtonsoft.Json;
using Sediin.MVC.HtmlHelpers;
using Sediin.PraticheRegionali.DOM.Data;
using System.Runtime.InteropServices;

namespace Sediin.PraticheRegionali.DOM.Importer
{
    public delegate void OnImporterReport(string processoId, string username, string tipoImport, int index, int totale, string message);

    public delegate void OnErrorFileReport(string base64, string tipo, string username, string ruolo);

    public class ImportProvider
    {
        public enum ImportKey
        {
            Sp_CAF,
            Sp_Consulente,
            Sp_Datoriale,
            Sp_Sindacale,
            Sp_Ebac,
            Azienda,
            Dipendente,
            Uniemens,
            Coperture
        }

        public string Username { get; set; }

        public string Ruolo { get; set; }

        public int Anno { get; set; }

        public MemoryStream FileStream { get; set; }

        public string TipoImport { get; set; }

        public List<string> ErrorList { get; set; }

        class Metropolitane
        {
            public int LocalitaId { get; set; }
            public int ComuneId { get; set; }
            public int ProvinciaId { get; set; }
            public int RegioneId { get; set; }
        }

        class RootobjectAzienda
        {
            public string ragione_sociale { get; set; }
            public string matricola_inps { get; set; }
            public string partita_iva { get; set; }
            public string codice_fiscale { get; set; }
            public string csc { get; set; }
            public string classificazione { get; set; }
            public string attivita_economica { get; set; }
            public string codice_istat { get; set; }
            public string cognome_legale { get; set; }
            public string nome_legale { get; set; }
            public string indirizzo_legale { get; set; }
            public string comune_legale { get; set; }
            public string email_legale { get; set; }
            public string pec_legale { get; set; }
            public string indirizzo { get; set; }
            public string civico { get; set; }
            public string provincia { get; set; }
            public string comune { get; set; }
            public string cap { get; set; }
            public string email { get; set; }
            public string pec { get; set; }
            public string cognome_referente { get; set; }
            public string nome_referente { get; set; }
            public string email_referente { get; set; }
            public string pec_referente { get; set; }
            public string indirizzo_referente { get; set; }
            public string comune_referente { get; set; }
            public Data data_cessazione { get; set; }
            public Data data_iscrizione { get; set; }
            public string tipo { get; set; }
        }

        class RootobjectAzienda2 : RootobjectAzienda
        {
            public new DateTime? data_cessazione { get; set; }
            public new DateTime? data_iscrizione { get; set; }
        }

        class Data
        {
            public Data(DateTime? date)
            {
                this.date = date;
            }

            private DateTime? _date;
            public DateTime? date
            {
                get
                {
                    return _date;
                }
                set
                {
                    if (value == null)
                    {
                        _date = null;
                    }
                    else
                    {
                        DateTime.TryParse(value?.ToString(), out DateTime d);

                        _date = d;
                    }

                    if (_date == DateTime.MinValue)
                    {
                        _date = null;
                    }

                }
            }
        }

        class RootobjectSportello
        {
            public string CodiceFiscalePIva { get; set; }
            public string RagioneSociale { get; set; }
            public string _Indirizzo { get; set; }
            public string Cognome { get; set; }
            public string Nome { get; set; }
            public string Pec { get; set; }
            public string Email { get; set; }
            public string Telefono { get; set; }
            public string Cellulare { get; set; }
            public string Indirizzo { get; set; }
            public string Provincia { get; set; }
            public string Comune { get; set; }
            public string Localita { get; set; }
        }

        class RootobjectDipendente
        {
            public string matricola_inps { get; set; }
            public string CCNLCNEL { get; set; }
            public Data DataAssunzione { get; set; }
            public Data DataCessazione { get; set; }
            public string TempoImpiego { get; set; }
            public string TempoLavoro { get; set; }
            public string TipoContratto { get; set; }
            public string CodiceFiscale { get; set; }
            public string Nome { get; set; }
            public string Cognome { get; set; }
            public Data Datanascita { get; set; }
            public string Email { get; set; }
            public string Cellulare { get; set; }
            public string Iban { get; set; }
            public string Provincia { get; set; }
            public string Comune { get; set; }
            public string ProvinciaNascita { get; set; }
            public string ComuneNascita { get; set; }
            public string Indirizzo { get; set; }
        }

        class RootobjectDipendente2 : RootobjectDipendente
        {
            public new DateTime? Datanascita { get; set; }
            public new DateTime? DataAssunzione { get; set; }
            public new DateTime? DataCessazione { get; set; }

        }

        class RootobjectCoperture
        {
            public string matricola_inps { get; set; }
            public bool? copertura_fsba { get; set; }
        }

        public event OnImporterReport OnReport;

        public event OnErrorFileReport OnErrorFile;

        public void ProcessImport()
        {
            var _id = Guid.NewGuid().ToString();

            ImportKey _tipo = (ImportKey)Enum.Parse(typeof(ImportKey), TipoImport);

            try
            {
                List<string> _lines = new List<string>();
                using (StreamReader streamReader = new StreamReader(FileStream))
                {
                    string str;
                    while ((str = streamReader.ReadLine()) != null)
                    {
                        _lines.Add(str);
                    }
                }

                var _x = 0;

                var _totaleRighe = _lines.Count();

                if (_totaleRighe == 0)
                {
                    OnReport?.Invoke(_id, Username, TipoImport, 0, _totaleRighe, "Non ci sono righe da elaborare.");
                    return;
                }

                OnReport?.Invoke(_id, Username, TipoImport, 0, _totaleRighe, "Inizio processo, attendere...");

                var _taskList = new List<Task>();

                ErrorList = new List<string>();

                foreach (var item in _lines)
                {
                    _taskList.Add(Task.Run(() =>
                    {
                        try
                        {
                            var _mess = "";

                            switch (_tipo)
                            {
                                case ImportKey.Sp_CAF:
                                case ImportKey.Sp_Consulente:
                                case ImportKey.Sp_Datoriale:
                                case ImportKey.Sp_Sindacale:
                                case ImportKey.Sp_Ebac:
                                    _mess = InsertSportello(item, _tipo.ToString());
                                    break;
                                case ImportKey.Azienda:
                                    _mess = InsertAzienda(item);
                                    break;
                                case ImportKey.Dipendente:
                                    _mess = InsertDipendente(item);
                                    break;
                                case ImportKey.Uniemens:
                                    _mess = InsertUniemens(item, this.Anno);
                                    break;
                                case ImportKey.Coperture:
                                    _mess = InsertCoperture(item);
                                    break;

                                default:
                                    break;
                            }

                            OnReport?.Invoke(_id, Username, TipoImport, Interlocked.Increment(ref _x), _totaleRighe, _mess);
                        }
                        catch (DbEntityValidationException ex)
                        {
                            //report error
                            OnReport?.Invoke(_id, Username, TipoImport, Interlocked.Increment(ref _x), _totaleRighe, ex.Message);
                        }
                        catch (DbUpdateException ex)
                        {
                            //report error
                            OnReport?.Invoke(_id, Username, TipoImport, Interlocked.Increment(ref _x), _totaleRighe, ex.Message);
                        }
                        catch (Exception ex)
                        {
                            //report error
                            OnReport?.Invoke(_id, Username, TipoImport, Interlocked.Increment(ref _x), _totaleRighe, ex.Message);
                        }
                    }));
                }

                Task.WhenAll(_taskList).Wait();

                OnReport?.Invoke(_id, Username, TipoImport, _totaleRighe, _totaleRighe, "Processo terminato, " + TipoImport + " importate");

                if (ErrorList.Count() > 0)
                {
                    try
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.AppendLine(string.Join(Environment.NewLine, ErrorList));

                        var _base64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(sb.ToString()));
                        OnErrorFile?.Invoke(_base64, this.TipoImport, Username, Ruolo);
                    }
                    catch
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                OnReport?.Invoke(_id, Username, TipoImport, 0, 0, ex.Message);
            }
            finally
            {
                //bonifica aziende
                try
                {
                    switch (_tipo)
                    {
                        case ImportKey.Sp_CAF:
                        case ImportKey.Sp_Consulente:
                        case ImportKey.Sp_Datoriale:
                        case ImportKey.Sp_Sindacale:
                        case ImportKey.Sp_Ebac:
                            Task.Run(() => BonificaAnagraficaSportello());
                            break;
                        case ImportKey.Azienda:
                            Task.Run(() => BonificaAnagraficaAziende());
                            break;
                        case ImportKey.Dipendente:
                            Task.Run(() => BonificaAnagraficaDipendenti());
                            break;
                        case ImportKey.Uniemens:
                            break;
                        case ImportKey.Coperture:
                            break;
                        default:
                            break;
                    }
                }
                catch
                {
                }
            }
        }

        #region Coperture

        string InsertCoperture(string righa)
        {
            var _error = "";
            RootobjectCoperture _copertura = new RootobjectCoperture();
            try
            {
                UnitOfWork u = new UnitOfWork();

                var jitem = righa.Replace("$\"", "\"");

                _copertura = JsonConvert.DeserializeObject<RootobjectCoperture>(jitem);

                if (_copertura == null)
                {
                    _error = $"Righa non valida";
                    ErrorList.Add(_error);
                    return _error;
                }

                if (string.IsNullOrWhiteSpace(_copertura.matricola_inps))
                {
                    _error = $"Matricola Inps non valido {_copertura?.matricola_inps}";
                    ErrorList.Add(_error);
                    return _error;
                }

                var _matricola = _copertura.matricola_inps?.TrimAll();
                var _azienda = u.AziendaRepository.Get(x => x.MatricolaInps == _matricola).FirstOrDefault();
                if (_azienda == null)
                {
                    _error = $"Azienda non censita, matricola: {_copertura?.matricola_inps}";
                    ErrorList.Add(_error);
                    return _error;
                }

                var _messaggio = $"Copertura {_copertura.matricola_inps?.TrimAll()} inserita";
                int? _coperturaId = null;
                var _coperturaa = u.CoperturaRepository.Get(x => x.AziendaId == _azienda.AziendaId).FirstOrDefault();

                if (_coperturaa != null)
                {
                    _messaggio = $"Copertura {_copertura.matricola_inps?.TrimAll()} aggiornato";
                    _coperturaId = _coperturaa.AziendaId;
                }

                Copertura model = new Copertura()
                {
                    Coperto = _copertura.copertura_fsba.GetValueOrDefault(),
                    AziendaId = _azienda.AziendaId
                };

                if (_coperturaId.HasValue)
                {
                    model.CoperturaId = _coperturaId.Value;
                }

                u.CoperturaRepository.InsertOrUpdate(model);
                u.Save(false);

                return _messaggio;

            }
            catch (Exception ex)
            {
                _error = $"{ex?.Message} {ex?.InnerException?.InnerException?.Message} - Matricola: {_copertura?.matricola_inps}";
                ErrorList.Add(_error);
                return _error;
            }
        }

        #endregion

        #region dipendente

        string InsertDipendente(string righa)
        {
            var _error = "";
            RootobjectDipendente _dip = new RootobjectDipendente();
            try
            {
                UnitOfWork u = new UnitOfWork();

                var jitem = righa.Replace("$\"", "");

                RootobjectDipendente parseModel()
                {
                    try
                    {
                        _dip = JsonConvert.DeserializeObject<RootobjectDipendente>(jitem);

                    }
                    catch (Exception)
                    {
                        try
                        {
                            var __az = JsonConvert.DeserializeObject<RootobjectDipendente2>(jitem);

                            foreach (var item in typeof(RootobjectDipendente2).GetProperties())
                            {
                                Reflection.SetValue(_dip, item.Name, item.GetValue(__az));
                            }

                            _dip.Datanascita = new Data(__az.Datanascita);
                            _dip.DataAssunzione = new Data(__az.DataAssunzione);
                            _dip.DataCessazione = new Data(__az.DataCessazione);

                        }
                        catch (Exception)
                        {

                        }
                    }

                    return _dip;
                }

                _dip = parseModel();

                if (_dip == null)
                {
                    //_x++;
                    _error = $"Righa non valida";
                    ErrorList.Add(_error);
                    return _error;
                }

                if (string.IsNullOrWhiteSpace(_dip.matricola_inps))
                {
                    _error = $"Codice fiscale non valido ({_dip?.Nome} {_dip?.Cognome} - {_dip?.CodiceFiscale})";
                    ErrorList.Add(_error);
                    return _error;
                }

                var _messaggio = $"Dipendente {_dip?.Nome?.TrimAll()} {_dip?.Cognome?.TrimAll()} inserito";
                int? _dipendenteId = null;
                var _dipendente = u.DipendenteRepository.Get(x => x.CodiceFiscale.ToUpper() == _dip.CodiceFiscale.ToUpper()).FirstOrDefault();

                if (_dipendente != null)
                {
                    _messaggio = $"Dipendente {_dip?.Nome?.TrimAll()} {_dip?.Cognome?.TrimAll()} aggiornato";
                    _dipendenteId = _dipendente.DipendenteId;
                }

                var _prov = u.ProvinceRepository.Get(x => x.SIGPRO == _dip.Provincia)?.FirstOrDefault();
                var _provnascita = u.ProvinceRepository.Get(x => x.SIGPRO == _dip.ProvinciaNascita)?.FirstOrDefault();

                var _comune = u.ComuniRepository.Get(x => x.CODCOM == _dip.Comune)?.FirstOrDefault();
                var _comunenascita = u.ComuniRepository.Get(x => x.CODCOM == _dip.ComuneNascita)?.FirstOrDefault();

                int? _regioneId = null;
                int? _regionenascitaId = null;

                if (_prov != null)
                {
                    var _regione = u.RegioniRepository.Get(x => x.CODREG == _prov.CODREG)?.FirstOrDefault();
                    _regioneId = _regione?.RegioneId;
                }

                if (_provnascita != null)
                {
                    var _regionenascita = u.RegioniRepository.Get(x => x.CODREG == _provnascita.CODREG)?.FirstOrDefault();
                    _regionenascitaId = _regionenascita?.RegioneId;
                }

                Dipendente model = new Dipendente()
                {

                    CodiceFiscale = _dip.CodiceFiscale?.TrimAll(),
                    ComuneId = _comune?.ComuneId,
                    Datanascita = _dip.Datanascita?.date,
                    Email = _dip.Email?.TrimAll(),
                    Iban = _dip.Iban?.TrimAll(),
                    Indirizzo = _dip.Indirizzo?.TrimAll(),
                    LocalitaId = null,
                    ProvinciaId = _prov?.ProvinciaId,
                    RegioneId = _regioneId,
                    Cellulare = _dip.Cellulare?.TrimAll(),
                    Cognome = _dip.Cognome?.TrimAll(),
                    Nome = _dip.Nome?.TrimAll(),
                    ComuneNascitaId = null,
                    ProvinciaNascitaId = _provnascita?.ProvinciaId,
                    RegioneNascitaId = _regionenascitaId,

                };

                if (_dipendenteId.HasValue)
                {
                    model.DipendenteId = _dipendenteId.Value;
                    //u.DipendenteRepository.Update(model);
                }
                //else
                //{
                //    u.DipendenteRepository.Insert(model);
                //}
                u.DipendenteRepository.InsertOrUpdate(model);
                u.Save(false);

                var _az = u.AziendaRepository.Get(x => x.MatricolaInps == _dip.matricola_inps).FirstOrDefault();

                if (_az != null)
                {
#pragma warning disable CS0168 // La variabile è dichiarata, ma non viene mai usata
                    try
                    {
                        var _dipendenteAzienda = u.DipendenteAziendaRepository.Get(x => x.DipendenteId == model.DipendenteId && x.AziendaId == _az.AziendaId).FirstOrDefault();

                        int? _dipendenteAziendaId = null;

                        if (_dipendenteAzienda != null)
                        {
                            _dipendenteAziendaId = _dipendenteAzienda.DipendenteAziendaId;
                        }

                        var _TempoLavoroId = u.TempoLavoroRepository.Get(x => x.TempoPieno == true)?.FirstOrDefault()?.TempoLavoroId;
                        var _TipoContrattoId = u.TipoContrattoRepository.Get(x => x.Descrizione.Contains(_dip.TipoContratto))?.FirstOrDefault()?.TipoContrattoId;
                        var _TipoImpiegoId = u.TipoImpiegoRepository.Get(x => x.Descrizione.Contains(_dip.TempoImpiego))?.FirstOrDefault()?.TipoImpiegoId;

                        var _model = new DipendenteAzienda()
                        {
                            AziendaId = _az.AziendaId,
                            DipendenteId = model.DipendenteId,
                            CCNLCNEL = _dip.CCNLCNEL?.TrimAll(),
                            DataAssunzione = _dip.DataAssunzione?.date != null ? _dip.DataAssunzione?.date.GetValueOrDefault() : null,
                            DataCessazione = _dip.DataCessazione?.date != null ? _dip.DataCessazione?.date.GetValueOrDefault() : null,
                            TempoLavoroId = _TempoLavoroId,

                            TipoContrattoId = _TipoContrattoId,
                            TipoImpiegoId = _TipoImpiegoId,
                            DocumentoAltro = null,
                            DocumentoIdentita = null,
                        };

                        if (_dipendenteAziendaId.HasValue)
                        {
                            _model.DipendenteAziendaId = _dipendenteAziendaId.Value;
                            //u.DipendenteAziendaRepository.Update(_model);
                        }
                        //else
                        //{
                        //    u.DipendenteAziendaRepository.Insert(_model);
                        //}
                        u.DipendenteAziendaRepository.InsertOrUpdate(_model);
                        u.Save(false);

                    }
                    catch (Exception ex)
                    {
                    }
#pragma warning restore CS0168 // La variabile è dichiarata, ma non viene mai usata

                }


                //_x++;
                return _messaggio;

            }
            catch (Exception ex)
            {
                //_x++;
                _error = $"{ex?.Message} {ex?.InnerException?.InnerException?.Message} - ({_dip?.Nome} {_dip?.Cognome} - {_dip?.CodiceFiscale})";
                ErrorList.Add(_error);
                return _error;
            }
        }

        #endregion

        #region aziende

        string InsertAzienda(string righa)
        {
            var _error = "";
            RootobjectAzienda _az = new RootobjectAzienda();
            try
            {
                UnitOfWork u = new UnitOfWork();

                Metropolitane getMetropolitane(string cap, string comune)
                {
                    try
                    {
                        var _localita = u.LocalitaRepository.Get(x => x.CAP == cap && x.DENLOC == comune)?.FirstOrDefault();

                        var _comune = u.ComuniRepository.Get(x => x.CODCOM == _localita.CODCOM)?.FirstOrDefault();

                        var _prov = u.ProvinceRepository.Get(x => x.SIGPRO == _comune.SIGPRO)?.FirstOrDefault();

                        var _regione = u.RegioniRepository.Get(x => x.CODREG == _prov.CODREG).FirstOrDefault();

                        return new Metropolitane
                        {
                            LocalitaId = _localita.LocalitaId,
                            ComuneId = _comune.ComuneId,
                            ProvinciaId = _prov.ProvinciaId,
                            RegioneId = _regione.RegioneId
                        };
                    }
                    catch (Exception)
                    {
                        return default;
                    }
                };

                int? getTipologia(string tipo)
                {
                    try
                    {
                        return u.TipologiaRepository.Get(x => x.Descrizione.Contains(tipo))?.FirstOrDefault()?.TipologiaId;
                    }
                    catch
                    {
                        return null;
                    }
                };

                var jitem = righa.Replace("$\"", "\"");


                RootobjectAzienda parseModel()
                {
                    try
                    {
                        _az = JsonConvert.DeserializeObject<RootobjectAzienda>(jitem);

                    }
                    catch (Exception)
                    {
                        var __az = JsonConvert.DeserializeObject<RootobjectAzienda2>(jitem);

                        foreach (var item in typeof(RootobjectAzienda2).GetProperties())
                        {
                            Reflection.SetValue(_az, item.Name, item.GetValue(__az));
                        }

                        _az.data_cessazione = new Data(__az.data_cessazione);
                        _az.data_iscrizione = new Data(__az.data_iscrizione);
                    }

                    return _az;
                }

                _az = parseModel();

                if (_az == null)
                {
                    //_x++;
                    _error = $"Righa non valida";
                    ErrorList.Add(_error);
                    return _error;
                }

                if (string.IsNullOrWhiteSpace(_az.matricola_inps))
                {
                    _error = $"Matricola Inps non valida ({_az?.ragione_sociale?.TrimAll()} - {_az?.partita_iva?.TrimAll()})";
                    ErrorList.Add(_error);
                    return _error;
                }

                var _messaggio = $"Azienda {_az.ragione_sociale?.TrimAll()} inserita";
                int? _aziendaId = null;
                var _azienda = u.AziendaRepository.Get(x => x.MatricolaInps == _az.matricola_inps).FirstOrDefault();

                if (_azienda != null)
                {
                    _messaggio = $"Azienda {_az.ragione_sociale?.TrimAll()} aggiornata";
                    _aziendaId = _azienda.AziendaId;
                }

                Metropolitane metropolitaneAzienda = getMetropolitane(_az.cap, _az.comune);

                Azienda model = new Azienda()
                {
                    AttivitaEconomica = _az.attivita_economica?.TrimAll(),
                    Classificazione = _az.classificazione?.TrimAll(),
                    CodiceFiscale = _az.codice_fiscale?.TrimAll(),
                    CodiceIstat = _az.codice_istat?.TrimAll(),
                    CognomeTitolare = null,
                    ComuneId = metropolitaneAzienda?.ComuneId,
                    CSC = _az.csc,
                    DataCessazione = _az.data_cessazione?.date,
                    DataIscrizione = _az.data_iscrizione == null ? DateTime.Now : _az.data_iscrizione.date.GetValueOrDefault(),
                    Email = _az.email?.TrimAll(),
                    Iban = null,
                    Indirizzo = _az.indirizzo?.TrimAll(),
                    LocalitaId = metropolitaneAzienda?.LocalitaId,
                    MatricolaInps = _az.matricola_inps?.TrimAll(),
                    NomeTitolare = null,
                    Partesociale = null,
                    PartitaIva = _az.partita_iva?.TrimAll(),
                    Pec = _az.pec,
                    ProvinciaId = metropolitaneAzienda?.ProvinciaId,
                    RappresentanteCellulare = null,
                    RappresentanteCognome = _az.cognome_legale?.TrimAll(),
                    RappresentanteComuneId = null,
                    RappresentanteEmail = _az.email_legale?.TrimAll(),
                    RappresentanteIndirizzo = _az.indirizzo_legale?.TrimAll(),
                    RappresentanteLocalitaId = null,
                    RappresentanteNome = _az.nome_legale?.TrimAll(),
                    RappresentantePec = _az.pec_legale?.TrimAll(),
                    RappresentanteProvinciaId = null,
                    RappresentanteRegioneId = null,
                    ReferenteCellulare = null,
                    ReferenteCognome = _az.cognome_referente?.TrimAll(),
                    ReferenteComuneId = null,
                    ReferenteEmail = _az.email_referente?.TrimAll(),
                    ReferenteIndirizzo = _az.indirizzo_referente?.TrimAll(),
                    ReferenteLocalitaId = null,
                    ReferenteNome = _az.nome_referente?.TrimAll(),
                    ReferenteProvinciaId = null,
                    ReferentePec = _az.pec_referente?.TrimAll(),
                    ReferenteRegioneId = null,
                    RegioneId = metropolitaneAzienda?.RegioneId,
                    TipologiaId = getTipologia(_az.tipo?.TrimAll()),
                    RagioneSociale = _az.ragione_sociale?.TrimAll()
                };


                if (_aziendaId.HasValue)
                {
                    model.AziendaId = _aziendaId.Value;
                    //u.AziendaRepository.Update(model);
                }
                //else
                //{
                //    u.AziendaRepository.Insert(model);
                //}

                u.AziendaRepository.InsertOrUpdate(model);
                u.Save(false);

                //_x++;
                return _messaggio;

            }
            catch (Exception ex)
            {
                //_x++;
                _error = $"{ex?.Message} {ex?.InnerException?.InnerException?.Message} - ({_az?.ragione_sociale} - {_az?.partita_iva})";
                ErrorList.Add(_error);
                return _error;
            }
        }

        #endregion

        #region sportello

        string InsertSportello(string righa, string tipoConsulente)
        {
            var _error = "";
            RootobjectSportello _az = new RootobjectSportello();
            try
            {
                UnitOfWork u = new UnitOfWork();

                Metropolitane getMetropolitane(string cap, string comune)
                {
                    try
                    {
                        var _localita = u.LocalitaRepository.Get(x => x.CAP == cap && x.DENLOC == comune)?.FirstOrDefault();

                        var _comune = u.ComuniRepository.Get(x => x.CODCOM == _localita.CODCOM)?.FirstOrDefault();

                        var _prov = u.ProvinceRepository.Get(x => x.SIGPRO == _comune.SIGPRO)?.FirstOrDefault();

                        var _regione = u.RegioniRepository.Get(x => x.CODREG == _prov.CODREG).FirstOrDefault();

                        return new Metropolitane
                        {
                            LocalitaId = _localita.LocalitaId,
                            ComuneId = _comune.ComuneId,
                            ProvinciaId = _prov.ProvinciaId,
                            RegioneId = _regione.RegioneId
                        };
                    }
                    catch (Exception)
                    {
                        return default;
                    }
                };

                var jitem = righa.Replace("$\"", "\"");

                _az = JsonConvert.DeserializeObject<RootobjectSportello>(jitem);

                if (_az == null)
                {
                    //_x++;
                    _error = $"Righa non valida";
                    ErrorList.Add(_error);
                    return _error;
                }

                if (string.IsNullOrWhiteSpace(_az.CodiceFiscalePIva))
                {
                    _error = $"Codice Fiscale non valido ({_az?.RagioneSociale} - {_az?.CodiceFiscalePIva})";
                    ErrorList.Add(_error);
                    return _error;
                }

                var _messaggio = $"Sportello {_az.RagioneSociale?.TrimAll()} inserito";
                int? _sportelloId = null;
                var _sportello = u.SportelloRepository.Get(x => x.CodiceFiscalePIva == _az.CodiceFiscalePIva).FirstOrDefault();

                if (_sportello != null)
                {
                    _messaggio = $"Sportello {_az.RagioneSociale?.TrimAll()} aggiornato";
                    _sportelloId = _sportello.SportelloId;
                }

                Metropolitane metropolitaneAzienda = getMetropolitane(_az.Localita, _az.Comune);

                Sportello model = new Sportello()
                {
                    ComuneId = metropolitaneAzienda?.ComuneId,
                    Email = _az.Email?.TrimAll(),
                    Indirizzo = _az._Indirizzo?.TrimAll(),
                    LocalitaId = metropolitaneAzienda?.LocalitaId,
                    Pec = _az.Pec?.TrimAll(),
                    ProvinciaId = metropolitaneAzienda?.ProvinciaId,

                    RegioneId = metropolitaneAzienda?.RegioneId,
                    RagioneSociale = _az.RagioneSociale?.TrimAll(),
                    Cellulare = _az.Cellulare?.TrimAll(),
                    CodiceFiscalePIva = _az.CodiceFiscalePIva?.TrimAll(),
                    Cognome = _az.Cognome?.TrimAll(),
                    Nome = _az.Nome?.TrimAll(),
                    Telefono = _az.Telefono?.TrimAll(),
                    Ruolo = tipoConsulente
                };

                if (_sportelloId.HasValue)
                {
                    model.SportelloId = _sportelloId.Value;
                    //u.ConsulenteCSRepository.Update(model);
                }
                //else
                //{
                //    u.ConsulenteCSRepository.Insert(model);
                //}

                u.SportelloRepository.InsertOrUpdate(model);
                u.Save(false);

                //_x++;
                return _messaggio;

            }
            catch (Exception ex)
            {
                //_x++;
                _error = $"{ex?.Message} {ex?.InnerException?.InnerException?.Message} - ({_az?.RagioneSociale} - {_az?.CodiceFiscalePIva})";
                ErrorList.Add(_error);
                return _error;
            }
        }

        #endregion

        #region Uniemens

        private string InsertUniemens(string righa, int anno)
        {
            var _error = "";
            var jitem = "";
            int id_ebt = 0;

            try
            {
                jitem = righa.Replace("$", "").TrimAll();
                jitem = jitem.Replace("{   }", "[]");
                jitem = jitem.Replace("{  }", "[]");
                jitem = jitem.Replace("{ }", "[]");
                jitem = jitem.Replace("{}", "[]");
                jitem = jitem.Replace("\"oid\":", "");
                jitem = jitem.Replace("\"date\":", "");
                jitem = jitem.Replace("Ebna (2016)", "Ebna");
                jitem = jitem.Replace("Ebna (2022)", "Ebna");

                var _uniemens = JsonConvert.DeserializeObject<UniemensModel>(jitem);

                UnitOfWork u = new UnitOfWork();

                var _az = u.AziendaRepository.Get(m => m.MatricolaInps == _uniemens.matricola_inps).FirstOrDefault();

                if (_az == null)
                {
                    //_x++;
                    _error = $"Azienda {_uniemens.matricola_inps} non trovata, id_ebt: {_uniemens.id_ebt}";
                    ErrorList.Add(_error);
                    return _error;
                }

                int.TryParse(_uniemens.id_ebt?.ToString(), out id_ebt);

                var _messaggio = $"Uniemens per azienda: {_az.AziendaId} - id_ebt: {id_ebt} inserito";
                int? _uniemensId = null;
                var __uniemens = u.UniemensRepository.Get(x => x.Anno == anno && x.AziendaId == _az.AziendaId).FirstOrDefault();

                if (__uniemens != null)
                {
                    _messaggio = $"Uniemens per azienda: {_az.AziendaId} - id_ebt: {id_ebt} aggiornato";
                    _uniemensId = __uniemens.UniemensId;
                }

                Uniemens model = new Uniemens
                {
                    Mensilita = _uniemens.mensilita.Count(),
                    Anno = this.Anno,
                    AziendaId = _az.AziendaId,
                    ID_EBT = id_ebt,
                    UniemensBson = jitem
                };

                if (_uniemensId.HasValue)
                {
                    model.UniemensId = _uniemensId.Value;
                    //u.UniemensRepository.Update(model);
                }
                //else
                //{
                //    u.UniemensRepository.Insert(model);
                //}
                u.UniemensRepository.InsertOrUpdate(model);
                u.Save(false);

                return _messaggio;

            }
            catch (Exception ex)
            {
                _error = $"{ex?.Message} {ex?.InnerException?.InnerException?.Message} - id_ebt: {id_ebt}";
                ErrorList.Add(_error);
                return _error;
            }
        }

        #endregion

        #region Bonifica dati

        public string BonificaAnagraficaAziende()
        {
            try
            {
                UnitOfWork unitOfWork = new UnitOfWork();

                StringBuilder sblog = new StringBuilder();
                int _tot = 0;
                var sql = "select MatricolaInps from(select * from(";
                sql += "SELECT distinct ";
                sql += "[MatricolaInps], COUNT([MatricolaInps]) as tot ";
                sql += "FROM [Azienda] ";
                sql += "group by [MatricolaInps]) as tb ";
                sql += "where tot > 1 ) as tb";

                SediinPraticheRegionaliDbContext c = new SediinPraticheRegionaliDbContext();
                var l = c.Database.SqlQuery<string>(sql);

                foreach (var item in l)
                {
                    var _c = unitOfWork.AziendaRepository.Get(x => x.MatricolaInps == item).ToArray();

                    int[] _a = _c.Select(xx => xx.AziendaId).ToArray();

                    var _pra = unitOfWork.PraticheRegionaliImpreseRepository.Get(x => _a.Contains((int)x.AziendaId));

                    var _idexclude = new List<int>
                    {
                        _a.FirstOrDefault()
                    };

                    if (_pra?.Count() > 0)
                    {
                        _idexclude = new List<int>();
                        _idexclude.AddRange(_pra.Select(x => (int)x.AziendaId).ToList());
                    }

                    for (int i = 0; i < _c.Count(); i++)
                    {
                        try
                        {
                            var _id = _c[i].AziendaId;

                            if (_idexclude.Contains(_id))
                            {
                                continue;
                            }

                            var _aziendeassociate = unitOfWork.DipendenteAziendaRepository.Get(x => x.AziendaId == _id);
                            if (_aziendeassociate.Count() > 0)
                            {
                                foreach (var az in _aziendeassociate)
                                {
                                    unitOfWork.DipendenteAziendaRepository.Delete(az.DipendenteAziendaId);
                                }
                            }
                            _tot++;
                            unitOfWork.AziendaRepository.Delete(_id);
                            unitOfWork.Save(false);
                        }
                        catch (Exception ex)
                        {
                            sblog.AppendLine(ex.ToString());
                        }
                    }
                }

                if (sblog.Length > 0 || _tot > 0)
                {
                    sblog.Insert(0, "<strong>Anagrafica Aziende bonificata: </strong><br/><br/>" + "Aziende bonificati: " + _tot + "<br/>");
                    return sblog.ToString();
                }

                return "Nessun azienda da bonificare";
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string BonificaAnagraficaSportello()
        {
            try
            {
                UnitOfWork unitOfWork = new UnitOfWork();

                StringBuilder sblog = new StringBuilder();
                int _tot = 0;
                var sql = "select CodiceFiscalePIva from(select * from(";
                sql += "SELECT distinct ";
                sql += "[CodiceFiscalePIva], COUNT([CodiceFiscalePIva]) as tot ";
                sql += "FROM [Sportello] ";
                sql += "group by [CodiceFiscalePIva]) as tb ";
                sql += "where tot > 1 ) as tb";

                SediinPraticheRegionaliDbContext c = new SediinPraticheRegionaliDbContext();
                var l = c.Database.SqlQuery<string>(sql);

                foreach (var item in l)
                {
                    var _c = unitOfWork.SportelloRepository.Get(x => x.CodiceFiscalePIva == item).ToArray();

                    int[] _a = _c.Select(xx => xx.SportelloId).ToArray();

                    var _pra = unitOfWork.PraticheRegionaliImpreseRepository.Get(x => _a.Contains((int)x.SportelloId));

                    var _idexclude = new List<int>
                    {
                        _a.FirstOrDefault()
                    };

                    if (_pra?.Count() > 0)
                    {
                        _idexclude = new List<int>();
                        _idexclude.AddRange(_pra.Select(x => (int)x.DipendenteId).ToList());
                    }

                    for (int i = 0; i < _c.Count(); i++)
                    {
                        try
                        {
                            var _id = _c[i].SportelloId;

                            if (_idexclude.Contains(_id))
                            {
                                continue;
                            }

                            var _deleghe = unitOfWork.DelegheSportelloAziendaRepository.Get(x => x.SportelloId == _id);
                            if (_deleghe.Count() > 0)
                            {
                                foreach (var az in _deleghe)
                                {
                                    unitOfWork.DelegheSportelloAziendaRepository.Delete(az.SportelloId);
                                }
                            }

                            var _deleghe2 = unitOfWork.DelegheSportelloDipendenteRepository.Get(x => x.SportelloId == _id);
                            if (_deleghe2.Count() > 0)
                            {
                                foreach (var az in _deleghe)
                                {
                                    unitOfWork.DelegheSportelloDipendenteRepository.Delete(az.SportelloId);
                                }
                            }
                            _tot++;
                            unitOfWork.DipendenteRepository.Delete(_id);
                            unitOfWork.Save(false);
                        }
                        catch (Exception ex)
                        {
                            sblog.AppendLine(ex.ToString());
                        }
                    }
                }

                if (sblog.Length > 0 || _tot > 0)
                {
                    sblog.Insert(0, "<strong>Anagrafica Consulenti bonificata: </strong><br/><br/>" + "Consulenti bonificati: " + _tot + "<br/>");
                    return sblog.ToString();
                }

                return "Nessun consulente da bonificare";
            }
            catch (Exception)
            {
                throw;
            }
        }

        public string BonificaAnagraficaDipendenti()
        {
            try
            {
                UnitOfWork unitOfWork = new UnitOfWork();

                StringBuilder sblog = new StringBuilder();
                int _tot = 0;
                var sql = "select CodiceFiscale from(select * from(";
                sql += "SELECT distinct ";
                sql += "[CodiceFiscale], COUNT([CodiceFiscale]) as tot ";
                sql += "FROM [Dipendente] ";
                sql += "group by [CodiceFiscale]) as tb ";
                sql += "where tot > 1 ) as tb";

                SediinPraticheRegionaliDbContext c = new SediinPraticheRegionaliDbContext();
                var l = c.Database.SqlQuery<string>(sql);

                foreach (var item in l)
                {
                    var _c = unitOfWork.DipendenteRepository.Get(x => x.CodiceFiscale == item).ToArray();

                    int[] _a = _c.Select(xx => xx.DipendenteId).ToArray();

                    var _pra = unitOfWork.PraticheRegionaliImpreseRepository.Get(x => _a.Contains((int)x.DipendenteId));

                    var _idexclude = new List<int>
                    {
                        _a.FirstOrDefault()
                    };

                    if (_pra?.Count() > 0)
                    {
                        _idexclude = new List<int>();
                        _idexclude.AddRange(_pra.Select(x => (int)x.DipendenteId).ToList());
                    }

                    for (int i = 0; i < _c.Count(); i++)
                    {
                        try
                        {
                            var _id = _c[i].DipendenteId;

                            if (_idexclude.Contains(_id))
                            {
                                continue;
                            }

                            var _aziendeassociate = unitOfWork.DipendenteAziendaRepository.Get(x => x.DipendenteId == _id);
                            if (_aziendeassociate.Count() > 0)
                            {
                                foreach (var az in _aziendeassociate)
                                {
                                    unitOfWork.DipendenteAziendaRepository.Delete(az.DipendenteAziendaId);
                                }
                            }
                            _tot++;
                            unitOfWork.DipendenteRepository.Delete(_id);
                            unitOfWork.Save(false);
                        }
                        catch (Exception ex)
                        {
                            sblog.AppendLine(ex.ToString());
                        }
                    }
                }

                if (sblog.Length > 0 || _tot > 0)
                {
                    sblog.Insert(0, "<strong>Anagrafica dipendenti bonificata: </strong><br/><br/>" + "Dipendenti bonificati: " + _tot + "<br/>");
                    return sblog.ToString();
                }

                return "Nessun dipendenti da bonificare";
            }
            catch (Exception)
            {
                throw;
            }
        }

        #endregion
    }
}
