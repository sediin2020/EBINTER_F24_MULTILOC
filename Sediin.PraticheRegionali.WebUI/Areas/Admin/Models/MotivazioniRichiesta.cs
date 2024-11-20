﻿using Sediin.PraticheRegionali.DOM.Entitys;
using Sediin.PraticheRegionali.WebUI.Areas.Backend.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sediin.PraticheRegionali.WebUI.Areas.Admin.Models
{
    public class MotivazioniRichiestaModelRicercaViewModel : IPagingEntity
    {
        public int PageSize { get; set; }

        public int TotalRecords { get; set; }

        public int CurrentPage { get; set; }

        public IEnumerable<MotivazioniRichiesta> Result { get; set; }

        public MotivazioniRichiestaModel Filtri { get; set; }
    }
    public class MotivazioniRichiestaSearchModel
    {
        public int? TipoRichiestaId { get; set; }
        public string Motivazione { get; set; }
        public int? MotivazioniRichiestaRicercaModel_TipoRichiestaId { get; set; }
    }

    public class MotivazioniRichiestaModel
    {
        [Required(ErrorMessage = "Motivazioni Richiesta Id Obbligatoria!")]
        public int MotivazioniRichiestaId { get; set; }
        public int? TipoRichiestaId { get; set; }
        [Required]
        [DisplayName("Motivazione")]
        public string Motivazione { get; set; }
    }

    public class InsMotivazioniRichiesta
    {
        [Required]
        [DisplayName("Codice Motivazione Richiesta")]
        public int MotivazioniRichiestaId { get; set; }
        [Required]
        [DisplayName("Tipo Richiesta")]
        public int TipoRichiestaId { get; set; }
        [Required]
        [DisplayName("Descrizione")]
        public string Motivazione { get; set; }
        public IEnumerable<TipoRichiesta> TipoRichiesta { get; set; }

    }

    public class MotivazioniRichiestaRicercaModel
    {
        public int PageSize { get; set; } = 10;
        public string Ordine { get; set; } = "Motivazione";
    }
        public class EliminaMotivazioneRichiesta
    {
        public int MotivazioniRichiestaId { get; set; }
    }

}