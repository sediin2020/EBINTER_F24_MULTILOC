using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using Sediin.PraticheRegionali.DOM.Entitys;

namespace Sediin.PraticheRegionali.DOM.DAL
{
    public class PraticheAziendaUtility
    {
        public class CalcolaImportoRimborsatoModel
        {
            public decimal? ImportoRichiesto { get; set; }
            public decimal? AliquoteIRPEF { get; set; }
            public decimal? PercentualeContributo { get; set; }
            public decimal? ImportoIRPEF { get; set; }
            public decimal? ImportoContributo { get; set; }
            public decimal? ImportoContributoNetto { get; set; }
            public decimal? ContributoFisso { get; set; }
            public decimal? ContributoImportoMinimo { get; set; }
            public decimal? ContributoImportoMassimo { get; set; }
        }

        public static CalcolaImportoRimborsatoModel CalcolaImportoRimborsatoContributo(int tipoRichiestaId, decimal? importo = 0, decimal? percentualeContributo = null)
        {
            try
            {
                UnitOfWork unitOfWork = new UnitOfWork();

                var _tipoRichiesta = unitOfWork.TipoRichiestaRepository.Get(x => x.TipoRichiestaId == tipoRichiestaId).FirstOrDefault();

                var _percentualeContributo = percentualeContributo.HasValue ? percentualeContributo : (decimal)_tipoRichiesta.ContributoPercentuale.GetValueOrDefault();

                var _aliquoteIRPEF = (decimal)_tipoRichiesta.AliquoteIRPEF.GetValueOrDefault();

                var _contributoImportoMinimo = _tipoRichiesta.ContributoImportoMinimo.GetValueOrDefault();

                var _contributoImportoMassimo = _tipoRichiesta.ContributoImportoMassimo.GetValueOrDefault();

                var _contributoFisso = _tipoRichiesta.ContributoFisso.GetValueOrDefault();

                var _importoRimborsato = 0m;

                if (_tipoRichiesta.ContributoFisso.GetValueOrDefault() > 0 && importo.GetValueOrDefault() == 0)
                {
                    _importoRimborsato = _tipoRichiesta.ContributoFisso.GetValueOrDefault();
                }
                else
                {
                    //if (_percentualeContributo > 0)
                    //{
                    _importoRimborsato = (decimal)Math.Round(((double)importo.GetValueOrDefault() / 100) * (double)_percentualeContributo, 2);

                    //}
                    //else
                    //{
                    //    _importoRimborsato = (decimal)Math.Round((double)importo, 2);
                    //}
                }

                if (_contributoImportoMassimo > 0 && _importoRimborsato > _contributoImportoMassimo)
                {
                    _importoRimborsato = _contributoImportoMassimo;
                }

                var _importoIREF = (decimal)Math.Round(((double)_importoRimborsato / 100) * (double)_aliquoteIRPEF, 2);

                var _importoRimborsatoNetto = _importoRimborsato - _importoIREF;

                var _importoRichiesto = importo > 0 ? importo : _importoRimborsato;

                if ((_importoRichiesto > 0  && _contributoImportoMinimo > 0 && _importoRichiesto < _contributoImportoMinimo))
                {
                    throw new Exception("Importo non valido, importo minimo: " + _contributoImportoMinimo.ToString("n"));
                }

                return new CalcolaImportoRimborsatoModel
                {
                    ImportoRichiesto = importo,
                    AliquoteIRPEF = _aliquoteIRPEF,
                    ImportoContributo = _importoRimborsato,
                    ImportoContributoNetto = _importoRimborsatoNetto,
                    ImportoIRPEF = _importoIREF,
                    ContributoImportoMassimo = _contributoImportoMassimo,
                    ContributoImportoMinimo = _contributoImportoMinimo,
                    PercentualeContributo = _percentualeContributo,
                    ContributoFisso = _contributoFisso
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public static bool VerificaTipoRichiestaUnivocoCodiceFiscale(int aziendaId, int tipoRichiestaId, string codiceFiscale, int richiestaId, string nomeCampo, bool? unica = true)
        {
            try
            {
                UnitOfWork u = new UnitOfWork();
                var _richieste = u.PraticheRegionaliImpreseRepository.Get(x =>
                x.PraticheRegionaliImpreseId != richiestaId
                && x.TipoRichiestaId == tipoRichiestaId
                && (unica != true ? x.AziendaId == aziendaId : true)
                && (x.StatoPraticaId != (int)SediinPraticheRegionaliEnums.StatoPratica.Bozza
                && x.StatoPraticaId != (int)SediinPraticheRegionaliEnums.StatoPratica.Annullata));
                //&& (x.StatoPraticaId == (int)SediinPraticheRegionaliEnums.StatoPratica.Inviata
                //|| x.StatoPraticaId == (int)SediinPraticheRegionaliEnums.StatoPratica.InviataRevisionata
                //|| x.StatoPraticaId == (int)SediinPraticheRegionaliEnums.StatoPratica.Confermata));

                var _datiPratica = _richieste?.Select(x => x.DatiPratica.Where(d => d.Nome != null && d.Nome.ToUpper().EndsWith(nomeCampo.ToUpper())));

                foreach (var item in _datiPratica)
                {
                    foreach (var row in item)
                    {
                        if (row?.Valore?.ToLower() == codiceFiscale?.ToLower())
                        {
                            return true;
                        }
                    }
                }

                return false;

            }
            catch (Exception)
            {
                return false;
            }

        }

        public static decimal? CalcolaRimborsoDipendenteEnergia(decimal? energiaElettricaTotaleAnnoPrecedente, decimal? gasMetanoTotaleAnnoPrecedente, decimal? energiaElettricaTotaleAnnoRichiesta, decimal? gasMetanoTotaleAnnoRichiesta, TipoRichiesta tiporichiesta)
        {
            var _sommaPrecedente = energiaElettricaTotaleAnnoPrecedente.GetValueOrDefault() + gasMetanoTotaleAnnoPrecedente.GetValueOrDefault();

            var _sommaRichiesta = energiaElettricaTotaleAnnoRichiesta.GetValueOrDefault() + gasMetanoTotaleAnnoRichiesta.GetValueOrDefault();

            var _perc = (decimal)Math.Round(((_sommaRichiesta / _sommaPrecedente) * 100) - 100, 2);

            if (_perc >= tiporichiesta.PercentualeEnergia.GetValueOrDefault())
            {
                return tiporichiesta.ContributoFisso;
            }

            throw new Exception("La richiesta non raggiunge l'importo minimo");
        }

        public static decimal? CalcolaRimborsoAziendaEnergia(decimal? energiaElettricaTotaleAnnoPrecedente, decimal? gasMetanoTotaleAnnoPrecedente, decimal? energiaElettricaTotaleAnnoRichiesta, decimal? gasMetanoTotaleAnnoRichiesta, TipoRichiesta tiporichiesta)
        {
            var _sommaPrecedente = energiaElettricaTotaleAnnoPrecedente.GetValueOrDefault() + gasMetanoTotaleAnnoPrecedente.GetValueOrDefault();

            var _sommaRichiesta = energiaElettricaTotaleAnnoRichiesta.GetValueOrDefault() + gasMetanoTotaleAnnoRichiesta.GetValueOrDefault();

            var _dif = _sommaRichiesta - _sommaPrecedente;

            var _somma = (decimal)Math.Round((_dif / 100) * tiporichiesta.PercentualeEnergia.GetValueOrDefault(), 2);

            var _result = _somma;

            if (_result >= tiporichiesta.ContributoImportoMinimo.GetValueOrDefault())
            {
                if (_result >= tiporichiesta.ContributoImportoMassimo.GetValueOrDefault())
                {
                    return tiporichiesta.ContributoImportoMassimo.GetValueOrDefault();
                }

                return _result;
            }

            throw new Exception("La richiesta non raggiunge l'importo minimo");
        }
    }
}
