using Sediin.PraticheRegionali.DOM.Entitys;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sediin.PraticheRegionali.WebUI.Areas.Admin.Models
{
    public class RequisitiClass
    {
        public string Descrizione { get; set; }
    }
    
    public class InsRequisiti
    {
        [Required]
        public string Descrizione { get; set; }
    }

    public class EliminaRequisito
    {
        public int RequisitiId { get; set; }
    }
}