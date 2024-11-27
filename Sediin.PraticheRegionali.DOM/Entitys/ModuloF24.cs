using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using Sediin.PraticheRegionali.Utils;
using System.Diagnostics;

namespace Sediin.PraticheRegionali.DOM.Entitys
{
    [Table("Prospetto")]
    public class Prospetto
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProspettoId { get; set; }
        [Required]
        [StringLength(4)]
        public string Anno { get; set; }
        [Required]
        [StringLength(2)]
        public string Mese { get; set; }

        [Required]
        [StringLength(175)]
        public string Descrizione { get; set; }

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

        //[Required]
        public string FileName { get; set; }

        public int? Numero_Quote { get; set; }
        public decimal? Importo_Totale { get; set; }

        [InverseProperty("Prospetto")]
        public virtual ICollection<Quote> Quote { get; set; }

    }

    [Table("Quote")]
    public class Quote
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QuoteId { get; set; }
        public int ProspettoId { get; set; }
        [ForeignKey("ProspettoId")]
        public virtual Prospetto Prospetto { get; set; }
        public int EbtId { get; set; }
        [ForeignKey("EbtId")]
        public virtual Ebt Ebt { get; set; }
        public string Iban { get; set; } 
        public decimal Saldo { get; set; }
        public DateTime Data_Riferimento { get; set; }         
     
    }
}
