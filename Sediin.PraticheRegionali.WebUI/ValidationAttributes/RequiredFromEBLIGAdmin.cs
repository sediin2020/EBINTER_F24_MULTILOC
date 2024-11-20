using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sediin.PraticheRegionali.WebUI.ValidationAttributes
{
    public class RequiredFromSediinPraticheRegionaliAdmin : ValidationAttribute, IClientValidatable
    {

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var user = HttpContext.Current.User;

            if (!user.IsInRole(IdentityHelper.Roles.Admin.ToString()) || !user.IsInRole(IdentityHelper.Roles.Super.ToString()))
            {
                return ValidationResult.Success;
            }

            if (value == null)
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
                ValidationType = "requiredfromsediinpraticheregionaliadmin"
            };
            return new[] { mvr };
        }
    }
}