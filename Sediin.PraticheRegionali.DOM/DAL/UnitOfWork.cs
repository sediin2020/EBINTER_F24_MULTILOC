using Sediin.PraticheRegionali.DOM.Data;
using Sediin.PraticheRegionali.DOM.Entitys;
using System;
using System.Threading.Tasks;

namespace Sediin.PraticheRegionali.DOM.DAL
{
    public class UnitOfWork : IDisposable
    {
        private SediinPraticheRegionaliDbContext context = new SediinPraticheRegionaliDbContext();

        private GenericRepository<Ebt> ebtRepository;
        private GenericRepository<IbanStorico> ibanStoricoRepository;
        private GenericRepository<F24Percentuale> f24PercentualeRepository;
        private GenericRepository<MultiLocPercentuale> multiLocPercentualeRepository;

        public GenericRepository<Ebt> EbtRepository
        {
            get
            {
                if (this.ebtRepository == null)
                {
                    this.ebtRepository = new GenericRepository<Ebt>(context);
                }
                return ebtRepository;
            }
        }
        public GenericRepository<IbanStorico> IbanStoricoRepository
        {
            get
            {
                if (this.ibanStoricoRepository == null)
                {
                    this.ibanStoricoRepository = new GenericRepository<IbanStorico>(context);
                }
                return ibanStoricoRepository;
            }
        }
        public GenericRepository<F24Percentuale> F24PercentualeRepository
        {
            get
            {
                if (this.f24PercentualeRepository == null)
                {
                    this.f24PercentualeRepository = new GenericRepository<F24Percentuale>(context);
                }
                return f24PercentualeRepository;
            }
        }
        public GenericRepository<MultiLocPercentuale> MultiLocPercentualeRepository
        {
            get
            {
                if (this.multiLocPercentualeRepository == null)
                {
                    this.multiLocPercentualeRepository = new GenericRepository<MultiLocPercentuale>(context);
                }
                return multiLocPercentualeRepository;
            }
        }
        /// <summary>
        /// /////////////////////////////////////////////////////////////////
        /// </summary>
        private GenericRepository<Allegati> allegatiRepository;
        private GenericRepository<Requisiti> requisitiRepository;
        private GenericRepository<DichiarazioniDPR> dichiarazionidprRepository;
        private GenericRepository<Tipologia> tipologiaRepository;
        private GenericRepository<Regioni> regioniRepository;
        private GenericRepository<Province> provinceRepository;
        private GenericRepository<Comuni> comuniRepository;
        private GenericRepository<Localita> localitaRepository;
        private GenericRepository<Azienda> aziendaRepository;
        private GenericRepository<TipoRichiesta> tipoRichiestaRepository;
        private GenericRepository<TipoRichiestaRequisiti> tipoRichiestaRequisitiRepository;
        private GenericRepository<TipoRichiestaDichiarazioniDPR> tipoRichiestaDichiarazioniDPRRepository;
        private GenericRepository<TipoRichiestaAllegati> tipoRichiestaAllegatiRepository;
        private GenericRepository<PraticheRegionaliImprese> praticheRegionaliImpreseRepository;
        private GenericRepository<PraticheRegionaliImpreseDatiPratica> praticheRegionaliImpreseDatiPraticaRepository;
        private GenericRepository<PraticheRegionaliImpreseAllegati> praticheRegionaliImpreseAllegatiRepository;
        private GenericRepository<StatoPratica> statoPraticaRepository;
        private GenericRepository<Azioni> azioniPraticaRepository;
        private GenericRepository<AzioniRuolo> azioniRuoloRepository;
        private GenericRepository<PraticheRegionaliImpreseStatoPraticaStorico> praticheRegionaliImpreseStatoPraticaStoricoRepository;
        private GenericRepository<Motivazioni> motivazioniRepository;
        private GenericRepository<MotivazioniRichiesta> motivazioniRichiestaRepository;
        private GenericRepository<Dipendente> dipendenteRepository;
        private GenericRepository<DipendenteAzienda> dipendenteAziendaRepository;
        private GenericRepository<TempoLavoro> tempoLavoroRepository;
        private GenericRepository<TipoContratto> tipoContrattoRepository;
        private GenericRepository<TipoImpiego> tipoImpiegoRepository;
        private GenericRepository<Parentela> parentelaRepository;
        private GenericRepository<Liquidazione> liquidazioneRepository;
        private GenericRepository<LiquidazionePraticheRegionali> liquidazionePraticheRegionaliRepository;
        private GenericRepository<StatoLiquidazione> statoLiquidazioneRepository;
        private GenericRepository<Uniemens> uniemensRepository;
        private GenericRepository<Logs> logsRepository;
        private GenericRepository<Copertura> coperturaRepository;
        private GenericRepository<Sportello> sportelloRepository;
        private GenericRepository<DelegheSportelloDipendente> delegheSportelloDipendenteRepository;
        private GenericRepository<NavigatioHistory> navigatioHistoryRepository;
        private GenericRepository<AvvisoUtente> avvisoUtenteRepository;
        private GenericRepository<AvvisoUtenteRuoli> avvisoUtenteRuoliRepository;
        private GenericRepository<Utente> utentiRepository;
        private GenericRepository<DelegheSportelloAzienda> delegheSportelloAziendaRepository;
        private GenericRepository<PraticheRegionaliImpreseRequisiti> praticheRegionaliImpreseRequisitiRepository;
        private GenericRepository<PraticheRegionaliImpreseRichidente> praticheRegionaliImpreseRichidenteRepository;
        private GenericRepository<PraticheRegionaliImpreseDpr> praticheRegionaliImpreseDprRepository;
        private GenericRepository<LiquidazionePraticheRegionaliMailInviatiEsito> liquidazionePraticheRegionaliMailInviatiEsitoRepository;
        private GenericRepository<ContatoreAnnuale> contatoreAnnualeRepository;



        /// <summary>
        /// /////////////////////////////////////////////////////////////////////////
        /// </summary>


        public GenericRepository<ContatoreAnnuale> ContatoreAnnualeRepository
        {
            get
            {
                if (this.contatoreAnnualeRepository == null)
                {
                    this.contatoreAnnualeRepository = new GenericRepository<ContatoreAnnuale>(context);
                }
                return contatoreAnnualeRepository;
            }
        }

        public GenericRepository<LiquidazionePraticheRegionaliMailInviatiEsito> LiquidazionePraticheRegionaliMailInviatiEsitoRepository
        {
            get
            {
                if (this.liquidazionePraticheRegionaliMailInviatiEsitoRepository == null)
                {
                    this.liquidazionePraticheRegionaliMailInviatiEsitoRepository = new GenericRepository<LiquidazionePraticheRegionaliMailInviatiEsito>(context);
                }
                return liquidazionePraticheRegionaliMailInviatiEsitoRepository;
            }
        }

        public GenericRepository<PraticheRegionaliImpreseDpr> PraticheRegionaliImpreseDprRepository
        {
            get
            {
                if (this.praticheRegionaliImpreseDprRepository == null)
                {
                    this.praticheRegionaliImpreseDprRepository = new GenericRepository<PraticheRegionaliImpreseDpr>(context);
                }
                return praticheRegionaliImpreseDprRepository;
            }
        }

        public GenericRepository<PraticheRegionaliImpreseRichidente> PraticheRegionaliImpreseRichidenteRepository
        {
            get
            {
                if (this.praticheRegionaliImpreseRichidenteRepository == null)
                {
                    this.praticheRegionaliImpreseRichidenteRepository = new GenericRepository<PraticheRegionaliImpreseRichidente>(context);
                }
                return praticheRegionaliImpreseRichidenteRepository;
            }
        }

        public GenericRepository<DichiarazioniDPR> DichiarazioniDPRRepository
        {
            get
            {
                if (this.dichiarazionidprRepository == null)
                {
                    this.dichiarazionidprRepository = new GenericRepository<DichiarazioniDPR>(context);
                }
                return dichiarazionidprRepository;
            }
        }

        public GenericRepository<PraticheRegionaliImpreseRequisiti> PraticheRegionaliImpreseRequisitiRepository
        {
            get
            {
                if (this.praticheRegionaliImpreseRequisitiRepository == null)
                {
                    this.praticheRegionaliImpreseRequisitiRepository = new GenericRepository<PraticheRegionaliImpreseRequisiti>(context);
                }
                return praticheRegionaliImpreseRequisitiRepository;
            }
        }
        public GenericRepository<TipoRichiestaRequisiti> TipoRichiestaRequisitiRepository
        {
            get
            {
                if (this.tipoRichiestaRequisitiRepository == null)
                {
                    this.tipoRichiestaRequisitiRepository = new GenericRepository<TipoRichiestaRequisiti>(context);
                }
                return tipoRichiestaRequisitiRepository;
            }
        }

        public GenericRepository<TipoRichiestaDichiarazioniDPR> TipoRichiestaDichirazioniDPRRepository
        {
            get
            {
                if (this.tipoRichiestaDichiarazioniDPRRepository == null)
                {
                    this.tipoRichiestaDichiarazioniDPRRepository = new GenericRepository<TipoRichiestaDichiarazioniDPR>(context);
                }
                return tipoRichiestaDichiarazioniDPRRepository;
            }
        }

        public GenericRepository<DelegheSportelloAzienda> DelegheSportelloAziendaRepository
        {
            get
            {
                if (this.delegheSportelloAziendaRepository == null)
                {
                    this.delegheSportelloAziendaRepository = new GenericRepository<DelegheSportelloAzienda>(context);
                }
                return delegheSportelloAziendaRepository;
            }
        }

        public GenericRepository<AvvisoUtenteRuoli> AvvisoUtenteRuoliRepository
        {
            get
            {
                if (this.avvisoUtenteRuoliRepository == null)
                {
                    this.avvisoUtenteRuoliRepository = new GenericRepository<AvvisoUtenteRuoli>(context);
                }
                return avvisoUtenteRuoliRepository;
            }
        }

        public GenericRepository<AvvisoUtente> AvvisoUtenteRepository
        {
            get
            {
                if (this.avvisoUtenteRepository == null)
                {
                    this.avvisoUtenteRepository = new GenericRepository<AvvisoUtente>(context);
                }
                return avvisoUtenteRepository;
            }
        }

        public GenericRepository<NavigatioHistory> NavigatioHistoryRepository
        {
            get
            {
                if (this.navigatioHistoryRepository == null)
                {
                    this.navigatioHistoryRepository = new GenericRepository<NavigatioHistory>(context);
                }
                return navigatioHistoryRepository;
            }
        }
        public GenericRepository<DelegheSportelloDipendente> DelegheSportelloDipendenteRepository
        {
            get
            {
                if (this.delegheSportelloDipendenteRepository == null)
                {
                    this.delegheSportelloDipendenteRepository = new GenericRepository<DelegheSportelloDipendente>(context);
                }
                return delegheSportelloDipendenteRepository;
            }
        }

        public GenericRepository<Sportello> SportelloRepository
        {
            get
            {
                if (this.sportelloRepository == null)
                {
                    this.sportelloRepository = new GenericRepository<Sportello>(context);
                }
                return sportelloRepository;
            }
        }

        public GenericRepository<Copertura> CoperturaRepository
        {
            get
            {
                if (this.coperturaRepository == null)
                {
                    this.coperturaRepository = new GenericRepository<Copertura>(context);
                }
                return coperturaRepository;
            }
        }

        public GenericRepository<Logs> LogsRepository
        {
            get
            {
                if (this.logsRepository == null)
                {
                    this.logsRepository = new GenericRepository<Logs>(context);
                }
                return logsRepository;
            }
        }

        public GenericRepository<Uniemens> UniemensRepository
        {
            get
            {
                if (this.uniemensRepository == null)
                {
                    this.uniemensRepository = new GenericRepository<Uniemens>(context);
                }
                return uniemensRepository;
            }
        }
        public GenericRepository<Utente> UtentiRepository
        {
            get
            {
                if (this.utentiRepository == null)
                {
                    this.utentiRepository = new GenericRepository<Utente>(context);
                }
                return utentiRepository;
            }
        }

        #region Metropolitane

        public GenericRepository<Regioni> RegioniRepository
        {
            get
            {
                if (this.regioniRepository == null)
                {
                    this.regioniRepository = new GenericRepository<Regioni>(context);
                }
                return regioniRepository;
            }
        }

        public GenericRepository<Province> ProvinceRepository
        {
            get
            {
                if (this.provinceRepository == null)
                {
                    this.provinceRepository = new GenericRepository<Province>(context);
                }
                return provinceRepository;
            }
        }

        public GenericRepository<Comuni> ComuniRepository
        {
            get
            {
                if (this.comuniRepository == null)
                {
                    this.comuniRepository = new GenericRepository<Comuni>(context);
                }
                return comuniRepository;
            }
        }

        public GenericRepository<Localita> LocalitaRepository
        {
            get
            {
                if (this.localitaRepository == null)
                {
                    this.localitaRepository = new GenericRepository<Localita>(context);
                }
                return localitaRepository;
            }
        }

        #endregion

        #region profilazione utenti

        public GenericRepository<TipoImpiego> TipoImpiegoRepository
        {
            get
            {
                if (this.tipoImpiegoRepository == null)
                {
                    this.tipoImpiegoRepository = new GenericRepository<TipoImpiego>(context);
                }
                return tipoImpiegoRepository;
            }
        }


        public GenericRepository<TipoContratto> TipoContrattoRepository
        {
            get
            {
                if (this.tipoContrattoRepository == null)
                {
                    this.tipoContrattoRepository = new GenericRepository<TipoContratto>(context);
                }
                return tipoContrattoRepository;
            }
        }

        public GenericRepository<TempoLavoro> TempoLavoroRepository
        {
            get
            {
                if (this.tempoLavoroRepository == null)
                {
                    this.tempoLavoroRepository = new GenericRepository<TempoLavoro>(context);
                }
                return tempoLavoroRepository;
            }
        }

        public GenericRepository<DipendenteAzienda> DipendenteAziendaRepository
        {
            get
            {
                if (this.dipendenteAziendaRepository == null)
                {
                    this.dipendenteAziendaRepository = new GenericRepository<DipendenteAzienda>(context);
                }
                return dipendenteAziendaRepository;
            }
        }

        public GenericRepository<Dipendente> DipendenteRepository
        {
            get
            {
                if (this.dipendenteRepository == null)
                {
                    this.dipendenteRepository = new GenericRepository<Dipendente>(context);
                }
                return dipendenteRepository;
            }
        }

        public GenericRepository<Tipologia> TipologiaRepository
        {
            get
            {
                if (this.tipologiaRepository == null)
                {
                    this.tipologiaRepository = new GenericRepository<Tipologia>(context);
                }
                return tipologiaRepository;
            }
        }

       

        public GenericRepository<Azienda> AziendaRepository
        {
            get
            {
                if (this.aziendaRepository == null)
                {
                    this.aziendaRepository = new GenericRepository<Azienda>(context);
                }
                return aziendaRepository;
            }
        }

        //public GenericRepository<DelegheConsulenteCSAzienda> DelegheConsulenteCSAziendaRepository
        //{
        //    get
        //    {
        //        if (this.delegheConsulenteCSAziendaRepository == null)
        //        {
        //            this.delegheConsulenteCSAziendaRepository = new GenericRepository<DelegheConsulenteCSAzienda>(context);
        //        }
        //        return delegheConsulenteCSAziendaRepository;
        //    }
        //}

        //public GenericRepository<ConsulenteCS> ConsulenteCSRepository
        //{
        //    get
        //    {
        //        if (this.consulenteCSRepository == null)
        //        {
        //            this.consulenteCSRepository = new GenericRepository<ConsulenteCS>(context);
        //        }
        //        return consulenteCSRepository;
        //    }
        //}

        #endregion

        #region Pratica Azienda

        public GenericRepository<Allegati> AllegatiRepository
        {
            get
            {
                if (this.allegatiRepository == null)
                {
                    this.allegatiRepository = new GenericRepository<Allegati>(context);
                }
                return allegatiRepository;
            }
        }

        public GenericRepository<Requisiti> RequisitiRepository
        {
            get
            {
                if (this.requisitiRepository == null)
                {
                    this.requisitiRepository = new GenericRepository<Requisiti>(context);
                }
                return requisitiRepository;
            }
        }

        public GenericRepository<PraticheRegionaliImpreseStatoPraticaStorico> PraticheRegionaliImpreseStatoPraticaStoricoRepository
        {
            get
            {
                if (this.praticheRegionaliImpreseStatoPraticaStoricoRepository == null)
                {
                    this.praticheRegionaliImpreseStatoPraticaStoricoRepository = new GenericRepository<PraticheRegionaliImpreseStatoPraticaStorico>(context);
                }
                return praticheRegionaliImpreseStatoPraticaStoricoRepository;
            }
        }

        public GenericRepository<PraticheRegionaliImprese> PraticheRegionaliImpreseRepository
        {
            get
            {
                if (this.praticheRegionaliImpreseRepository == null)
                {
                    this.praticheRegionaliImpreseRepository = new GenericRepository<PraticheRegionaliImprese>(context);
                }
                return praticheRegionaliImpreseRepository;
            }
        }

        public GenericRepository<PraticheRegionaliImpreseDatiPratica> PraticheRegionaliImpreseDatiPraticaRepository
        {
            get
            {
                if (this.praticheRegionaliImpreseDatiPraticaRepository == null)
                {
                    this.praticheRegionaliImpreseDatiPraticaRepository = new GenericRepository<PraticheRegionaliImpreseDatiPratica>(context);
                }
                return praticheRegionaliImpreseDatiPraticaRepository;
            }
        }

        public GenericRepository<PraticheRegionaliImpreseAllegati> PraticheRegionaliImpreseAllegatiRepository
        {
            get
            {
                if (this.praticheRegionaliImpreseAllegatiRepository == null)
                {
                    this.praticheRegionaliImpreseAllegatiRepository = new GenericRepository<PraticheRegionaliImpreseAllegati>(context);
                }
                return praticheRegionaliImpreseAllegatiRepository;
            }
        }

        public GenericRepository<TipoRichiestaAllegati> TipoRichiestaAllegatiRepository
        {
            get
            {
                if (this.tipoRichiestaAllegatiRepository == null)
                {
                    this.tipoRichiestaAllegatiRepository = new GenericRepository<TipoRichiestaAllegati>(context);
                }
                return tipoRichiestaAllegatiRepository;
            }
        }
        public GenericRepository<StatoPratica> StatoPraticaRepository
        {
            get
            {
                if (this.statoPraticaRepository == null)
                {
                    this.statoPraticaRepository = new GenericRepository<StatoPratica>(context);
                }
                return statoPraticaRepository;
            }
        }

        public GenericRepository<AzioniRuolo> AzioniRuoloRepository
        {
            get
            {
                if (this.azioniRuoloRepository == null)
                {
                    this.azioniRuoloRepository = new GenericRepository<AzioniRuolo>(context);
                }
                return azioniRuoloRepository;
            }
        }

        public GenericRepository<Azioni> AzioniPraticaRepository
        {
            get
            {
                if (this.azioniPraticaRepository == null)
                {
                    this.azioniPraticaRepository = new GenericRepository<Azioni>(context);
                }
                return azioniPraticaRepository;
            }
        }

        public GenericRepository<TipoRichiesta> TipoRichiestaRepository
        {
            get
            {
                if (this.tipoRichiestaRepository == null)
                {
                    this.tipoRichiestaRepository = new GenericRepository<TipoRichiesta>(context);
                }
                return tipoRichiestaRepository;
            }
        }

        public GenericRepository<Motivazioni> MotivazioniRepository
        {
            get
            {
                if (this.motivazioniRepository == null)
                {
                    this.motivazioniRepository = new GenericRepository<Motivazioni>(context);
                }
                return motivazioniRepository;
            }
        }

        public GenericRepository<Parentela> ParentelaRepository
        {
            get
            {
                if (this.parentelaRepository == null)
                {
                    this.parentelaRepository = new GenericRepository<Parentela>(context);
                }
                return parentelaRepository;
            }
        }

        public GenericRepository<MotivazioniRichiesta> MotivazioniRichiestaRepository
        {
            get
            {
                if (this.motivazioniRichiestaRepository == null)
                {
                    this.motivazioniRichiestaRepository = new GenericRepository<MotivazioniRichiesta>(context);
                }
                return motivazioniRichiestaRepository;
            }
        }

        #endregion

        #region Liquidazione

        public GenericRepository<Liquidazione> LiquidazioneRepository
        {
            get
            {
                if (this.liquidazioneRepository == null)
                {
                    this.liquidazioneRepository = new GenericRepository<Liquidazione>(context);
                }
                return liquidazioneRepository;
            }
        }
        public GenericRepository<LiquidazionePraticheRegionali> LiquidazionePraticheRegionaliRepository
        {
            get
            {
                if (this.liquidazionePraticheRegionaliRepository == null)
                {
                    this.liquidazionePraticheRegionaliRepository = new GenericRepository<LiquidazionePraticheRegionali>(context);
                }
                return liquidazionePraticheRegionaliRepository;
            }
        }
        public GenericRepository<StatoLiquidazione> StatoLiquidazioneRepository
        {
            get
            {
                if (this.statoLiquidazioneRepository == null)
                {
                    this.statoLiquidazioneRepository = new GenericRepository<StatoLiquidazione>(context);
                }
                return statoLiquidazioneRepository;
            }
        }


        #endregion
        
        public void Save(bool? validateOnSaveEnabled = true)
        {
            context.Configuration.ValidateOnSaveEnabled = validateOnSaveEnabled.GetValueOrDefault();
            context.SaveChanges();
        }

        public async Task SaveAsync(bool? validateOnSaveEnabled = true)
        {
            context.Configuration.ValidateOnSaveEnabled = validateOnSaveEnabled.GetValueOrDefault();
            await context.SaveChangesAsync();
        }

        private bool disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
