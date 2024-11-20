using Sediin.PraticheRegionali.DOM.Entitys;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sediin.PraticheRegionali.WebUI.Areas.Admin.Models
{
    public class DichiarazioniDPRClass
    {
        public string Descrizione { get; set; }
    }
    
    public class InsDichiarazioniDPR
    {
        [Required]
        public string Descrizione { get; set; }
    }

    public class EliminaDichiarazioneDPR
    {
        public int DichiarazioniDPRId { get; set; }
    }
}