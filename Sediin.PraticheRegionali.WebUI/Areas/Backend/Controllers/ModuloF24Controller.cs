using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Caching;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Web.Mvc;
using DocumentFormat.OpenXml.EMMA;
using DocumentFormat.OpenXml.Wordprocessing;
using LambdaSqlBuilder;
using Microsoft.AspNet.Identity;
using Sediin.MVC.HtmlHelpers;
using Sediin.PraticheRegionali.DOM.Entitys;
using Sediin.PraticheRegionali.WebUI.Areas.Backend.Models;
using Sediin.PraticheRegionali.WebUI.Controllers;
using Sediin.PraticheRegionali.WebUI.Filters;
using Sediin.PraticheRegionali.WebUI.Models;
using static Sediin.PraticheRegionali.WebUI.IdentityHelper;
using System.Web;
using Newtonsoft.Json;
using System.IdentityModel.Claims;
using MailKit;

namespace Sediin.PraticheRegionali.WebUI.Areas.Backend.Controllers
{
    [@Authorize]
    public class ModuloF24Controller : BaseController
    {

        public string PathProspetti { get => "Documenti\\Prospetti"; private set { } }

        [@Authorize(Roles = new Roles[] { Roles.Admin, Roles.Sp_Ebinter })]
        public async Task<ActionResult> NuovoProspetto()
        {
            try
            {
                var model = new ProspettoViewModel();
                Prospetto _prospetto = null;
                model = Reflection.CreateModel<ProspettoViewModel>(_prospetto);
                return AjaxView("Prospetto", model);
            }
            catch (Exception ex)
            {
                return AjaxView("Error", new HandleErrorInfo(ex, "ProspettoController", "NuovoProspetto"));
            }

        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        [@Authorize(Roles = new Roles[] { Roles.Admin, Roles.Sp_Ebinter })]
        public async Task<ActionResult> NuovoProspetto(ProspettoViewModel model)
        {
            try
            {
                //if (model.File_Prospetto == null)
                //    ModelState.AddModelError("File_Prospetto", "File Prospetto è obbligatorio");
                
                if (!ModelState.IsValid)
                    throw new Exception(ModelStateErrorToString(ModelState));

                Prospetto _prospetto = null;
                //cast to entity model
                var _resultModel = Reflection.CreateModel<Prospetto>(model);
                var _data = model.Data_Inserimento;

                //var fileb64 = GetBaseFileAndValid(model.File_Prospetto);
                //if (fileb64 == null)
                //    throw new Exception("Errore caricamento file");

                _resultModel.FileName = Savefile(GetUploadFolder(PathProspetti, 0), model.File_Prospetto);
                if (string.IsNullOrEmpty(_resultModel.FileName))
                    throw new Exception("Errore caricamento file");

                //MemoryStream stream = new MemoryStream();
                //using (FileStream file = new FileStream(Path.Combine(GetUploadFolder(PathProspetti, 0),model.FileName), FileMode.Open, FileAccess.Read))
                //    file.CopyTo(stream);

                //var _list = new List<Quote>();
                //using (StreamReader streamReader = new StreamReader(stream))
                //{
                //    string riga;
                //    string jstr;
                //    Quote _quota;
                //    while ((riga = streamReader.ReadLine()) != null)
                //    {
                //        jstr = riga.Replace("$\"", "\"");
                //        _quota = JsonConvert.DeserializeObject<Quote>(jstr);
                //        if (_quota != null) { _list.Add(_quota); }                       
                //    }
                //}

                //_resultModel.Quote = _list;

                unitOfWork.ProspettoRepository.InsertOrUpdate(_resultModel);
                unitOfWork.Save(false);
                return Json(new
                {
                    isValid = true,
                    prospettoId = _resultModel.ProspettoId,
                    message = "Prospetto " + (model.ProspettoId == 0 ? "inserito" : "aggiornato")
                });


            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }

        }

        [@Authorize(Roles = new Roles[] { Roles.Admin, Roles.Sp_Ebinter })]
        public async Task<ActionResult> RicercaProspetto()
        {
            ProspettoRicercaModel model = new ProspettoRicercaModel();
            return AjaxView("RicercaProspetto", model);
        }

        [HttpPost]
        [@Authorize(Roles = new Roles[] { Roles.Admin, Roles.Sp_Ebinter })]
        public ActionResult RicercaProspetto(ProspettoRicercaModel model, int? page)
        {

            int totalRows = 0;

            var _query = unitOfWork.ProspettoRepository.Get(ref totalRows, RicercaFilter2(model), model.Ordine, page, model.PageSize);

            var _result = GeModelWithPaging<ProspettoRicercaViewModel, Prospetto>(page, _query, model, totalRows, model.PageSize);
            return AjaxView("RicercaListProspetto", _result);
        }

        [@Authorize(Roles = new Roles[] { Roles.Admin, Roles.Sp_Ebinter })]
        public ActionResult RicercaExcel(ProspettoRicercaModel model)
        {
            var _query = from a in unitOfWork.ProspettoRepository.Get(RicercaFilter(model)).OrderBy(r => r.Anno)
                         select new
                         {
                             a.Anno,
                             a.Mese,
                             a.Descrizione,
                             a.Data_Inserimento,
                             a.Numero_Quote,
                             a.Importo_Totale
                         };

            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(_query, "Prospetto");
        }

        private SqlLam<Prospetto> RicercaFilter2(ProspettoRicercaModel model)
        {

            TrimAll(model);

            var f = new SqlLam<Prospetto>();



            if (!string.IsNullOrEmpty(model.ProspettoRicercaModel_Anno))
            {
                f.And(x => x.Anno == model.ProspettoRicercaModel_Anno);
            }

            return f;
        }

        private Expression<Func<Prospetto, bool>> RicercaFilter(ProspettoRicercaModel model)
        {
            int? _sportelloId = null;

            if (IsInRole(new Roles[] { Roles.Admin, Roles.Sp_Ebinter }))
            {
                _sportelloId = GetSportelloId.Value;
            }

            TrimAll(model);

            return x =>
               model.ProspettoRicercaModel_Anno != null ? x.Anno == model.ProspettoRicercaModel_Anno : true;
        }



        //var fileb64 = model.file.GetBaseFileAndValid();
        //byte[] fileData = Convert.FromBase64String(model.fileBase64);
        //Utils.SaveData(_file, fileData);

    }
}
