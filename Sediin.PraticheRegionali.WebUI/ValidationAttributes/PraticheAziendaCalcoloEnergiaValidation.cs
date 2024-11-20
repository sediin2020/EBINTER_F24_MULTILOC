using Sediin.PraticheRegionali.DOM.DAL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sediin.PraticheRegionali.WebUI.ValidationAttributes
{
    public class PraticheAziendaCalcoloEnergiaValidation : ValidationAttribute, IClientValidatable
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {

                int getint(object anno)
                {
                    int.TryParse(anno?.ToString(), out int _anno);
                    return _anno;
                };

                decimal getdecimal(object importo)
                {
                    decimal.TryParse(importo?.ToString(), out decimal _importo);
                    return _importo;
                };

                var type = validationContext.ObjectInstance.GetType();

                var annoprecedente = getint(type.GetProperty("AnnoPrecedente").GetValue(validationContext.ObjectInstance));
                var annorichiesta = getint(type.GetProperty("AnnoRichiesta").GetValue(validationContext.ObjectInstance));

                if (annoprecedente < 2020)
                {
                    return new ValidationResult("Anno precedente non valido");
                }

                if (annoprecedente >= DateTime.Now.Year)
                {
                    return new ValidationResult("Anno precedente non valido");
                }

                if (annorichiesta < 2020)
                {
                    return new ValidationResult("Anno richiesta non valido");
                }

                if (annorichiesta > DateTime.Now.Year)
                {
                    return new ValidationResult("Anno richiesta non valido");
                }

                if (annoprecedente >= annorichiesta)
                {
                    return new ValidationResult("Il anno precedente deve essere minore a anno richiesta");
                }

                var _tiporichiestaid = getint(type.GetProperty("TipoRichiestaId").GetValue(validationContext.ObjectInstance));
                var _energiaElettricaTotaleAnnoPrecedente = getdecimal(type.GetProperty("EnergiaElettricaTotaleAnnoPrecedente").GetValue(validationContext.ObjectInstance));
                var _gasMetanoTotaleAnnoPrecedente = getdecimal(type.GetProperty("GasMetanoTotaleAnnoPrecedente").GetValue(validationContext.ObjectInstance));
                var _energiaElettricaTotaleAnnoRichiesta = getdecimal(type.GetProperty("EnergiaElettricaTotaleAnnoRichiesta").GetValue(validationContext.ObjectInstance));
                var _gasMetanoTotaleAnnoRichiesta = getdecimal(type.GetProperty("GasMetanoTotaleAnnoRichiesta").GetValue(validationContext.ObjectInstance));

                UnitOfWork unitOfWork = new UnitOfWork();
                var _tiporichiesta = unitOfWork.TipoRichiestaRepository.Get(x => x.TipoRichiestaId == _tiporichiestaid).FirstOrDefault();

                if (_tiporichiesta == null)
                {
                    return new ValidationResult("Tipo richiesta non valida");
                }

                //calcolo
                decimal? _TotaleRimborsoRichiesto = 0;

                if (_tiporichiesta.IsTipoRichiestaDipendente.GetValueOrDefault())
                {
                    _TotaleRimborsoRichiesto = PraticheAziendaUtility.CalcolaRimborsoDipendenteEnergia(_energiaElettricaTotaleAnnoPrecedente, _gasMetanoTotaleAnnoPrecedente, _energiaElettricaTotaleAnnoRichiesta, _gasMetanoTotaleAnnoRichiesta, _tiporichiesta);
                }
                else
                {
                    _TotaleRimborsoRichiesto = PraticheAziendaUtility.CalcolaRimborsoAziendaEnergia(_energiaElettricaTotaleAnnoPrecedente, _gasMetanoTotaleAnnoPrecedente, _energiaElettricaTotaleAnnoRichiesta, _gasMetanoTotaleAnnoRichiesta, _tiporichiesta);
                }

                type.GetProperty("TotaleRimborsoRichiesto").SetValue(validationContext.ObjectInstance, _TotaleRimborsoRichiesto.GetValueOrDefault());

                value = _TotaleRimborsoRichiesto.GetValueOrDefault();

                if (getdecimal(value) == 0)
                {
                    return new ValidationResult("Importo rimborso non po essere 0");
                }

                return ValidationResult.Success;
            }
            catch (Exception ex)
            {
                return new ValidationResult(ex.Message);
            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {

            ModelClientValidationRule mvr = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessage,
                ValidationType = "pratichecalcoloenergiavalidation"
            };

            return new[] { mvr };
        }
    }
}