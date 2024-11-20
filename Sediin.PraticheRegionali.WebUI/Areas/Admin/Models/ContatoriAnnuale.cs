using Sediin.PraticheRegionali.DOM.Entitys;
using Sediin.PraticheRegionali.WebUI.Areas.Backend.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sediin.PraticheRegionali.WebUI.Areas.Admin.Models
{
    public class ContatoriAnnualeModelRicercaViewModel : IPagingEntity
    {
        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int CurrentPage { get; set; }

        public IEnumerable<ContatoreAnnuale> Result { get; set; }

        public ContatoreAnnualeModel Filtri { get; set; }
    }

    public class ContatoreAnnualeSearchModel
    {
        public string PraticheRegionaliImprese { get; set; }
    }

    public class ContatoreAnnualeModel
    {
        [Required(ErrorMessage = "Chiave Id Errata!")]
        public int ContatoreAnnualeId { get; set; }
        [Required(ErrorMessage = "Descrizione Obbligatoria!")]
        [DisplayName("Descrizione")]
        public string PraticheRegionaliImprese { get; set; }
        public DateTime? DataInizio { get; set; }
        public DateTime? DataFine { get; set; }
        [Required(ErrorMessage = "Tetto Massimo Lordo Obbligatorio!")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Il valore deve essere maggiore di zero.")]
        public decimal? TettoMassimoLordo { get; set; }
    }

    public class InsContatoreAnnuale
    {
        [Required]
        [DisplayName("Descrizione")]
        public string PraticheRegionaliImprese { get; set; }
        public DateTime? DataInizio { get; set; }
        public DateTime? DataFine { get; set; }
        [Required(ErrorMessage = "Tetto Massimo Lordo Obbligatorio!")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Il valore deve essere maggiore di zero.")]
        public decimal? TettoMassimoLordo { get; set; }
    }


    public class ContatoreAnnualeRicercaModel
    {
        public int PageSize { get; set; } = 10;
        public string Ordine { get; set; } = "Descrizione";
    }
    public class EliminaContatoreAnnuale
    {
        public int contatoreannualeId { get; set; }
    }

}