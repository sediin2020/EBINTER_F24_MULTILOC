using System.Web;
using EBLIG.WebUI.Filters;

namespace Sediin.PraticheRegionali.WebUI.Filters
{
    public class Authorize : UnauthorizedRequest
    {
        public new IdentityHelper.Roles[] Roles { get; set; }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (!httpContext.User.Identity.IsAuthenticated)
            {
                return false;
            }

            if (Roles == null)
            {
                return true;
            }

            if (httpContext.User.IsInRole(IdentityHelper.Roles.Admin.ToString()) 
                || httpContext.User.IsInRole(IdentityHelper.Roles.Super.ToString()))
            {
                return true;
            }

            foreach (var role in Roles)
            {
                if (httpContext.User.IsInRole(role.ToString()))
                {
                    return true;
                }
            }

            return false;
        }
    }
}