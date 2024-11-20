using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sediin.PraticheRegionali.WebUI.Areas.Admin.Models
{
    public class InstantMessageModel
    {
        [Required]
        public string Messaggio { get; set; }
    }
}