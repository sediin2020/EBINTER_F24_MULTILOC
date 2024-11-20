using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sediin.PraticheRegionali.WebUI.ValidationAttributes
{
    public class RequiredIsTrueValidator : ValidationAttribute, IClientValidatable
    {
        public string IsRequiredField { get; set; }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var type = validationContext.ObjectInstance.GetType();

            if (type.GetProperty(IsRequiredField).GetValue(validationContext.ObjectInstance)==null)
            {
                return ValidationResult.Success;
            }

            var _IsRequiredField = (bool)type.GetProperty(IsRequiredField).GetValue(validationContext.ObjectInstance);

            if (string.IsNullOrWhiteSpace(value?.ToString()) && _IsRequiredField)
            {
                return new ValidationResult(ErrorMessage);
            }

            return ValidationResult.Success;
        }

        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {

            ModelClientValidationRule mvr = new ModelClientValidationRule
            {
                ErrorMessage = ErrorMessage,
                ValidationType = "requiredistrue"
            };

            mvr.ValidationParameters.Add("isrequiredfield", IsRequiredField);
            return new[] { mvr };
        }
    }
}