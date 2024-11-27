using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Sediin.PraticheRegionali.Utils;

namespace Sediin.PraticheRegionali.DOM.Entitys
{
    [Table("Ebt")]
    public class Ebt
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int EbtId { get; set; }

        [MaxLength(175)]
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [MaxLength(175)]
        [Required]
        [EmailAddress]
        public string Pec { get; set; }

        [MaxLength(175)]
        //[Required]
        [Display(Name = "Codice SAP/INPS")]
        public string Sap { get; set; }

        [MaxLength(27)]
        [Required]
        [Display(Name = "IBAN Transitorio")]
        public string Iban_Transitorio { get; set; }
        [MaxLength(27)]
        [Required]
        [Display(Name = "IBAN Operativo")]
        public string Iban_Operativo { get; set; }

        public string Note { get; set; }
        [Required]
        [Display(Name = "% F24")]
        public decimal F24_Percentuale { get; set; }
        //[Required]
        [Display(Name = "% Multilocalizzate")]
        public decimal MultiLoc_Percentuale { get; set; }

        public DateTime? Accordo { get; set; }

        public bool Sospeso { get; set; }
        public DateTime? Data_Sospensione { get; set; }


        [MaxLength(175)]
        //[Required]
        [Display(Name = "Nome Referente")]
        public string ReferenteNome { get; set; }

        [MaxLength(175)]
        //[Required]
        [Display(Name = "Cognome Referente")]
        public string ReferenteCognome { get; set; }

        [MaxLength(175)]
        //[Required]
        [EmailAddress]
        [Display(Name = "Email Referente")]
        public string ReferenteEmail { get; set; }

        [MaxLength(175)]
        //[Required]
        [EmailAddress]
        [Display(Name = "Pec Referente")]
        public string ReferentePec { get; set; }

        //[Required]
        [MaxLength(50)]
        [Display(Name = "Cellulare Referente")]
        public string ReferenteCellulare { get; set; }

       


        private int? _RegioneId;

        [Required]
        [DisplayName("Regione")]
        public int? RegioneId { get; set; }

        [Required(ErrorMessage = "Provincia e un campo obbligatorio")]
        [DisplayName(displayName: "Provincia")]
        public int? ProvinciaId { get; set; }  

        [ForeignKey("RegioneId")]
        public virtual Regioni Regione { get; set; }

        [ForeignKey("ProvinciaId")]
        public virtual Province Provincia { get; set; }

        //[Required]
        //[DisplayName("Comune")]
        //public int? ComuneId { get; set; }

        //[Required]
        //[DisplayName("Localita")]
        //public int? LocalitaId { get; set; }

        //[ForeignKey("ComuneId")]
        //public virtual Comuni Comune { get; set; }

        //[ForeignKey("LocalitaId")]
        //public virtual Localita Localita { get; set; }


        public DateTime Data_Inserimento
        {
            get
            {
                return DateTime.Now;
            }
            set
            {

            }
        }
        public DateTime Data_Modifica
        {
            get
            {
                return DateTime.Now;
            }
            set
            {

            }
        }


        [InverseProperty("Ebt")]
        public virtual ICollection<IbanStorico> IbanStorico { get; set; }
        [InverseProperty("Ebt")]
        public virtual ICollection<F24Percentuale> F24Percentuale { get; set; }
        [InverseProperty("Ebt")]
        public virtual ICollection<MultiLocPercentuale> MultiLocPercentuale { get; set; }


    }

    [Table("IbanStorico")]
    public class IbanStorico
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IbanStoricoId { get; set; }
        public int EbtId { get; set; }
        [ForeignKey("EbtId")]
        public virtual Ebt Ebt { get; set; }
        public DateTime DataInserimento { get; set; }
        public string Iban_Operativo { get; set; }
        public string Iban_transitorio { get; set; }

    }

    [Table("F24Percentuale")]
    public class F24Percentuale
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int F24PercentualeId { get; set; }
        public int EbtId { get; set; }
        [ForeignKey("EbtId")]
        public virtual Ebt Ebt { get; set; }
        public DateTime DataInserimento { get; set; }
        public decimal F24 { get; set; } 

    }

    [Table("MultiLocPercentuale")]
    public class MultiLocPercentuale
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MultiLocPercentualeId { get; set; }

        public int EbtId { get; set; }
        [ForeignKey("EbtId")]
        public virtual Ebt Ebt { get; set; } 
        public DateTime DataInserimento { get; set; }
        public decimal MultiLoc { get; set; }

    }
}
