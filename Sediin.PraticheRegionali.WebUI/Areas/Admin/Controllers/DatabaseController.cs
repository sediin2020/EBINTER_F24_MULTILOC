using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;
using Sediin.PraticheRegionali.DOM.DbScript;
using Sediin.PraticheRegionali.Utils;
using Sediin.PraticheRegionali.WebUI.Controllers;
using Sediin.PraticheRegionali.WebUI.Filters;
using static Sediin.PraticheRegionali.WebUI.IdentityHelper;

namespace Sediin.PraticheRegionali.WebUI.Areas.Admin.Controllers
{
    [@Authorize(Roles = new Roles[] { Roles.Admin, Roles.Super })]
    public class DatabaseController : BaseController
    {
        public string PathBackup
        {
            get
            {

                var cartellaServer = Path.Combine(ConfigurationProvider.Instance.GetConfiguration().UploadFolder, "Backup\\Database");

                return cartellaServer;
            }

            private set { }
        }

        public ActionResult Backup()
        {
            return AjaxView();
        }

        public ActionResult ListDatabaseBackup()
        {
            List<FileInfo> files = new List<FileInfo>();

            if (Directory.Exists(PathBackup))
            {
                foreach (var item in Directory.GetFiles(PathBackup, "*.zip"))
                {
                    var f = new FileInfo(item);
                    files.Add(f);
                }
            }

            return AjaxView("ListDatabaseBackup", files);
        }

        public async Task<ActionResult> BackupDatabase()
        {
            try
            {
                if (!Directory.Exists(PathBackup))
                {
                    Directory.CreateDirectory(PathBackup);
                }

                DbProvider provider = new DbProvider();
                //provider.OnReportProgressBackupDb += Provider_OnReportProgressBackupDb;

                await provider.BackupDatabase(PathBackup);

                return JsonResultTrue("Backup creato");
            }
            catch (Exception)
            {
                throw;
            }
        }

        //private void Provider_OnReportProgressBackupDb(int percentuale, string id)
        //{
        //    Thread.Sleep(100);
        //    IHubContext context = GlobalHost.ConnectionManager.GetHubContext<SediinPraticheRegionaliHub>();
        //    context.Clients.All.onReportImportStatus(id, User.Identity.Name, "DbImport", percentuale, 100, "");
        //}

        public ActionResult Download(string filename)
        {
            try
            {
                var _f = Path.Combine(PathBackup, filename);

                return File(_f, "application/x-zip-compressed", filename);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public ActionResult Elimina(string filename)
        {
            try
            {
                var _f = Path.Combine(PathBackup, filename);

                System.IO.File.Delete(_f);

                return JsonResultTrue("Backup eliminato");
            }
            catch (Exception)
            {
                throw;
            }
        }
        public ActionResult SvuotaDatabase()
        {
            return AjaxView();
        }

        [HttpPost]
        public ActionResult SvuotaDatabase(List<string> table)
        {
            DbProvider provider = new DbProvider();
            provider.SvuotaDatabase(table);

            return JsonResultTrue("Operazione eseguita");
        }

        [HttpPost]
        public ActionResult RestoreDatabase(string db)
        {
            DbProvider provider = new DbProvider();
            provider.RestoreDatabase(db);

            return JsonResultTrue("Operazione eseguita");
        }
    }
}