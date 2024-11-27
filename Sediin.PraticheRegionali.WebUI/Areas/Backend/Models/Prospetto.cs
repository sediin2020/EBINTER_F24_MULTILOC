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
using Microsoft.AspNetCore.Http;

namespace Sediin.PraticheRegionali.WebUI.Areas.Backend.Models
{
    public class ProspettoRicercaModel
    {
        
        public string ProspettoRicercaModel_Anno { get; set; }        
        public string ProspettoRicercaModel_Mese { get; set; }
        public string Ordine { get; set; } = "";
        public int PageSize { get; set; } = 10;


    }

    public class ProspettoRicercaViewModel : IPagingEntity
    {
        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int CurrentPage { get; set; }

        public IEnumerable<Prospetto> Result { get; set; }

        public ProspettoRicercaModel Filtri { get; set; }
    }

    public class ProspettoViewModel : Prospetto
    {
        public bool? ReadOnly { get; set; }

        [Required]
        [DisplayName("File Prospetto")]
        public string File_Prospetto { get; set; }



    }


}