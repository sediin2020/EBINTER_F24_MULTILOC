using System;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using LambdaSqlBuilder;
using Sediin.PraticheRegionali.DOM.Entitys;
using Sediin.PraticheRegionali.WebUI.Areas.Admin.Models;
using Sediin.PraticheRegionali.WebUI.Areas.Backend.Controllers;
using Sediin.PraticheRegionali.WebUI.Controllers;
using Sediin.PraticheRegionali.WebUI.Filters;
using static Sediin.PraticheRegionali.WebUI.IdentityHelper;

namespace Sediin.PraticheRegionali.WebUI.Areas.Admin.Controllers
{
    [@Authorize]
    [@Authorize(Roles = new Roles[] { Roles.Admin, Roles.Super })]
    public class LogsController : BaseController
    {
        #region ricerca

        public ActionResult Ricerca()
        {
            return AjaxView();
        }

        [HttpPost]
        public ActionResult Ricerca(LogsRicercaModel model, int? page)
        {
            int totalRows = 0;
            var _query = unitOfWork.LogsRepository.Get(ref totalRows, RicercaFilter2(model), model.Ordine, page, model.PageSize);

            var _result = GeModelWithPaging<LogsRicercaViewModel, Logs>(page, _query, model, totalRows, model.PageSize);

            return AjaxView("RicercaList", _result);
        }

        public ActionResult RicercaExcel(LogsRicercaModel model)
        {
            var _query = from a in unitOfWork.LogsRepository.Get(RicercaFilter(model))
                         select new
                         {
                             a.Data,
                             a.Action,
                             a.Username,
                             a.Ruolo,
                             a.Message,
                         };

            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(_query, "ErrorLogs");
        }

        private SqlLam<Logs> RicercaFilter2(LogsRicercaModel model)
        {
            var f = new SqlLam<Logs>();

            if (!string.IsNullOrWhiteSpace(model.Username))
            {
                f.And(x => x.Username.Contains(model.Username));
            }

            if (!string.IsNullOrWhiteSpace(model.Data))
            {
                DateTime _d1 = DateTime.Now;
                DateTime.TryParse(HttpUtility.UrlDecode(model.Data), out _d1);

                DateTime? _datastart = new DateTime(_d1.Year, _d1.Month, _d1.Day, 0, 0, 0);

                DateTime? _dataend = new DateTime(_d1.Year, _d1.Month, _d1.Day, 23, 59, 59);

                f.And(x => x.Data >= _datastart && x.Data <= _dataend);
            }

            return f;
        }

        private Expression<Func<Logs, bool>> RicercaFilter(LogsRicercaModel model)
        {
            DateTime? _datastart = null;
            DateTime? _dataend = null;
            if (!string.IsNullOrWhiteSpace(model.Data))
            {
                DateTime _d1 = DateTime.Now;
                DateTime.TryParse(HttpUtility.UrlDecode(model.Data), out _d1);

                _datastart = new DateTime(_d1.Year, _d1.Month, _d1.Day, 0, 0, 0);

                _dataend = new DateTime(_d1.Year, _d1.Month, _d1.Day, 23, 59, 59);

            }

            return x => ((model.Username != null ? x.Username.Contains(model.Username) : true)
            && (_datastart.HasValue ? x.Data >= _datastart && x.Data <= _dataend : true));
        }

        #endregion

        public ActionResult Log(int id)
        {
            return AjaxView(model: unitOfWork.LogsRepository.Get(x => x.LogsId == id).FirstOrDefault());
        }
    }
}