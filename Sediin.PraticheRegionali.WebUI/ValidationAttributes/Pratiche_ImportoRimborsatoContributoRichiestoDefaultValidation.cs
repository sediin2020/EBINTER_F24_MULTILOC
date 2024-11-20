using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNetCore.Http;
using Sediin.PraticheRegionali.DOM.DAL;


namespace Sediin.PraticheRegionali.WebUI.ValidationAttributes
{
    public class Pratiche_ImportoRimborsatoContributoRichiestoDefaultValidation : ValidationAttribute, IClientValidatable
    {
        public IdentityHelper.Roles[] RolesValidate { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            try
            {
                var user = System.Web.HttpContext.Current.User;

                var _userinrole = false;

                foreach (var role in RolesValidate)
                {
                    if (user.IsInRole(role.ToString()))
                    {
                        _userinrole = true;
                        break;
                    }
                }

                if (!_userinrole)
                {
                    return ValidationResult.Success;
                }

                if (string.IsNullOrWhiteSpace(value?.ToString()))
                {
                    return new ValidationResult(ErrorMessage);
                }

                int.TryParse(value?.ToString(), out int _importoRichiesto);

                if (_importoRichiesto == 0)
                {
                    return new ValidationResult("Importo richiesto deve essere maggiore a 0");
                }

                var type = validationContext.ObjectInstance.GetType();

                int.TryParse(type.GetProperty("TipoRichiestaId").GetValue(validationContext.ObjectInstance)?.ToString(), out int _tipoRichiestaId);

                var _imortoCalcolato = PraticheAziendaUtility.CalcolaImportoRimborsatoContributo(_tipoRichiestaId, _importoRichiesto);

                return ValidationResult.Success;
            }
            catch (System.Exception ex)
            {
                return new ValidationResult(ex.Message);

            }
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {

            ModelClientValidationRule mvr = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessage,
                ValidationType = "praticheimportorimborsatocontributorichiestodefaultvalidation"
            };

            //mvr.ValidationParameters.Add("importoerogatoelement", "ImportoErogato");

            return new[] { mvr };
        }
    }
}