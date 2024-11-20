using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Sediin.PraticheRegionali.WebUI.Startup))]
namespace Sediin.PraticheRegionali.WebUI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            app.MapSignalR();
        }
    }
}
