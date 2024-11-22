using DocumentFormat.OpenXml.Wordprocessing;
using Sediin.PraticheRegionali.DOM.Entitys;
using Sediin.PraticheRegionali.WebUI.ValidationAttributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sediin.PraticheRegionali.WebUI.Areas.Backend.Models
{
    public class EbtRicercaModel
    {
        public int? EbtRicercaModel_RegioneId { get; set; }
        public string EbtRicercaModel_Regione { get; set; }
        public int? EbtRicercaModel_ProvinciaId { get; set; }
        public string EbtRicercaModel_Provincia { get; set; }
        public string Ordine { get; set; } = "";

        public int PageSize { get; set; } = 10;


    }

    public class EbtRicercaViewModel : IPagingEntity
    {
        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int CurrentPage { get; set; }

        public IEnumerable<Ebt> Result { get; set; }

        public EbtRicercaModel Filtri { get; set; }
    }

    public class EbtViewModel : Ebt
    {
        public bool? ReadOnly { get; set; }
        public string Iban_Operativo_Old { get; set; }
        public string Iban_Transitorio_Old { get; set; }
        public decimal F24_Percentuale_Old { get; set; }
        public decimal MultiLoc_Percentuale_Old { get; set; }

    }


}