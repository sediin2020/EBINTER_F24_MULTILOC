﻿using Sediin.PraticheRegionali.Utils;
using Sediin.PraticheRegionali.WebUI.Controllers;
using System.Linq;
using System.Web.Mvc;

namespace Sediin.PraticheRegionali.WebUI.Areas.Backend.Controllers
{
    public class MetropolitaneController : BaseController
    {
        // GET: Backend/Metropolitane
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult GetRegioni(bool? includiEstero = false)
        {
            var _c = unitOfWork.RegioniRepository.Get().Where(c => (!(bool)includiEstero ? c.CODREG != 99 : true) && c.CODREG != 0).OrderBy(o => o.DENREG).ToList();

            var _statoestero = _c.FirstOrDefault(x => x.CODREG == 99);

            if (_statoestero != null)
            {
                _c.Remove(_statoestero);
                _c.Insert(0, _statoestero);
            }

            return Json(_c, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProvince()
        {
            var regioneid = ConfigurationProvider.Instance.GetConfiguration().RegioneId;

            var codreg = unitOfWork.RegioniRepository.Get(x => x.RegioneId == regioneid).FirstOrDefault().CODREG;

            return Json(unitOfWork.ProvinceRepository.Get(x => x.CODREG == codreg).OrderBy(o => o.DENPRO), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProvinceByCodReg(int codreg, int? provinciaIdFilter = null)
        {
            return Json(unitOfWork.ProvinceRepository.Get(x => (x.CODREG == codreg)
            && (provinciaIdFilter != null ? x.ProvinciaId == (int)provinciaIdFilter.Value : true)).OrderBy(o => o.DENPRO), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetComuni(string sigpro)
        {
            if (sigpro == null)
            {
                return null;
            }


            return Json(unitOfWork.ComuniRepository.Get(x => x.SIGPRO.ToUpper() == sigpro.ToUpper()).OrderBy(o => o.DENCOM), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLocalita(string codcom)
        {
            if (codcom == null)
            {
                return null;
            }

            return Json(unitOfWork.LocalitaRepository.Get(x => x.CODCOM.ToUpper() == codcom.ToUpper()).OrderBy(o => o.CAP).ThenBy(o => o.DENLOC), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetRegioniAutocomplete(string phrase)
        {
            if (phrase == null)
            {
                return null;
            }
            var _result = unitOfWork.RegioniRepository.Get(x => x.DENREG.StartsWith(phrase)).OrderBy(o => o.DENREG);

            if (_result.Count() == 0)
            {
                return null;
            }
            return Json(_result.Select(x => new { x.RegioneId, x.DENREG }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetProvinceAutocomplete(string phrase)
        {
            if (phrase == null)
            {
                return null;
            }
            var _result = unitOfWork.ProvinceRepository.Get(x => x.DENPRO.StartsWith(phrase)).OrderBy(o => o.DENPRO);

            if (_result.Count() == 0)
            {
                return null;
            }
            return Json(_result.Select(x => new { x.ProvinciaId, x.DENPRO }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetComuniAutocomplete(string phrase)
        {
            if (phrase == null)
            {
                return null;
            }
            var _result = unitOfWork.ComuniRepository.Get(x => x.DENCOM.StartsWith(phrase)).OrderBy(o => o.DENCOM);

            if (_result.Count() == 0)
            {
                return null;
            }
            return Json(_result.Select(x => new { x.ComuneId, x.DENCOM }), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetComuniAziendeAutocomplete(string phrase)
        {
            if (phrase == null)
            {
                return null;
            }
            var _result = unitOfWork.AziendaRepository.Get(x => x.Comune != null ? x.Comune.DENCOM.StartsWith(phrase) : false).Select(x => x.Comune).Distinct();

            if (_result.Count() == 0)
            {
                return null;
            }
            return Json(_result.Select(x => new { x.ComuneId, x.DENCOM }).OrderBy(o => o.DENCOM), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetComuniSportelliAutocomplete(string phrase)
        {
            if (phrase == null)
            {
                return null;
            }
            var _result = unitOfWork.SportelloRepository.Get(x => x.Comune != null ? x.Comune.DENCOM.StartsWith(phrase) : false).Select(x => x.Comune).Distinct();

            if (_result.Count() == 0)
            {
                return null;
            }
            return Json(_result.Select(x => new { x.ComuneId, x.DENCOM }).OrderBy(o => o.DENCOM), JsonRequestBehavior.AllowGet);

        }
    }
}