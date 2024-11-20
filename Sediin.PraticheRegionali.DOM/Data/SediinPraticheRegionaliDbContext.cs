
using Sediin.PraticheRegionali.DOM.Entitys;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sediin.PraticheRegionali.DOM.Data
{
    public class SediinPraticheRegionaliDbContext : DbContext
    {
        //public SediinPraticheRegionaliDbContext() : base("data Source=.\\;Initial Catalog=EBLAC;Integrated Security=True")
        public SediinPraticheRegionaliDbContext() : base("SediinPraticheRegionaliDbContext")
        {
            Database.SetInitializer<SediinPraticheRegionaliDbContext>(null);
            //base.Configuration.ProxyCreationEnabled = false;
        }

        public DbSet<Azienda> Azienda { get; set; }

        //  Gestione Tabelle >> Metropoliotane <<
        public DbSet<Regioni> Regioni { get; set; }

        public DbSet<Province> Province { get; set; }

        public DbSet<Comuni> Comuni { get; set; }

        public DbSet<Localita> Localita { get; set; }

        public DbSet<Motivazioni> Motivazioni { get; set; }

        public DbSet<Tipologia> Tipologia { get; set; }

        public DbSet<Allegati> Allegati { get; set; }

        public DbSet<TipoRichiestaAllegati> TipoRichiestaAllegati { get; set; }

        public DbSet<PraticheRegionaliImprese> PraticheRegionaliImprese { get; set; }

        public DbSet<PraticheRegionaliImpreseDatiPratica> PraticheRegionaliImpreseDatiPratica { get; set; }

        public DbSet<PraticheRegionaliImpreseAllegati> PraticheRegionaliImpreseAllegati { get; set; }

        public DbSet<StatoPratica> StatoPratica { get; set; }

        public DbSet<Utente> Utenti { get; set; }

        public DbSet<TipoRichiesta> TipoRichiesta { get; set; }

        public DbSet<Azioni> Azioni { get; set; }

        public DbSet<AzioniRuolo> AzioniRuolo { get; set; }

        public DbSet<MotivazioniRichiesta> MotivazioniRichiesta { get; set; }

        public DbSet<Dipendente> Dipendente { get; set; }
       
        public DbSet<DipendenteAzienda> DipendenteAzienda { get; set; }
        
        public DbSet<TempoLavoro> TempoLavoro { get; set; }
        
        public DbSet<TipoContratto> TipoContratto { get; set; }
        
        public DbSet<TipoImpiego> TipoImpiego { get; set; }
        
        public DbSet<Parentela> Parentela { get; set; }
        
        public DbSet<Liquidazione> Liquidazione { get; set; }
        
        public DbSet<LiquidazionePraticheRegionali> LiquidazionePraticheRegionali { get; set; }
        
        public DbSet<StatoLiquidazione> StatoLiquidazione { get; set; }

        public DbSet<Uniemens> Uniemens { get; set; }

        public DbSet<Logs> Logs { get; set; }

        public DbSet<Copertura> Copertura { get; set; }

        public DbSet<Sportello> Sportello{ get; set; }

        public DbSet<DelegheSportelloDipendente> DelegheSportelloDipendente { get; set; }
        
        public DbSet<DelegheSportelloAzienda> DelegheSportelloAzienda { get; set; }

        public DbSet<NavigatioHistory> NavigatioHistory { get; set; }

        public DbSet<AvvisoUtente> AvvisoUtente { get; set; }

        public DbSet<AvvisoUtenteRuoli> AvvisoUtenteRuoli { get; set; }
        
        public DbSet<Requisiti> Requisiti { get; set; }
        
        public DbSet<DichiarazioniDPR> DichiarazioniDPR { get; set; }
        
        public DbSet<TipoRichiestaDichiarazioniDPR> TipoRichiestaDichiarazioniDPR { get; set; }
       
        public DbSet<TipoRichiestaRequisiti> TipoRichiestaRequisiti { get; set; }
        
        public DbSet<PraticheRegionaliImpreseRequisiti> PraticheRegionaliImpreseRequisiti { get; set; }
        
        public DbSet<PraticheRegionaliImpreseRichidente> PraticheRegionaliImpreseRichidente { get; set; }
       
        public DbSet<PraticheRegionaliImpreseDpr> PraticheRegionaliImpreseDpr { get; set; }
        
        public DbSet<LiquidazionePraticheRegionaliMailInviatiEsito> LiquidazionePraticheRegionaliMailInviatiEsito { get; set; }
        public DbSet<PraticheRegionaliImpreseStatoPraticaStorico> PraticheRegionaliImpreseStatoPraticaStorico { get; set; }

        public DbSet<ContatoreAnnuale> ContatoreAnnuale { get; set; }
    }
}
