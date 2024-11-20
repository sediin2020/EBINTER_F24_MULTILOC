using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sediin.PraticheRegionali.DOM.Entitys
{
    [Table("Sportello")]
    public class Sportello
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SportelloId { get; set; }

        [Required]
        [DisplayName("Codice Fiscale / Partita Iva")]
        [MaxLength(16)]
        public string CodiceFiscalePIva { get; set; }

        [Required]
        [DisplayName("Ragione Sociale")]
        [MaxLength(175)]
        public string RagioneSociale { get; set; }

        [Required]
        [MaxLength(175)]
        public string Indirizzo { get; set; }

        [Required]
        [DisplayName("Regione")]
        public int? RegioneId { get; set; }

        [Required]
        [DisplayName(displayName: "Provincia")]
        public int? ProvinciaId { get; set; }

        [Required]
        [DisplayName("Comune")]
        public int? ComuneId { get; set; }

        [Required]
        [DisplayName("Localita")]
        public int? LocalitaId { get; set; }

        [Required]
        [MaxLength(175)]
        public string Cognome { get; set; }

        [Required]
        [MaxLength(175)]
        public string Nome { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(175)]
        public string Pec { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(175)]
        public string Email { get; set; }

        [Required]
        [MaxLength(50)]
        public string Telefono { get; set; }

        [Required]
        [MaxLength(50)]
        public string Cellulare { get; set; }

        public virtual Regioni Regione { get; set; }

        public virtual Province Provincia { get; set; }

        public virtual Comuni Comune { get; set; }

        public virtual Localita Localita { get; set; }

        [ForeignKey("SportelloId")]
        public virtual ICollection<DelegheSportelloDipendente> DelegheSportelloDipendente { get; set; }

        [ForeignKey("SportelloId")]
        public virtual ICollection<DelegheSportelloAzienda> DelegheSportelloAzienda { get; set; }

        [Required]
        public string Ruolo { get; set; }

        public bool? AutorizzoComunicazioni { get; set; }
    }

    [Table("DelegheSportelloDipendente")]
    public class DelegheSportelloDipendente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DelegheSportelloDipendenteId { get; set; }
       
        public int SportelloId { get; set; }

        [ForeignKey("SportelloId")]
        public virtual Sportello Sportello { get; set; }
      
        public int DipendenteId { get; set; }
   
        [ForeignKey("DipendenteId")]
        public virtual Dipendente Dipendente{ get; set; }

        public DateTime? DataInserimento { get; set; }

        public DateTime? DataDelegaDisdetta { get; set; }

        public bool? DelegaAttiva { get; set; }

        [MaxLength(75)]
        public string DocumentoIdentita { get; set; }

        [MaxLength(75)]
        public string DelegaDipendente { get; set; }
    }


    [Table("DelegheSportelloAzienda")]
    public class DelegheSportelloAzienda
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int DelegheSportelloAziendaId { get; set; }

        public int SportelloId { get; set; }

        public int AziendaId { get; set; }

        public DateTime? DataInserimento { get; set; }

        public DateTime? DataDelegaDisdetta { get; set; }

        public bool? DelegaAttiva { get; set; }

        [MaxLength(75)]
        public string DocumentoIdentita { get; set; }

        [MaxLength(75)]
        public string DelegaAzienda { get; set; }

        [ForeignKey("SportelloId")]
        public virtual Sportello Sportello { get; set; }

        [ForeignKey("AziendaId")]
        public virtual Azienda Azienda { get; set; }
    }

}
