using Sediin.PraticheRegionali.WebUI.Filters;
using System.Web.Mvc;

namespace Sediin.PraticheRegionali.WebUI
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new UserOnlineAttribute());
            filters.Add(new CompletaRegistrazioneAttribute());
            filters.Add(new MaxJsonSizeAttribute());
            filters.Add(new EncryptedActionParameterAttribute());
        }
    }
}
