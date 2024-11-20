using Sediin.PraticheRegionali.DOM.Entitys;
using Sediin.PraticheRegionali.WebUI.Areas.Backend.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sediin.PraticheRegionali.WebUI.Areas.Admin.Models
{
    public class LogsRicercaModel
    {
        public int PageSize { get; set; } = 10;
        public string Ordine { get; set; } = "Data desc";

        public string Username { get; set; }

        public string Data { get; set; }
    }

    public class LogsRicercaViewModel : IPagingEntity
    {
        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int CurrentPage { get; set; }

        public IEnumerable<Logs> Result { get; set; }

        public LogsRicercaModel Filtri { get; set; }
    }
}