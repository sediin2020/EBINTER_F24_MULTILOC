using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sediin.PraticheRegionali.DOM.Entitys
{
    [Table("PraticheRegionaliImprese")]
    public class PraticheRegionaliImprese
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PraticheRegionaliImpreseId { get; set; }

        public int AziendaId { get; set; }

        [ForeignKey("AziendaId")]
        public virtual Azienda Azienda { get; set; }

        public int TipoRichiestaId { get; set; }

        [ForeignKey("TipoRichiestaId")]
        public virtual TipoRichiesta TipoRichiesta { get; set; }

        public int StatoPraticaId { get; set; }

        [ForeignKey("StatoPraticaId")]
        public virtual StatoPratica StatoPratica { get; set; }

        public DateTime DataInserimento { get; set; }

        public DateTime? DataInvio { get; set; }

        public DateTime? DataConferma { get; set; }

        [MaxLength(75)]
        public string UsernameInvio { get; set; }

        [MaxLength(75)]
        public string UsernameConferma { get; set; }

        [MaxLength(75)]
        public string ProtocolloId { get; set; }

        //public int? ConsulenteCSId { get; set; }

        //[ForeignKey("ConsulenteCSId")]
        //public virtual ConsulenteCS ConsulenteCS { get; set; }

        [ForeignKey("PraticheRegionaliImpreseId")]
        public virtual ICollection<PraticheRegionaliImpreseDatiPratica> DatiPratica { get; set; }

        [ForeignKey("PraticheRegionaliImpreseId")]
        public virtual ICollection<PraticheRegionaliImpreseAllegati> Allegati { get; set; }

        [ForeignKey("PraticheRegionaliImpreseId")]
        public virtual ICollection<PraticheRegionaliImpreseStatoPraticaStorico> StatoPraticaStorico { get; set; }

        [ForeignKey("PraticheRegionaliImpreseId")]
        public virtual ICollection<PraticheRegionaliImpreseRequisiti> Requisiti { get; set; }
     
        [ForeignKey("PraticheRegionaliImpreseId")]
        public virtual ICollection<PraticheRegionaliImpreseDpr> Dpr { get; set; }


        [ForeignKey("PraticheRegionaliImpreseId")]
        public virtual ICollection<PraticheRegionaliImpreseRichidente> Richidenti { get; set; }

        public string UserInserimento { get; set; }

        public string RuoloUserInserimento { get; set; }

        public int? DipendenteId { get; set; }
        [ForeignKey("DipendenteId")]
        public virtual Dipendente Dipendente { get; set; }

        public int? SportelloId { get; set; }
        [ForeignKey("SportelloId")]
        public virtual Sportello Sportello { get; set; }

        public decimal? ImportoRichiesto { get; set; }
        
        public decimal? ImportoContributo { get; set; }

        public decimal? ImportoContributoNetto { get; set; }
        
        public decimal? PercentualeContributo { get; set; }

        public decimal? AliquoteIRPEF { get; set; }

        public decimal? ImportoIRPEF { get; set; }
       
        public decimal? ContributoImportoMinimo { get; set; }
       
        public decimal? ContributoImportoMassimo { get; set; }
       
        public decimal? ContributoFisso { get; set; }

        public int? ChildClassRowCount { get; set; }

        [ForeignKey("PraticheRegionaliImpreseId")]
        public virtual ICollection<LiquidazionePraticheRegionali> LiquidazionePraticheRegionali { get; set; }

        [MaxLength(50)]
        public string Iban { get; set; }

        public bool? Responsabilita { get; set; }
    }

    [Table("PraticheRegionaliImpreseDatiPratica")]
    public class PraticheRegionaliImpreseDatiPratica
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PraticheRegionaliImpreseDatiPraticaId { get; set; }

        [Required]
        public int PraticheRegionaliImpreseId { get; set; }

        [MaxLength(255)]
        public string Nome { get; set; }

        public string Valore { get; set; }
    }

    [Table("PraticheRegionaliImpreseStatoPraticaStorico")]
    public class PraticheRegionaliImpreseStatoPraticaStorico
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PraticheRegionaliImpreseStatoPraticaStoricoId { get; set; }

        public int PraticheRegionaliImpreseId { get; set; }

        public int StatoPraticaId { get; set; }

        [ForeignKey("StatoPraticaId")]
        public virtual StatoPratica StatoPratica { get; set; }

        public string Note { get; set; }

        public DateTime DataInserimento { get; set; }

        public int? MotivazioniId { get; set; }

        [ForeignKey("MotivazioniId")]
        public virtual Motivazioni Motivazione { get; set; }

        public string UserName { get; set; }

        public string UserRuolo { get; set; }
    }

    [Table("PraticheRegionaliImpreseAllegati")]
    public class PraticheRegionaliImpreseAllegati
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PraticheRegionaliImpreseAllegatiId { get; set; }

        [Required]
        public int PraticheRegionaliImpreseId { get; set; }

        [Required]
        public int TipoRichiestaAllegatiId { get; set; }

        [MaxLength(255)]
        public string FilenameOriginale { get; set; }

        [MaxLength(255)]
        public string Filename { get; set; }

    }

    [Table("PraticheRegionaliImpreseRequisiti")]
    public class PraticheRegionaliImpreseRequisiti
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PraticheRegionaliImpreseRequisitiId { get; set; }

        [Required]
        public int PraticheRegionaliImpreseId { get; set; }

        [Required]
        public int RequisitiId { get; set; }

    }

    [Table("PraticheRegionaliImpreseDpr")]
    public class PraticheRegionaliImpreseDpr
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PraticheRegionaliImpreseDprId { get; set; }

        [Required]
        public int PraticheRegionaliImpreseId { get; set; }

        [Required]
        public int DichiarazioniDPRId { get; set; }
    }

    [Table("PraticheRegionaliImpreseRichidente")]
    public class PraticheRegionaliImpreseRichidente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PraticheRegionaliImpreseRichidenteId { get; set; }

        [Required]
        public int PraticheRegionaliImpreseId { get; set; }

        [Required]
        [MaxLength(255)]
        public string Nominativo { get; set; }
      
        [Required]
        [MaxLength(16)]
        public string CodiceFiscale { get; set; }
    }

    [Table("StatoPratica")]
    public class StatoPratica
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StatoPraticaId { get; set; }

        [MaxLength(75)]
        public string Descrizione { get; set; }

        public bool? ReadOnly { get; set; }

        public int? Ordine { get; set; }

    }

    [Table("TipoRichiesta")]
    public class TipoRichiesta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TipoRichiestaId { get; set; }

        [Required]
        [MaxLength(50)]
        public string Modulo { get; set; }

        [Required]
        [MaxLength(500)]
        public string Descrizione { get; set; }

        [Required]
        [MaxLength(75)]
        public string View { get; set; }

        [Required]
        [MaxLength(500)]
        public string Classe { get; set; }

        public string Note { get; set; }

        public decimal? ContributoFisso { get; set; }

        [Required]
        public int? Anno { get; set; }

        public int? MaxRichiesteAnno { get; set; }

        public bool? CoperturaMatricolaInps { get; set; }

        public bool? AbilitatoNuovaRichiesta { get; set; }

        public bool? IsTipoRichiestaDipendente { get; set; }

        [MaxLength(125)]
        public string ChildClass { get; set; }

        public decimal? AliquoteIRPEF { get; set; }

        public bool? IbanAziendaRequired { get; set; }

        public bool? IbanDipendenteRequired { get; set; }

        public decimal? BudgetDisponibile { get; set; }

        [ForeignKey("TipoRichiestaId")]
        public virtual ICollection<PraticheRegionaliImprese> PraticheRegionaliImprese { get; set; }

        [ForeignKey("TipoRichiestaId")]
        public virtual ICollection<TipoRichiestaDichiarazioniDPR> TipoRichiestaDpr { get; set; }

        [ForeignKey("TipoRichiestaId")]
        public virtual ICollection<TipoRichiestaRequisiti> TipoRichiestaRequisiti { get; set; }

        public int? RequisitiMinimo { get; set; }
        public int? RequisitiMassimo { get; set; }

        public int? RichiedenteMinimo { get; set; }
        public int? RichiedenteMassimo { get; set; }

        public int? DprMinimo { get; set; }
        public int? DprMassimo { get; set; }

        public string RichiedenteTestoTitolo { get; set; }


        public bool? UnaTantum { get; set; }
        public decimal? ContributoPercentuale { get; set; }
        public decimal? ContributoImportoMinimo { get; set; }
        public decimal? ContributoImportoMassimo { get; set; }

        public decimal? PercentualeEnergia { get; set; }

        public bool? IbanTitolareRequired { get; set; }

    }

    [Table("Allegati")]
    public class Allegati
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AllegatoId { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Nome { get; set; }
    }

    [Table("TipoRichiestaAllegati")]
    public class TipoRichiestaAllegati
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TipoRichiestaAllegatiId { get; set; }

        public int AllegatoId { get; set; }

        [ForeignKey("AllegatoId")]
        public virtual Allegati Allegato { get; set; }

        public int TipoRichiestaId { get; set; }

        [ForeignKey("TipoRichiestaId")]
        public virtual TipoRichiesta TipoRichiesta { get; set; }

        public bool? Obblicatorio { get; set; }

        public bool? Caricamento { get; set; }

    }

    [Table("Azioni")]
    public class Azioni
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AzioniId { get; set; }

        public int TipoRichiestaId { get; set; }

        public int StatoPraticaId { get; set; }

        [MaxLength(75)]
        public string Nome { get; set; }

        [MaxLength(75)]
        public string Action { get; set; }

        [MaxLength(75)]
        public string Controller { get; set; }

        public bool? IsSubmit { get; set; }

        [MaxLength(75)]
        public string ButtonCss { get; set; }

        public bool? SuccessModalFullScreen { get; set; }

        public bool? SuccessModalOffcanvas { get; set; }

        [MaxLength(125)]
        public string TitleSuccessModal { get; set; }

        public int? Ordine { get; set; }

    }

    [Table("AzioniRuolo")]
    public class AzioniRuolo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AzioniRuoloId { get; set; }

        [MaxLength(55)]
        public string Action { get; set; }

        [MaxLength(25)]
        public string Ruolo { get; set; }

        public int StatoPraticaId { get; set; }
    }

    [Table("Motivazioni")]
    public class Motivazioni
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MotivazioniId { get; set; }

        public int StatoPraticaId { get; set; }

        [MaxLength(75)]
        public string Motivazione { get; set; }

        public string Note { get; set; }
    }

    [Table("MotivazioniRichiesta")]
    public class MotivazioniRichiesta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MotivazioniRichiestaId { get; set; }

        public int TipoRichiestaId { get; set; }

        [MaxLength(500)]
        public string Motivazione { get; set; }
    }

    [Table("Parentela")]
    public class Parentela
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ParentelaId { get; set; }

        [MaxLength(500)]
        public string Descrizione { get; set; }


        [MaxLength(500)]
        public string Note { get; set; }



    }


    [Table("Requisiti")]
    public class Requisiti
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RequisitiId { get; set; }

        [MaxLength(2500)]
        public string Descrizione { get; set; }

        [ForeignKey("RequisitiId")]
        public virtual ICollection<TipoRichiestaRequisiti> TipoRichiestaRequisiti { get; set; }

    }


    [Table("DichiarazioniDPR")]
    public class DichiarazioniDPR
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DichiarazioniDPRId { get; set; }

        [MaxLength(2500)]
        public string Descrizione { get; set; }

        [ForeignKey("DichiarazioniDPRId")]
        public virtual ICollection<TipoRichiestaDichiarazioniDPR> TipoRichiestaDichiarazioniDPR { get; set; }

    }



    [Table("TipoRichiestaRequisiti")]
    public class TipoRichiestaRequisiti
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TipoRichiestaRequisitiId { get; set; }
        public int RequisitiId { get; set; }
        [ForeignKey("RequisitiId")]
        public virtual Requisiti Requisiti { get; set; }

        public int TipoRichiestaId { get; set; }
        [ForeignKey("TipoRichiestaId")]
        public virtual TipoRichiesta TipoRichiesta { get; set; }

        public bool? Obblicatorio { get; set; }

        public decimal? ContributoImporto { get; set; }

        public decimal? ContributoPercentuale { get; set; }
    }

    [Table("TipoRichiestaDichiarazioniDPR")]
    public class TipoRichiestaDichiarazioniDPR
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TipoRichiestaDichiarazioniDPRId { get; set; }
        public int DichiarazioniDPRId { get; set; }
        [ForeignKey("DichiarazioniDPRId")]
        public virtual DichiarazioniDPR DichiarazioniDPR { get; set; }

        public int TipoRichiestaId { get; set; }
        [ForeignKey("TipoRichiestaId")]
        public virtual TipoRichiesta TipoRichiesta { get; set; }

        public bool? Obblicatorio { get; set; }
    }

    [Table("ContatoreAnnuale")]
    public class ContatoreAnnuale
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContatoreAnnualeId { get; set; }

        public string PraticheRegionaliImprese { get; set; }

        public DateTime? DataInizio { get; set; }

        public DateTime? DataFine{ get; set; }

        public decimal? TettoMassimoLordo { get; set; }
    }
}