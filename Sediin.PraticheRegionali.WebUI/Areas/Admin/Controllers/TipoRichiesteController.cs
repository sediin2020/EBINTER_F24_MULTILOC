using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using DocumentFormat.OpenXml.Office2016.Excel;
using Sediin.PraticheRegionali.DOM.Entitys;
using Sediin.PraticheRegionali.WebUI.Areas.Admin.Models;
using Sediin.PraticheRegionali.WebUI.Controllers;
using Sediin.PraticheRegionali.WebUI.Filters;
using static Sediin.PraticheRegionali.DOM.SediinPraticheRegionaliEnums;
using static Sediin.PraticheRegionali.WebUI.IdentityHelper;

namespace Sediin.PraticheRegionali.WebUI.Areas.Admin.Controllers
{
    [@Authorize(Roles = new Roles[] { Roles.Admin, Roles.Super })]
    public class TipoRichiesteController : BaseController
    {
        // GET: Backend/TipoRichieste
        public ActionResult Ricerca()
        {
            return AjaxView();
        }

        [HttpPost]
        public ActionResult Ricerca(TipoRichiesteRicercaModel model)
        {
            var _tiporich = unitOfWork.TipoRichiestaRepository.Get(RicercaFilter(model)).OrderByDescending(m => m.Anno).ThenBy(m => m.Descrizione);
            return AjaxView("RicercaList", _tiporich);
        }


        public ActionResult Modifica(int? id = null)
        {
            var _tipo = new TipoRichiesta();

            if (id.GetValueOrDefault() != 0)
            {
                _tipo = unitOfWork.TipoRichiestaRepository.Get(x => x.TipoRichiestaId == id).FirstOrDefault();
            }
            else
            {
                _tipo.AbilitatoNuovaRichiesta = true;
                _tipo.IbanAziendaRequired = true;
                _tipo.IbanTitolareRequired = true;
                _tipo.IsTipoRichiestaDipendente = false;
                _tipo.Anno = DateTime.Now.Year;
            }

            return AjaxView("Modifica", _tipo);
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Modifica(TipoRichiesta model)
        {
            try
            {
                var _t = new TipoRichiesta();

                var _isnew = false;
                if (model.TipoRichiestaId != 0)
                {
                    _t = unitOfWork.TipoRichiestaRepository.Get(x => x.TipoRichiestaId == model.TipoRichiestaId).FirstOrDefault();
                }
                else
                {
                    _t.Classe = typeof(Backend.Models.PraticheAzienda_Semplice).FullName;
                    _t.View = "Semplice";
                    _isnew = true;
                }

                if (model.CoperturaMatricolaInps != null)
                {
                    _t.CoperturaMatricolaInps = model.CoperturaMatricolaInps;
                }
                else
                {
                    _t.CoperturaMatricolaInps = false;
                }

                if (model.AbilitatoNuovaRichiesta != null)
                {
                    _t.AbilitatoNuovaRichiesta = model.AbilitatoNuovaRichiesta;
                }
                else
                {
                    _t.AbilitatoNuovaRichiesta = false;
                }

                if (model.UnaTantum != null)
                {
                    _t.UnaTantum = model.UnaTantum;
                }
                else
                {
                    _t.UnaTantum = false;
                }

                if (model.IsTipoRichiestaDipendente != null)
                {
                    _t.IsTipoRichiestaDipendente = model.IsTipoRichiestaDipendente;
                }
                else
                {
                    _t.IsTipoRichiestaDipendente = false;
                }

                if (model.IbanAziendaRequired != null)
                {
                    _t.IbanAziendaRequired = model.IbanAziendaRequired;
                }
                else
                {
                    _t.IbanAziendaRequired = false;
                }

                if (model.IbanDipendenteRequired != null)
                {
                    _t.IbanDipendenteRequired = model.IbanDipendenteRequired;
                }
                else
                {
                    _t.IbanDipendenteRequired = false;
                }

                if (model.RichiedenteMinimo == null)
                {
                    _t.RichiedenteMinimo = 0;
                }
                else
                {
                    _t.RichiedenteMinimo = model.RichiedenteMinimo;
                }

                if (model.RichiedenteMassimo == null)
                {
                    _t.RichiedenteMassimo = 0;
                }
                else
                {
                    _t.RichiedenteMassimo = model.RichiedenteMassimo;
                }

                if (model.RichiedenteMinimo > model.RichiedenteMassimo)
                {
                    _t.RichiedenteMassimo = model.RichiedenteMinimo;
                }

                if (model.IbanTitolareRequired != null)
                {
                    _t.IbanTitolareRequired = model.IbanTitolareRequired;
                }
                else
                {
                    _t.IbanTitolareRequired = false;
                }

                _t.Descrizione = model.Descrizione;
                _t.Note = model.Note;
                _t.ContributoFisso = model.ContributoFisso;
                _t.Anno = model.Anno;
                _t.MaxRichiesteAnno = model.MaxRichiesteAnno;
                _t.Anno = model.Anno;
                _t.Modulo = model.Modulo;
                _t.RichiedenteTestoTitolo = model.RichiedenteTestoTitolo;
                _t.BudgetDisponibile = model.BudgetDisponibile;
                _t.AliquoteIRPEF = model.AliquoteIRPEF;

                _t.ContributoPercentuale = model.ContributoPercentuale;
                _t.ContributoImportoMinimo = model.ContributoImportoMinimo;
                _t.ContributoImportoMassimo = model.ContributoImportoMassimo;

                _t.IbanTitolareRequired= model.IbanTitolareRequired;

                unitOfWork.TipoRichiestaRepository.InsertOrUpdate(_t);
                unitOfWork.Save();

                if (_isnew)
                {
                    //azioni

                    List<Azioni> _azioni = new List<Azioni>
                    {
                        new Azioni
                        {
                            StatoPraticaId=1,
                            Nome = "Salva bozza Bozza",
                            Action="Bozza",
                            IsSubmit=true,
                            ButtonCss= "primary",
                            TitleSuccessModal= null,
                            Ordine = 1,
                            TipoRichiestaId = _t.TipoRichiestaId
                        },
                        new Azioni
                        {
                            StatoPraticaId=1,
                            Nome = "Invia richiesta",
                            Action="Invia",
                            IsSubmit=true,
                            ButtonCss= "warning",
                            TitleSuccessModal= null,
                            Ordine = 2,
                            TipoRichiestaId = _t.TipoRichiestaId
                       },
                        new Azioni
                        {
                            StatoPraticaId=2,
                            Nome = "Conferma richiesta",
                            Action="Conferma",
                            IsSubmit=true,
                            ButtonCss= "success",
                            TitleSuccessModal= null,
                            Ordine = 5,
                            TipoRichiestaId = _t.TipoRichiestaId
                        },
                        new Azioni
                        {
                            StatoPraticaId=2,
                            Nome = "Metti in Revisione",
                            Action="Revisione",
                            IsSubmit=null,
                            ButtonCss= "warning",
                            TitleSuccessModal= "Revisione richiesta",
                            Ordine = 3,
                            SuccessModalOffcanvas=true,
                            TipoRichiestaId = _t.TipoRichiestaId
                        },
                        new Azioni
                        {
                            StatoPraticaId=2,
                            Nome = "Annulla richiesta",
                            Action="Annulla",
                            IsSubmit=null,
                            ButtonCss= "info",
                            TitleSuccessModal= "Annulla richiesta",
                            Ordine = 4,
                            SuccessModalOffcanvas=true,
                            TipoRichiestaId = _t.TipoRichiestaId
                        },
                         new Azioni
                        {
                            StatoPraticaId=4,
                            Nome = "Invia come Revisionata",
                            Action="InviaRevisionata",
                            IsSubmit=true,
                            ButtonCss= "warning",
                            TitleSuccessModal= null,
                            Ordine = 2,
                            TipoRichiestaId = _t.TipoRichiestaId
                        },
                         new Azioni
                        {
                            StatoPraticaId=4,
                            Nome = "Salva bozza",
                            Action="BozzaRevisionata",
                            IsSubmit=true,
                            ButtonCss= "primary",
                            TitleSuccessModal= null,
                            Ordine = 1,
                            TipoRichiestaId = _t.TipoRichiestaId
                        },
                        new Azioni
                        {
                            StatoPraticaId=4,
                            Nome = "Annulla richiesta",
                            Action="Annulla",
                            IsSubmit=null,
                            ButtonCss= "info",
                            TitleSuccessModal= "Annulla richiesta",
                            SuccessModalOffcanvas=true,
                            Ordine = 3,
                            TipoRichiestaId = _t.TipoRichiestaId
                        },
                         new Azioni
                        {
                            StatoPraticaId=1,
                            Nome = "Annulla richiesta",
                            Action="Annulla",
                            IsSubmit=null,
                            ButtonCss= "info",
                            TitleSuccessModal= "Annulla richiesta",
                            SuccessModalOffcanvas=true,
                            Ordine = 3,
                            TipoRichiestaId = _t.TipoRichiestaId
                        },
                         new Azioni
                        {
                            StatoPraticaId=5,
                            Nome = "Annulla richiesta",
                            Action="Annulla",
                            IsSubmit=null,
                            ButtonCss= "info",
                            TitleSuccessModal= "Annulla richiesta",
                            SuccessModalOffcanvas=true,
                            Ordine = 2,
                            TipoRichiestaId = _t.TipoRichiestaId
                        },
                         new Azioni
                        {
                            StatoPraticaId=5,
                            Nome = "Conferma richiesta",
                            Action="Conferma",
                            IsSubmit=true,
                            ButtonCss= "success",
                            TitleSuccessModal= null,
                            Ordine = 3,
                            TipoRichiestaId = _t.TipoRichiestaId
                        },
                        new Azioni
                        {
                            StatoPraticaId=5,
                            Nome = "Metti in Revisione",
                            Action="Revisione",
                            IsSubmit=null,
                            ButtonCss= "warning",
                            TitleSuccessModal= "Revisione richiesta",
                            SuccessModalOffcanvas=true,
                            Ordine = 1,
                            TipoRichiestaId = _t.TipoRichiestaId
                        },
               };

                    foreach (var item in _azioni)
                    {
                        unitOfWork.AzioniPraticaRepository.Insert(item);
                        unitOfWork.Save();
                    }
                }

                return JsonResultTrue("Tipo richiesta aggiornata");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        private Expression<Func<TipoRichiesta, bool>> RicercaFilter(TipoRichiesteRicercaModel model)
        {
            return x => (model.Anno != null ? x.Anno == model.Anno : true)
            && (model.Modulo != null && model.Modulo != "" ? x.Modulo.Contains(model.Modulo) : true)
            && (model.AbilitatoNuovaRichiesta != null ? (model.AbilitatoNuovaRichiesta == "0" ? x.AbilitatoNuovaRichiesta == false : x.AbilitatoNuovaRichiesta == true) : true)
            && (model.IsTipoRichiestaDipendente != null ? (model.IsTipoRichiestaDipendente == "0" ? x.IsTipoRichiestaDipendente == false : x.IsTipoRichiestaDipendente == true) : true);
        }

        [HttpPost]
        public ActionResult Allegati(AllegatiModel model)
        {
            try
            {
                //prendere tutti con check = true
                var _selezionatiList = new List<Models.Allegati>();
                _selezionatiList = model.Allegati.Where(x => x.Selezionato).ToList();

                //prendere tutti con check = false
                var _cancellatiList = new List<Models.Allegati>();
                _cancellatiList = model.Allegati.Where(x => !x.Selezionato).ToList();

                IEnumerable<int> _x = _cancellatiList.Select(xx => xx.AllegatoId).ToList();

                //quelli da cancellare
                var _daCancellareListt = unitOfWork.TipoRichiestaAllegatiRepository.
                    Get(x => x.TipoRichiestaId == model.TipoRichiestaId
                && _x.Contains(x.AllegatoId)).ToList();

                if (_daCancellareListt.Count() > 0)
                {
                    foreach (var item in _daCancellareListt.ToList())
                    {
                        //verifica prima che non ci sia una righa in PraticheRegionaliImpreseAllegati inserita per la associazione
                        if (unitOfWork.PraticheRegionaliImpreseAllegatiRepository.Get(x => x.TipoRichiestaAllegatiId == item.TipoRichiestaAllegatiId).Count() == 0)
                        {
                            unitOfWork.TipoRichiestaAllegatiRepository.Delete(item);
                        }
                    }
                }

                if (_selezionatiList.Count() > 0)
                {
                    foreach (var item in _selezionatiList)
                    {
                        TipoRichiestaAllegati _tipoRichiestaAllegatiRepository = unitOfWork.TipoRichiestaAllegatiRepository.Get(x => x.TipoRichiestaId == model.TipoRichiestaId && x.AllegatoId == item.AllegatoId).FirstOrDefault();

                        if (_tipoRichiestaAllegatiRepository == null)
                        {
                            _tipoRichiestaAllegatiRepository = new TipoRichiestaAllegati
                            {
                                TipoRichiestaId = model.TipoRichiestaId,
                                AllegatoId = item.AllegatoId,
                                Obblicatorio = item.Obbligatorio,
                                Caricamento=item.Caricamento
                            };
                            unitOfWork.TipoRichiestaAllegatiRepository.Insert(_tipoRichiestaAllegatiRepository);
                        }
                        else
                        {
                            _tipoRichiestaAllegatiRepository.Obblicatorio = item.Obbligatorio;
                            _tipoRichiestaAllegatiRepository.Caricamento = item.Caricamento;
                            unitOfWork.TipoRichiestaAllegatiRepository.Update(_tipoRichiestaAllegatiRepository);
                        }
                    }
                }

                unitOfWork.Save();

                return JsonResultTrue("Allegati per tipo richiesta aggiornati");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        int? _tiporichiestaallegatoid(int tipoRichiestaId, int allegatoid)
        {
            return unitOfWork.TipoRichiestaAllegatiRepository.Get(x => x.TipoRichiestaId == (int)tipoRichiestaId && x.AllegatoId == (int)allegatoid)?.FirstOrDefault().TipoRichiestaAllegatiId;
        }


        public ActionResult Allegati(int tipoRichiestaId)
        {
            var _allegatiTipoRich = unitOfWork.TipoRichiestaAllegatiRepository.Get(x => x.TipoRichiestaId == tipoRichiestaId);

            List<Models.Allegati> _allegati = new List<Models.Allegati>();

            foreach (var x in unitOfWork.AllegatiRepository.Get())
            {
                var _TipoRichiestaAllegatiId = unitOfWork.TipoRichiestaAllegatiRepository.Get(c => c.AllegatoId == x.AllegatoId && c.TipoRichiestaId == tipoRichiestaId).FirstOrDefault()?.TipoRichiestaAllegatiId;

                _allegati.Add(new Models.Allegati
                {
                    AllegatoId = x.AllegatoId,
                    Nome = x.Nome,
                    Obbligatorio = _allegatiTipoRich.FirstOrDefault(c => c.AllegatoId == (int)x.AllegatoId) != null ? _allegatiTipoRich.FirstOrDefault(c => c.AllegatoId == (int)x.AllegatoId).Obblicatorio.GetValueOrDefault() : false,
                    Caricamento = _allegatiTipoRich.FirstOrDefault(c => c.AllegatoId == (int)x.AllegatoId) != null ? _allegatiTipoRich.FirstOrDefault(c => c.AllegatoId == (int)x.AllegatoId).Caricamento.GetValueOrDefault() : false,
                    Selezionato = _allegatiTipoRich.FirstOrDefault(c => c.AllegatoId == (int)x.AllegatoId) != null,
                    TipoRichiestaId = tipoRichiestaId,
                    Modificabile = unitOfWork.PraticheRegionaliImpreseAllegatiRepository.Get(c => c.TipoRichiestaAllegatiId == _TipoRichiestaAllegatiId).FirstOrDefault() == null
                });
            }

            //var _allegati = from x in unitOfWork.AllegatiRepository.Get()
            //                select new Models.Allegati
            //                {
            //                    AllegatoId = x.AllegatoId,
            //                    Nome = x.Nome,
            //                    Obbligatorio = _allegatiTipoRich.FirstOrDefault(c => c.AllegatoId == (int)x.AllegatoId) != null ? _allegatiTipoRich.FirstOrDefault(c => c.AllegatoId == (int)x.AllegatoId).Obblicatorio.GetValueOrDefault() : false,
            //                    Caricamento = _allegatiTipoRich.FirstOrDefault(c => c.AllegatoId == (int)x.AllegatoId) != null ? _allegatiTipoRich.FirstOrDefault(c => c.AllegatoId == (int)x.AllegatoId).Caricamento.GetValueOrDefault() : false,
            //                    Selezionato = _allegatiTipoRich.FirstOrDefault(c => c.AllegatoId == (int)x.AllegatoId) != null,
            //                    TipoRichiestaId = tipoRichiestaId,
            //                    Modificabile = unitOfWork.PraticheRegionaliImpreseAllegatiRepository.Get(c => c.TipoRichiestaAllegatiId == x.t).FirstOrDefault() == null
            //                };

            var model = new AllegatiModel();
            model.Allegati = _allegati;
            model.TipoRichiestaId = tipoRichiestaId;

            return AjaxView("Allegati", model);
        }

        [HttpPost]
        public ActionResult DichiarazioniDPR(DichiarazioniDPRModel model)
        {
            try
            {
                var _riquisitiSelezionati = model.DichiarazioniDPR?.Where(x => x.Selezionato == true);
                var _riquisitiSelezionatiNonObblicatori = _riquisitiSelezionati?.Where(x => x.Obbligatorio != true);


                if (model.dprMinimo == null)
                {
                    model.dprMinimo = 0;
                }

                if (model.dprMinimo > model.dprMassimo)
                {
                    model.dprMassimo = model.dprMinimo;
                }

                if (model.dprMinimo > _riquisitiSelezionatiNonObblicatori.Count())
                {
                    throw new Exception("Il numero dei requisiti selezionato supera il limite dei requisiti consentiti/o.");
                }

                if (model.dprMassimo < _riquisitiSelezionatiNonObblicatori.Count())
                {
                    throw new Exception("Il numero dei requisiti selezionato supera il limite dei requisiti consentiti/o.");
                }


                var _t = unitOfWork.TipoRichiestaDichirazioniDPRRepository.Get(x => x.TipoRichiestaId == model.TipoRichiestaId).ToList();
                var _r = unitOfWork.TipoRichiestaRepository.Get(x => x.TipoRichiestaId == model.TipoRichiestaId).FirstOrDefault();

                if (_t.Count() > 0)
                {
                    foreach (var item in _t.ToList())
                    {
                        unitOfWork.TipoRichiestaDichirazioniDPRRepository.Delete(item);
                    }
                }

                if (model.DichiarazioniDPR.Count() > 0)
                {
                    foreach (var item in model.DichiarazioniDPR)
                    {
                        if (item.Selezionato == false)
                        {
                            continue;
                        }
                        var _a = new TipoRichiestaDichiarazioniDPR();
                        _a.TipoRichiestaId = model.TipoRichiestaId;
                        _a.DichiarazioniDPRId = item.DichiarazioniDPRId;
                        _a.Obblicatorio = item.Obbligatorio;
                        unitOfWork.TipoRichiestaDichirazioniDPRRepository.Insert(_a);
                    }
                }

                _r.DprMinimo = model.dprMinimo;
                _r.DprMassimo = model.dprMassimo;
                unitOfWork.TipoRichiestaRepository.Update(_r);

                unitOfWork.Save(false);

                return JsonResultTrue("Dichiarazioni DPR per tipo richiesta aggiornati");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult DichiarazioniDPR(int tipoRichiestaId)
        {

            var _requisitiTipoRich = unitOfWork.TipoRichiestaDichirazioniDPRRepository.Get(x => x.TipoRichiestaId == tipoRichiestaId);
            var _tiporichiesta = unitOfWork.TipoRichiestaRepository.Get(x => x.TipoRichiestaId == tipoRichiestaId).FirstOrDefault();

            var _requisiti = from x in unitOfWork.DichiarazioniDPRRepository.Get()
                             select new Models.DichiarazioniDPR
                             {
                                 DichiarazioniDPRId = x.DichiarazioniDPRId,
                                 Descrizione = x.Descrizione,
                                 Obbligatorio = _requisitiTipoRich.FirstOrDefault(c => c.DichiarazioniDPRId == (int)x.DichiarazioniDPRId) != null ? _requisitiTipoRich.FirstOrDefault(c => c.DichiarazioniDPRId == (int)x.DichiarazioniDPRId).Obblicatorio.GetValueOrDefault() : false,
                                 Selezionato = _requisitiTipoRich.FirstOrDefault(c => c.DichiarazioniDPRId == (int)x.DichiarazioniDPRId) != null,
                                 TipoRichiestaId = tipoRichiestaId
                             };

            var model = new DichiarazioniDPRModel();
            model.DichiarazioniDPR = _requisiti;
            model.TipoRichiestaId = tipoRichiestaId;
            model.dprMinimo = _tiporichiesta.DprMinimo;
            model.dprMassimo = _tiporichiesta.DprMassimo;

            if (model.dprMinimo == null)
            {
                model.dprMinimo = 0;
            }
            if (model.dprMassimo == null)
            {
                model.dprMassimo = 0;
            }

            return AjaxView("DichiarazioniDPR", model);
        }

        [HttpPost]
        public ActionResult Requisiti(RequisitiModel model)
        {
            try
            {
                var _riquisitiSelezionati = model.Requisiti?.Where(x => x.Selezionato == true);
                var _riquisitiSelezionatiNonObblicatori = _riquisitiSelezionati?.Where(x => x.Obbligatorio != true);

                if (model.requisitiMinimo == null)
                {
                    model.requisitiMinimo = 0;
                }

                if (model.requisitiMinimo > model.requisitiMassimo)
                {
                    model.requisitiMassimo = model.requisitiMinimo;
                }

                if (model.requisitiMinimo > _riquisitiSelezionatiNonObblicatori.Count())
                {
                    throw new Exception("Il numero dei requisiti selezionato supera il limite dei requisiti consentiti/o.");
                }

                if (model.requisitiMassimo > _riquisitiSelezionatiNonObblicatori.Count())
                {
                    throw new Exception("Il numero dei requisiti selezionato supera il limite dei requisiti consentiti/o.");
                }

                var _t = unitOfWork.TipoRichiestaRequisitiRepository.Get(x => x.TipoRichiestaId == model.TipoRichiestaId).ToList();
                var _r = unitOfWork.TipoRichiestaRepository.Get(x => x.TipoRichiestaId == model.TipoRichiestaId).FirstOrDefault();

                if (_t.Count() > 0)
                {
                    foreach (var item in _t.ToList())
                    {
                        unitOfWork.TipoRichiestaRequisitiRepository.Delete(item);
                    }
                }

                if (model.Requisiti.Count() > 0)
                {
                    foreach (var item in model.Requisiti)
                    {
                        if (item.Selezionato == false)
                        {
                            continue;
                        }
                        var _a = new TipoRichiestaRequisiti();
                        _a.TipoRichiestaId = model.TipoRichiestaId;
                        _a.RequisitiId = item.RequisitiId;
                        _a.Obblicatorio = item.Obbligatorio;
                        _a.ContributoImporto = item.ContributoImporto;
                        _a.ContributoPercentuale = item.ContributoPercentuale;
                        unitOfWork.TipoRichiestaRequisitiRepository.Insert(_a);
                    }
                }

                _r.RequisitiMinimo = model.requisitiMinimo;
                _r.RequisitiMassimo = model.requisitiMassimo;
                unitOfWork.TipoRichiestaRepository.Update(_r);

                unitOfWork.Save(false);

                return JsonResultTrue("Requisiti per tipo richiesta aggiornati");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult Requisiti(int tipoRichiestaId)
        {

            var _requisitiTipoRich = unitOfWork.TipoRichiestaRequisitiRepository.Get(x => x.TipoRichiestaId == tipoRichiestaId);
            var _tiporichiesta = unitOfWork.TipoRichiestaRepository.Get(x => x.TipoRichiestaId == tipoRichiestaId).FirstOrDefault();

            var _requisiti = from x in unitOfWork.RequisitiRepository.Get()
                             select new Models.Requisiti
                             {
                                 RequisitiId = x.RequisitiId,
                                 Descrizione = x.Descrizione,
                                 ContributoImporto = _requisitiTipoRich.FirstOrDefault(c => c.RequisitiId == (int)x.RequisitiId) != null ? _requisitiTipoRich.FirstOrDefault(c => c.RequisitiId == (int)x.RequisitiId).ContributoImporto : null,
                                 ContributoPercentuale = _requisitiTipoRich.FirstOrDefault(c => c.RequisitiId == (int)x.RequisitiId) != null ? _requisitiTipoRich.FirstOrDefault(c => c.RequisitiId == (int)x.RequisitiId).ContributoPercentuale : null,
                                 Obbligatorio = _requisitiTipoRich.FirstOrDefault(c => c.RequisitiId == (int)x.RequisitiId) != null ? _requisitiTipoRich.FirstOrDefault(c => c.RequisitiId == (int)x.RequisitiId).Obblicatorio.GetValueOrDefault() : false,
                                 Selezionato = _requisitiTipoRich.FirstOrDefault(c => c.RequisitiId == (int)x.RequisitiId) != null,
                                 TipoRichiestaId = tipoRichiestaId,

                             };

            var model = new RequisitiModel();
            model.Requisiti = _requisiti;
            model.TipoRichiestaId = tipoRichiestaId;
            model.requisitiMinimo = _tiporichiesta.RequisitiMinimo;
            model.requisitiMassimo = _tiporichiesta.RequisitiMassimo;

            if (model.requisitiMinimo == null)
            {
                model.requisitiMinimo = 0;
            }

            if (model.requisitiMassimo == null)
            {
                model.requisitiMassimo = 0;
            }

            return AjaxView("Requisiti", model);
        }

        public ActionResult Duplica(int tipoRichiestaId, int anno)
        {
            var model = new Duplica();
            model.TipoRichiestaId = tipoRichiestaId;
            model.Anno = anno;
            return AjaxView("Duplica", model);
        }

        [HttpPost]
        public ActionResult Duplica(TipoRichiesta model)
        {
            try
            {
                var tipoRichiesta = unitOfWork.TipoRichiestaRepository.Get(x => x.TipoRichiestaId == model.TipoRichiestaId).FirstOrDefault();

                //check se anno esiste
                var t = unitOfWork.TipoRichiestaRepository.Get(x => x.Descrizione == tipoRichiesta.Descrizione && x.Anno == model.Anno);
                if (t.Count() > 0)
                {
                    throw new Exception("Tipo richiesta già presente per l'anno " + model.Anno);
                }

                //se non esiste per quell'anno
                var _nuovaRichiesta = Sediin.MVC.HtmlHelpers.Reflection.CreateModel<TipoRichiesta>(tipoRichiesta);
                _nuovaRichiesta.Anno = model.Anno;
                unitOfWork.TipoRichiestaRepository.Insert(_nuovaRichiesta);
                unitOfWork.Save();
                var identity = _nuovaRichiesta.TipoRichiestaId;
                var _azioni = unitOfWork.AzioniPraticaRepository.Get(x => x.TipoRichiestaId == model.TipoRichiestaId);
                if (_azioni.Count() > 0)
                {
                    foreach (var a in _azioni)
                    {
                        a.TipoRichiestaId = identity;
                        unitOfWork.AzioniPraticaRepository.Insert(a);
                    }
                }
                var _motivazioni = unitOfWork.MotivazioniRichiestaRepository.Get(x => x.TipoRichiestaId == model.TipoRichiestaId);
                if (_motivazioni.Count() > 0)
                {
                    foreach (var m in _motivazioni)
                    {
                        m.TipoRichiestaId = identity;
                        unitOfWork.MotivazioniRichiestaRepository.Insert(m);
                    }
                }
                var _allegati = unitOfWork.TipoRichiestaAllegatiRepository.Get(x => x.TipoRichiestaId == model.TipoRichiestaId);
                if (_allegati.Count() > 0)
                {
                    foreach (var all in _allegati)
                    {
                        all.TipoRichiestaId = identity;
                        unitOfWork.TipoRichiestaAllegatiRepository.Insert(all);
                    }
                }
                unitOfWork.Save();
                return JsonResultTrue("Duplicazione per l'anno " + model.Anno + " effettuata");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult Elimina(int tipoRichiestaId)
        {
            var model = new Elimina();
            model.TipoRichiestaId = tipoRichiestaId;
            return AjaxView("Elimina", model);
        }

        [HttpPost]
        public ActionResult Elimina(TipoRichiesta model)
        {
            try
            {
                var pratiche = unitOfWork.PraticheRegionaliImpreseRepository.Get(x => x.TipoRichiestaId == model.TipoRichiestaId);
                //check se tipologia è usata da pratica
                if (pratiche.Count() > 0)
                {
                    throw new Exception("Impossibile eliminare: tipo richiesta già utilizzata per una pratica");
                }

                //se non usata
                unitOfWork.TipoRichiestaRepository.Delete(model.TipoRichiestaId);
                var _azioni = unitOfWork.AzioniPraticaRepository.Get(x => x.TipoRichiestaId == model.TipoRichiestaId);
                if (_azioni.Count() > 0)
                {
                    foreach (var a in _azioni)
                    {
                        unitOfWork.AzioniPraticaRepository.Delete(a);
                    }
                }
                var _motivazioni = unitOfWork.MotivazioniRichiestaRepository.Get(x => x.TipoRichiestaId == model.TipoRichiestaId);
                if (_motivazioni.Count() > 0)
                {
                    foreach (var m in _motivazioni)
                    {
                        unitOfWork.MotivazioniRichiestaRepository.Delete(m);
                    }
                }
                var _allegati = unitOfWork.TipoRichiestaAllegatiRepository.Get(x => x.TipoRichiestaId == model.TipoRichiestaId);
                if (_allegati.Count() > 0)
                {
                    foreach (var all in _allegati)
                    {
                        unitOfWork.TipoRichiestaAllegatiRepository.Delete(all);
                    }
                }
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