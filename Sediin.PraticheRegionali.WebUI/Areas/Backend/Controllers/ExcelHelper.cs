using ClosedXML.Excel;
using Sediin.MVC.HtmlHelpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Sediin.PraticheRegionali.WebUI.Areas.Backend.Controllers
{
    public class ExcelHelper : Controller
    {
        //public ActionResult CreateExcel<T>(string json, string nome)
        //{
        //    try
        //    {
        //        nome = string.IsNullOrWhiteSpace(nome) ? "Export" : nome;

        //        if (model == null || model?.Count() == 0)
        //            return Content("Neesun record trovato");

        //        using (XLWorkbook wb = new XLWorkbook())
        //        {
        //            wb.Worksheets.Add(Reflection.ListToDataTable<T>(model.ToList()));
        //            wb.Worksheet(1)?.Columns()?.AdjustToContents();

        //            using (MemoryStream stream = new MemoryStream())
        //            {
        //                wb.SaveAs(stream);
        //                return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nome + ".xlsx");
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        return null;
        //    }
        //}

        public ActionResult CreateExcel<T>(IEnumerable<T> model, string nome)
        {
#pragma warning disable CS0168 // La variabile è dichiarata, ma non viene mai usata
            try
            {
                nome = string.IsNullOrWhiteSpace(nome) ? "Export" : nome;

                if (model == null || model?.Count() == 0)
                    return Content("Neesun record trovato");

                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(Reflection.ListToDataTable<T>(model.ToList()));
                    wb.Worksheet(1)?.Columns()?.AdjustToContents();

                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nome + ".xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
#pragma warning restore CS0168 // La variabile è dichiarata, ma non viene mai usata
        }

        public string CreateExcelBase64<T>(IEnumerable<T> model)
        {
#pragma warning disable CS0168 // La variabile è dichiarata, ma non viene mai usata
            try
            {

                if (model == null || model?.Count() == 0)
                    return "Neesun record trovato";

                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(Reflection.ListToDataTable<T>(model.ToList()));
                    wb.Worksheet(1)?.Columns()?.AdjustToContents();

                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return Convert.ToBase64String(stream.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
#pragma warning restore CS0168 // La variabile è dichiarata, ma non viene mai usata
        }

        public FileResult CreateExcel(DataTable model, string nome)
        {
            try
            {
                if (model == null)
                    return null;

                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(model);
                    wb.Worksheet(1)?.Columns()?.AdjustToContents();

                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return File(stream.ToArray(), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", nome + ".xlsx");
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }
        }

        public string CreateExcelBase64(DataTable model)
        {
            try
            {
                if (model == null)
                    return "";

                using (XLWorkbook wb = new XLWorkbook())
                {
                    wb.Worksheets.Add(model);
                    wb.Worksheet(1)?.Columns()?.AdjustToContents();

                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return Convert.ToBase64String(stream.ToArray());
                    }
                }
            }
            catch (Exception)
            {
                return "";
            }

        }

        public string CreateExcelsBase64(List<DataTable> model)
        {
#pragma warning disable CS0168 // La variabile è dichiarata, ma non viene mai usata
            try
            {
                if (model == null)
                    return "";

                using (XLWorkbook wb = new XLWorkbook())
                {
                    foreach (var item in model)
                    {
                        wb.Worksheets.Add(item);
                    }

                    wb.Worksheet(1)?.Columns()?.AdjustToContents();

                    using (MemoryStream stream = new MemoryStream())
                    {
                        wb.SaveAs(stream);
                        return Convert.ToBase64String(stream.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                return "";
            }
#pragma warning restore CS0168 // La variabile è dichiarata, ma non viene mai usata

        }
    }
}