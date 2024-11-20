using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;
using Sediin.PraticheRegionali.DOM;
using Sediin.PraticheRegionali.DOM.Entitys;
using Sediin.PraticheRegionali.WebUI.ValidationAttributes;
using static Sediin.PraticheRegionali.WebUI.IdentityHelper;

namespace Sediin.PraticheRegionali.WebUI.Areas.Backend.Models
{
    public class VisualizzaBudgetViewModel
    {
        public TipoRichiesta TipoRichiesta { get; set; }

        public decimal? ImportoRichiesto { get; set; }
        public decimal? ImportoRichiestoBozza { get; set; }
        public decimal? ImportoRichiestoRevisione { get; set; }
        public decimal? ImportoRichiestoConfermato { get; set; }
    }

    public class PraticheAziendaRicercaModel
    {
        public bool? PraticheAziendaRicercaModel_InseritaoSportello { get; set; }

        public int? PraticheAziendaRicercaModel_TipoRichiestaId { get; set; }

        public string PraticheAziendaRicercaModel_NominativoDipendente { get; set; }

        public int? PraticheAziendaRicercaModel_DipendenteId { get; set; }

        public string PraticheAziendaRicercaModel_NominativoSportello { get; set; }

        public int? PraticheAziendaRicercaModel_SportelloId { get; set; }

        public string PraticheAziendaRicercaModel_RagioneSociale { get; set; }

        public int? PraticheAziendaRicercaModel_AziendaId { get; set; }

        public string PraticheAziendaRicercaModel_ProtocolloId { get; set; }

        public List<TipoRichiesta> TipoRichiesta { get; set; }

        public int? PraticheAziendaRicercaModel_StatoPraticaId { get; set; }

        public string PraticheAziendaRicercaModel_DataInvio { get; set; }

        public List<StatoPratica> StatoPratica { get; set; }
        
        public List<StatoLiquidazione> StatoLiquidazione { get; set; }

        public string PraticheAziendaRicercaModel_OrderBy { get; set; } = "DataInvio == null, DataInvio asc";

        public int? PraticheAziendaRicercaModel_StatoLiquidazioneId { get; set; }

    }

    public class PraticheAziendaRicercaViewModel : IPagingEntity
    {
        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int CurrentPage { get; set; }

        public decimal? ImportoLiquidato { get; set; }
        public decimal? ImportoDaLiquidare { get; set; }
        public decimal? ImportoInLiquidare { get; set; }
        public decimal? ImportoRiconoscitoNetto { get; set; }

        public IEnumerable<PraticheRegionaliImprese> Result { get; set; }

        public PraticheAziendaRicercaModel Filtri { get; set; }
    }

    public class PraticheAziendaNuovaRichiestaDipendente
    {
        public int? PraticheAziendaNuovaRichiesta_SportelloId { get; set; }

        [Required]
        [DisplayName("Nominativo Dipendente o Codice Fiscale")]
        public int? PraticheAziendaNuovaRichiesta_DipendenteId { get; set; }

        [Required]
        [DisplayName("Nominativo Dipendente o Codice Fiscale")]
        public string PraticheAziendaNuovaRichiesta_NominativoDipendente { get; set; }

        [Required]
        [DisplayName("Ragione Sociale o Matricola Inps")]
        public int? PraticheAziendaNuovaRichiesta_AziendaId { get; set; }

        [Required]
        [DisplayName("Ragione Sociale o Matricola Inps")]
        public string PraticheAziendaNuovaRichiesta_RagioneSociale { get; set; }

        [Required]
        [DisplayName("Tipo Richiesta")]
        public int? PraticheAziendaNuovaRichiesta_TipoRichiestaId { get; set; }

        public IEnumerable<TipoRichiesta> TipoRichiesta { get; set; }
    }

    public class PraticheAziendaNuovaRichiestaAzienda
    {
        public int? PraticheAziendaNuovaRichiesta_SportelloId { get; set; }

        [Required]
        [DisplayName("Ragione Sociale o Matricola Inps")]
        public int? PraticheAziendaNuovaRichiesta_AziendaId { get; set; }

        [Required]
        [DisplayName("Ragione Sociale o Matricola Inps")]
        public string PraticheAziendaNuovaRichiesta_RagioneSociale { get; set; }

        [Required]
        [DisplayName("Tipo Richiesta")]
        public int? PraticheAziendaNuovaRichiesta_TipoRichiestaId { get; set; }

        public IEnumerable<TipoRichiesta> TipoRichiesta { get; set; }
    }

    public class PraticheAziendaNuovaRichiesta
    {
        //[Required]
        [DisplayName("Dipendente, Nominativo o Codice Fiscale")]
        public int? PraticheAziendaNuovaRichiesta_DipendenteId { get; set; }

        [DisplayName("Nominativo")]
        public string PraticheAziendaNuovaRichiesta_NominativoDipendente { get; set; }

        //[Required]
        [DisplayName("Ragione Sociale o Matricola Inps")]
        public int? PraticheAziendaNuovaRichiesta_AziendaId { get; set; }

        //[Required]
        [DisplayName("Ragione Sociale o Matricola Inps")]
        public string PraticheAziendaNuovaRichiesta_RagioneSociale { get; set; }

        [Required]
        [DisplayName("Tipo Richiesta")]
        public int? PraticheAziendaNuovaRichiesta_TipoRichiestaId { get; set; }

        public IEnumerable<TipoRichiesta> TipoRichiesta { get; set; }

    }
    public class PraticheAziendaDpr
    {
        public bool? ReadOnly { get; set; }
        public int? RichiestaId { get; set; }
        public List<TipoRichiestaDichiarazioniDPR> Dpr { get; set; }
        public List<int> DprSelected { get; set; }

        public PraticheDpr[] PraticheDpr { get; set; }

        public int? DprMinimo { get; set; }

        public int? DprMassimo { get; set; }

        [Required()]
        [Range(1, int.MaxValue)]
        public int? TotaleDprSelezionate { get; set; }

    }

    public class PraticheDpr
    {
        public int DprId { get; set; }

        public bool? Selectedt { get; set; }
    }


    public class PraticheAziendaRequisiti
    {
        public bool? ReadOnly { get; set; }
        public int? RichiestaId { get; set; }
        public List<TipoRichiestaRequisiti> Requisiti { get; set; }
        public List<int> RequisitiSelected { get; set; }

        public PraticheRequisito[] PraticheRequisiti { get; set; }

        public int? RequisitiMinimo { get; set; }

        public int? RequisitiMassimo { get; set; }

        [Required()]
        [Range(1, int.MaxValue)]
        public int? TotaleRequisitiSelezionate { get; set; }

    }

    public class PraticheRequisito
    {
        public int RequisitiId { get; set; }

        public bool? Selectedt { get; set; }
    }


    public class PraticheAziendaRichidenti
    {
        public bool? ReadOnly { get; set; }

        public int? RichiestaId { get; set; }

        public int? RichiedenteMinimo { get; set; }

        public int? RichiedenteMassimo { get; set; }

        public List<PraticheRichidente> PraticheRichiedenti { get; set; }

        public string Titolo { get; set; }

        [Required()]
        [Range(1, int.MaxValue)]
        public int? TotaleRichiedenteSelezionate { get; set; }
    }

    public class PraticheRichidente
    {
        public string Nominativo { get; set; }

        public string CodiceFiscale { get; set; }

    }

    public class PraticheAziendaAllegati
    {
        public bool? ReadOnly { get; set; }
        public int? RichiestaId { get; set; }
        public List<TipoRichiestaAllegati> TipoRichiestaAllegati { get; set; }
        public List<PraticheRegionaliImpreseAllegati> RichiestaAllegati { get; set; }

        [DocumentiObblicatori(ErrorMessage = "Documentazione richiesta, caricare tutti documenti obbligatori", AllegatiIdSelInput = "AllegatiIdSelInput")]
        public string AllegatiId { get; set; }

        public string AllegatiIdSelInput { get; set; }

        public PraticheAziendaAllegatiUpload[] File { get; set; }

    }

    public class PraticheAziendaAllegatiUpload
    {
        public int TipoRichiestaAllegatiId { get; set; }

        public string Base64 { get; set; }

        public string NomeFile { get; set; }

        public string Estensione { get; set; }

        public string CodTipAlldescrizione { get; set; }

        public string Completefilename { get; set; }

        public int? PraticheRegionaliImpreseAllegatiId { get; set; }
    }

    public class PraticheAziendaAzioni
    {
        public IEnumerable<Azioni> Azioni { get; set; }

        public int? RichiestaId { get; set; }

        public int? TipoRichiestaId { get; set; }

        public int? StatoId { get; set; }

        public IEnumerable<AzioniRuolo> AzioniRuolo { get; set; }

        public bool? LiquidataOinLiquidazione { get; set; }
    }

    public class PraticheAziendaRevisione_Annulla
    {
        [Required]
        [DisplayName("Motivazione")]
        public int MotivazioneId { get; set; }

        [Required]
        [DisplayName("Richiesta")]
        public int RichiestaId { get; set; }

        public string Note { get; set; }

        public IEnumerable<Motivazioni> Motivazioni { get; set; }

        [Required]
        public SediinPraticheRegionaliEnums.StatoPratica StatoPratica { get; set; }

        public string Protocollo { get; set; }
    }

    public class PraticheAziendaMail
    {
        public string Nominativo { get; set; }

        public string Descrizione { get; set; }

        public string Note { get; set; }

        public string Protocollo { get; set; }
    }


    #region tipo pratica

    [AttributeUsage(AttributeTargets.Property, Inherited = false)]
    public class PraticheAzienda_ImportoContributoRichiestoAttribute : Attribute
    {

    }

    public class PraticheAziendaContainer
    {
        public PraticheAziendaContainer()
        {
            //StatoId = 1;//bozza
        }

        public bool? ReadOnly { get; set; }

        public bool? IsTipoRichiestaDipendente { get; set; }

        //[Required]
        public int RichiestaId { get; set; } = 0;

        [Required]
        public int TipoRichiestaId { get; set; }

        [Required]
        public int AziendaId { get; set; }

        public int? DipendenteId { get; set; }

        //[Required]
        public int StatoId { get; set; }

        public object DataModel { get; set; }

        public string View { get; set; }

        public string DescrizioneStato { get; set; }

        public string DescrizioneTipoRichiesta { get; set; }

        public string NoteTipoRichiesta { get; set; }

        public string Azione { get; set; }

        public string ProtocolloId { get; set; }

        public IEnumerable<PraticheRegionaliImpreseStatoPraticaStorico> StoricoStatoPratica { get; set; }

        public int? ChildClassRowCount { get; set; } = 0;

        public decimal? ImportoRichiesto { get; set; }

        public decimal? ImportoContributo { get; set; }

        public decimal? AliquoteIRPEF { get; set; }

        public decimal? ImportoIRPEF { get; set; }

        public decimal? ImportoContributoNetto { get; set; }

        public decimal? PercentualeContributo { get; set; }

        [NotMapped]
        [MaxLength(30)]
        [IfIBAN(ErrorMessage = "Il campo Iban non è valido")]
        public string Iban { get; set; }

        public bool LiquidataOinLiquidazione { get; set; }

        public PraticheRegionaliImprese PraticheRegionaliImprese { get; set; }

        public bool IbanAziendaRequired { get; set; }

        public bool IbanDipendenteRequired { get; set; }

        [CheckBoxValidation(ErrorMessage = "Il campo Dichiaro di essere consapevole delle responsabilità è obbligatorio.")]
        public bool? Responsabilita { get; set; } = false;

        public bool? IbanTitolareRequired { get; set; }
    }

    public class PraticheAzienda_BaseClass
    {
        [NotMapped]
        public bool? ReadOnly { get; set; }

        [NotMapped]
        public int? RichiestaId { get; set; }

        [NotMapped]
        public int? AziendaId { get; set; }

        [NotMapped]
        public int? DipendenteId { get; set; }

        [NotMapped]
        public int? StatoPraticaId { get; set; }

        [NotMapped]
        public int? TipoRichiestaId { get; set; }

        [NotMapped]
        public TipoRichiesta TipoRichiesta { get; set; }

        [NotMapped]
        public int? ChildClassRowCount { get; set; } = 0;

        [NotMapped]
        public string CodiceFiscale { get; set; }

        public decimal? ImportoRichiesto { get; set; }
        public decimal? AliquoteIRPEF { get; set; }
        public decimal? PercentualeContributo { get; set; }
        public decimal? ImportoIRPEF { get; set; }
        public decimal? ImportoContributo { get; set; }
        public decimal? ImportoContributoNetto { get; set; }
        public decimal? ContributoFisso { get; set; }
        public decimal? ContributoImportoMinimo { get; set; }
        public decimal? ContributoImportoMassimo { get; set; }
    }

    public class PraticheAzienda_Semplice : PraticheAzienda_BaseClass
    {
    }

    public class PraticheAzienda_Semplice_Calcolo : PraticheAzienda_BaseClass
    {
    }

    public class PraticheAzienda_SostegnoFsba : PraticheAzienda_BaseClass
    {
        [Required]
        public string Mesi { get; set; }

        public string MeseSelezionato { get; set; }
    }

    public class PraticheAzienda_Assunzioni : PraticheAzienda_BaseClass
    {
        public string TipoAssunzioni { get; set; }

        [Required(ErrorMessage = "La data assunzione deve essere obbligatoria.")]
        [DisplayName("Data assunzione")]
        public DateTime? DataAssunzione { get; set; }

        [PraticheAzienda_ImportoContributoRichiestoAttribute]
        public decimal ImportoContributoRichiestoAssunzioni { get; set; }

    }


    public class PraticheAzienda_VeicoliCommerciali : PraticheAzienda_BaseClass
    {
        [PraticheAzienda_ImportoContributoRichiestoAttribute]
        [Range(0.01, int.MaxValue, ErrorMessage ="Inserire un importo maggiore a 0,00")]
        public decimal ImportoContributoRichiestoVeicoli{ get; set; }

    }

    public class PraticheAzienda_BorsaStudioFormazione : PraticheAzienda_BaseClass
    {
        [PraticheAzienda_ImportoContributoRichiestoAttribute]
        [Range(0.01, int.MaxValue, ErrorMessage ="Inserire un importo maggiore a 0,00")]
        public decimal ImportoContributoRichiestoStudioFormazione{ get; set; }

    }

    public class PraticheAzienda_Energia : PraticheAzienda_BaseClass
    {
        public bool? ElettricaIntestatePersonaConvivente { get; set; }

        [RequiredIsTrueValidator(IsRequiredField = "ElettricaIntestatePersonaConvivente", ErrorMessage = "Utenze Elettrica persona convivente")]
        public string ElettricaNomePersonaConvivente { get; set; }

        public bool? GasMetanoIntestatePersonaConvivente { get; set; }

        [RequiredIsTrueValidator(IsRequiredField = "GasMetanoIntestatePersonaConvivente", ErrorMessage = "Utenze gas/metano persona convivente")]
        public string GasMetanoNomePersonaConvivente { get; set; }

        [Required(ErrorMessage = "Il campo anno precedente è obbligatorio")]
        [RegularExpression("(?:(?:202)[0-9]{1})", ErrorMessage = "Il campo anno precedente non e valido")]
        public int? AnnoPrecedente { get; set; }

        [Required(ErrorMessage = "Il campo anno richiesta è obbligatorio")]
        [RegularExpression("(?:(?:202)[0-9]{1})", ErrorMessage = "Il campo anno richiesta non e valido")]
        public int? AnnoRichiesta { get; set; }

        public decimal? EnergiaElettricaTotaleAnnoPrecedente { get; set; }

        public decimal? GasMetanoTotaleAnnoPrecedente { get; set; }

        public decimal? EnergiaElettricaTotaleAnnoRichiesta { get; set; }

        public decimal? GasMetanoTotaleAnnoRichiesta { get; set; }

        [PraticheAzienda_ImportoContributoRichiesto]
        [PraticheAziendaCalcoloEnergiaValidation]
        public decimal? TotaleRimborsoRichiesto { get; set; }
    }



    public class PraticheAzienda_Calcolo
    {
        public int? StatoPraticaId { get; set; }

        [DisplayName("Importo richiesto")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Inserire un importo maggiore a 0,00 euro")]
        [Pratiche_ImportoRimborsatoContributoRichiestoDefaultValidation(ErrorMessage = "Importo non valido", RolesValidate = new Roles[] { Roles.Admin })]
        [RequiredFromSediinPraticheRegionaliAdmin]
        public decimal? ImportoRichiesto { get; set; }
        public decimal? AliquoteIRPEF { get; set; }

        [Pratiche_ImportoRimborsatoContributoRichiestoDefaultValidation(ErrorMessage = "Percentuale non valida", RolesValidate = new Roles[] { Roles.Admin })]
        [Range(0.01, 100, ErrorMessage = "Inserire una percentuale maggiore a 0,00")]
        public decimal? PercentualeContributo { get; set; }

        public decimal? ImportoIRPEF { get; set; }
        public decimal? ImportoContributo { get; set; }
        public decimal? ImportoContributoNetto { get; set; }
        public decimal? ContributoFisso { get; set; }
        public decimal? ContributoImportoMinimo { get; set; }
        public decimal? ContributoImportoMassimo { get; set; }
    }

    #endregion


    public class PraticheAzienda_ContatoreAnnuale
    {
        public DateTime? DataInizio{ get; set; }
        public DateTime? DataFine{ get; set; }

        public decimal? TettoMassimo { get; set; }

        public decimal? ImportoRichieste { get; set; }

    }

}