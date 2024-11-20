using Sediin.PraticheRegionali.DOM.Entitys;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Sediin.PraticheRegionali.WebUI.Areas.Admin.Models
{
    public class TipoRichiesteRicercaModel
    {
        public int? Anno { get; set; }
        public string AbilitatoNuovaRichiesta { get; set; }
        public string IsTipoRichiestaDipendente { get; set; }
        public string Modulo { get; set; }
    }

    public class DichiarazioniDPRModel
    {
        public int TipoRichiestaId { get; set; }
        public int? dprMinimo { get; set; }
        public int? dprMassimo { get; set; }
        public IEnumerable<DichiarazioniDPR> DichiarazioniDPR { get; set; }
    }


    public class AllegatiModel
    {
        public int TipoRichiestaId { get; set; }
        public IEnumerable<Allegati> Allegati { get; set; }
    }

    public class RequisitiModel
    {
        public int TipoRichiestaId { get; set; }
        public int? requisitiMinimo { get; set; }
        public int? requisitiMassimo { get; set; }
        public IEnumerable<Requisiti> Requisiti { get; set; }
    }

    public class Allegati
    {
        public bool Selezionato { get; set; }
        public bool Obbligatorio { get; set; }
        public string Nome { get; set; }
        public int AllegatoId{ get; set; }
        public int TipoRichiestaId { get; set; }
        public bool Caricamento { get; set; }
        public bool Modificabile { get; set; }
    }

    public class Requisiti
    {
        public bool Selezionato { get; set; }
        public bool Obbligatorio { get; set; }
        public string Descrizione { get; set; }
        public int RequisitiId { get; set; }
        public int TipoRichiestaId { get; set; }
        public decimal? ContributoImporto { get; set; }
        public decimal? ContributoPercentuale { get; set; }
    }

    public class DichiarazioniDPR
    {
        public bool Selezionato { get; set; }
        public bool Obbligatorio { get; set; }
        public string Descrizione { get; set; }
        public int DichiarazioniDPRId { get; set; }
        public int TipoRichiestaId { get; set; }
    }

    public class Duplica
    {
        [RegularExpression("^(19|20)\\d{2}$", ErrorMessage = "Inserire un anno valido")]
        public int Anno { get; set; }
        public int TipoRichiestaId { get; set; }
    }    
    
    public class Elimina
    {
        public int TipoRichiestaId { get; set; }
    }
}