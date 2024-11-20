using Sediin.PraticheRegionali.DOM.Entitys;
using Sediin.PraticheRegionali.DOM.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Sediin.PraticheRegionali.WebUI.Areas.Backend.Models
{
    public class UniemensRicercaModel
    {
        public string Ordine { get; set; } = "Anno asc";

        public int PageSize { get; set; } = 10;

        public int? UniemensRicercaModel_AziendaId { get; set; }

        [DisplayName("Ragione sociale o Matricola Inps")]
        public string UniemensRicercaModel_RagioneSociale { get; set; }

        [DisplayName("Anno Contributi")]
        public int? UniemensRicercaModel_Anno { get; set; }

        public int? UniemensRicercaModel_UniemensId { get; set; }

        public int? UniemensRicercaModel_SportelloId { get; set; }

        public int? UniemensRicercaModel_DipendenteId { get; set; }

        [DisplayName("Azienda tramite Dipendente")]
        public string UniemensRicercaModel_NominativoDipendente { get; set; }
    }

    public class UniemensRicercaViewModel : IPagingEntity
    {
        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int CurrentPage { get; set; }

        public IEnumerable<Uniemens> Result { get; set; }

        public UniemensRicercaModel Filtri { get; set; }
    }

    public class UniemensViewModel
    {
        public Uniemens Uniemens { get; set; }
        public UniemensModel UniemensModel { get; set; }
    }

    public class UniemensMensilitaViewModel
    {
        public Uniemens Uniemens { get; set; }
        public IEnumerable<Mensilita> Mensilita { get; set; }
    }
}