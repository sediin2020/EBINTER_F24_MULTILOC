using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using Sediin.MVC.HtmlHelpers;
using Sediin.PraticheRegionali.Utils;
using Sediin.PraticheRegionali.WebUI.Areas.Admin.Models;
using Sediin.PraticheRegionali.WebUI.Controllers;
using Sediin.PraticheRegionali.WebUI.Filters;
using Sediin.PraticheRegionali.WebUI.Models;
using static Sediin.PraticheRegionali.WebUI.Areas.Admin.Models.Thema;
using static Sediin.PraticheRegionali.WebUI.IdentityHelper;

namespace Sediin.PraticheRegionali.WebUI.Areas.Admin.Controllers
{
    [@Authorize(Roles = new Roles[] { Roles.Admin, Roles.Super })]
    public class ConfigurationController : BaseController
    {
        public ConfigurationController()
        {
        }

        public ActionResult RagioneSociale()
        {
            var _config = ConfigurationProvider.Instance.GetConfigurationFromFile();
            var model = Reflection.CreateModel<RagioneSocialeConfigModel>(_config.RagioneSociale);
            model.LogoBase64 = _config.LogoBase64;

            return AjaxView(model: model);
        }

        [HttpPost]
        public ActionResult RagioneSociale(RagioneSocialeConfigModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                var _config = ConfigurationProvider.Instance.GetConfigurationFromFile();
                _config.RagioneSociale = Reflection.CreateModel<RagioneSociale>(model);
                _config.LogoBase64 = model.LogoBase64;

                ConfigurationProvider.Instance.SaveConfiguration(_config);

                return JsonResultTrue("Ragione sociale aggiornata");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult Sepa()
        {
            var model = Reflection.CreateModel<SepaConfigModel>(ConfigurationProvider.Instance.GetConfigurationFromFile().Sepa);
            return AjaxView(model: model);
        }

        [HttpPost]
        public ActionResult Sepa(SepaConfigModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                var _config = ConfigurationProvider.Instance.GetConfigurationFromFile();
                _config.Sepa = Reflection.CreateModel<Sepa>(model);

                ConfigurationProvider.Instance.SaveConfiguration(_config);

                return JsonResultTrue("Sepa aggiornata");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }


        public ActionResult Ftp()
        {
            var model = Reflection.CreateModel<FTPConfigModel>(ConfigurationProvider.Instance.GetConfigurationFromFile().Ftp);
            return AjaxView(model: model);
        }

        [HttpPost]
        public ActionResult Ftp(FTPConfigModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                var _config = ConfigurationProvider.Instance.GetConfigurationFromFile();
                _config.Ftp = Reflection.CreateModel<FTP>(model);

                ConfigurationProvider.Instance.SaveConfiguration(_config);

                return JsonResultTrue("Ftp aggiornata");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult Email()
        {
            var model = Reflection.CreateModel<MailSettingConfigModel>(ConfigurationProvider.Instance.GetConfigurationFromFile().MailSetting);
            return AjaxView(model: model);
        }

        [HttpPost]
        public ActionResult Email(MailSettingConfigModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    throw new Exception(ModelStateErrorToString(ModelState));
                }

                var _config = ConfigurationProvider.Instance.GetConfigurationFromFile();
                _config.MailSetting = Reflection.CreateModel<MailSetting>(model);

                ConfigurationProvider.Instance.SaveConfiguration(_config);

                return JsonResultTrue("Email settings aggiornata");
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        public ActionResult EmailTest()
        {
            return AjaxView(model: new TestMailSettingConfigModel { Oggetto = "test", Messaggio = "test" });
        }

        [HttpPost]
        public ActionResult EmailTest(TestMailSettingConfigModel model)
        {
            var send = SendMailAsync(new SimpleMailMessage
            {
                Body = model.Messaggio,
                Subject = model.Oggetto,
                ToEmail = model.EmailTo
            });

            if (send.Result == true)
            {
                return JsonResultTrue("Mail inviata a: " + model.EmailTo);
            }

            return JsonResultFalse("Si e verificato un errore nel invio mail, controlla la pagina Logs");
        }

        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Bootstrap(Thema model)
        {
            try
            {
                DisablePageCaching();

                var _config = ConfigurationProvider.Instance.GetConfigurationFromFile();
                _config.Thema = Reflection.CreateModel<ThemaPortale>(model);

                ConfigurationProvider.Instance.SaveConfiguration(_config);

                return JsonResultTrue("Thema aggiornato");
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult Bootstrap()
        {
            var _config = ConfigurationProvider.Instance.GetConfiguration().Thema;

            List<Bootstrap> _list = new List<Bootstrap>();

            foreach (var item in Directory.GetFiles(System.Web.Hosting.HostingEnvironment.MapPath("~/Content/bootstrap-themes")))
            {
                var _file = new FileInfo(item);
                _list.Add(new Bootstrap
                {
                    File = _file.Name,
                    Nome = _file.Name.Replace("bootstrap.", "").Replace(".min.css", ""),

                });
            }

            Thema model = new Thema
            {
                ModalBackgroundoColor = _config.ModalBackgroundoColor,
                ModalColor = _config.ModalColor,
                BootstrapCss = _config.BootstrapCss,
                ColoreFooter = _config.ColoreFooter,
                CustomCss = _config.CustomCss,
                NavBarColorHover = _config.NavBarColorHover,
                SideBarBackgroundColorLogo = _config.SideBarBackgroundColorLogo,
                NavBarColor = _config.NavBarColor,
                NavBarBackgroundoColor = _config.NavBarBackgroundoColor,
                SideBarBackgroundColor = _config.SideBarBackgroundColor,
                SideBarColor = _config.SideBarColor,
                SideBarHoverBackground = _config.SideBarHoverBackground,
                SideBarHoverColor = _config.SideBarHoverColor,
                BootstrapThema = _list
            };

            return AjaxView("Bootstrap", model);
        }

    }
}