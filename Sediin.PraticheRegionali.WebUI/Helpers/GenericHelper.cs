using ClosedXML.Excel;
using Sediin.PraticheRegionali.DOM;
using Sediin.PraticheRegionali.Utils;
using Sediin.PraticheRegionali.WebUI.Areas.Backend.Controllers;
using Sediin.PraticheRegionali.WebUI.Models;
using HtmlAgilityPack;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Sediin.PraticheRegionali.DOM.Entitys;
using Microsoft.Owin.Security.Provider;
using Sediin.PraticheRegionali.WebUI.Controllers;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Office2010.Excel;

namespace Sediin.PraticheRegionali.WebUI.Helpers
{
    public static class GenericHelper
    {
        public static ConfigurationViewData GetConfiguration(this HtmlHelper html)
        {
            return ConfigurationProvider.Instance.GetConfiguration();
        }

        public static decimal GetDecimal(this HtmlHelper html, object val)
        {
            decimal.TryParse(val?.ToString(), out decimal v);
            return v;
        }

        public static ApplicationUser GetUser(this HtmlHelper html)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            return manager.FindByName(html.ViewContext.HttpContext.User.Identity.Name);
        }

        public static IEnumerable<RolesPortale> GetRolesFriendlyName(this HtmlHelper html)
        {
            try
            {
                return ConfigurationProvider.Instance.GetConfiguration().Roles;//.Where(x => (bool)x.Attivo);

                //List<(string, string, string)> _list = new List<(string, string, string)>();

                //var manager = new ApplicationRoleManager(new RoleStore<IdentityRole>(new ApplicationDbContext()));
                //var _excludetroles = ConfigurationProvider.Instance.GetConfiguration().ExcludetRoles;
                //foreach (var role in manager.Roles)
                //{
                //    if (_excludetroles.FirstOrDefault(x => x == role.Name) != null)
                //    {
                //        continue;
                //    }
                //    _list.Add((role.Id, role.Name, GetRoleFriendlyName(html, role.Name)));
                //}

                //return _list;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string GetUserRoleFriendlyName(this HtmlHelper html)
        {
            return GetRoleFriendlyName(html, GetUserRole(html));
        }

        public static string GetRoleFriendlyName(this HtmlHelper html, string role)
        {
            return GetRolesFriendlyName(html).FirstOrDefault(x => x.Rolename == role)?.FriendlyName;
        }

        public static string GetUserRole(this HtmlHelper html)
        {
            try
            {
                var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(new ApplicationDbContext()));
                return manager.GetRoles(html.ViewContext.HttpContext.User.Identity.GetUserId()).FirstOrDefault();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static MvcHtmlString GetRichiestaStatoCss(this HtmlHelper helper, int statoId)
        {
            try
            {
                var cssStato = "primary";

                if (statoId == (int)SediinPraticheRegionaliEnums.StatoPratica.Bozza)
                {
                    cssStato = "bozza";
                }

                if (statoId == (int)SediinPraticheRegionaliEnums.StatoPratica.Revisione)
                {
                    cssStato = "warning";
                }

                if (statoId == (int)SediinPraticheRegionaliEnums.StatoPratica.Annullata)
                {
                    cssStato = "info";
                }

                if (statoId == (int)SediinPraticheRegionaliEnums.StatoPratica.Inviata || statoId == (int)SediinPraticheRegionaliEnums.StatoPratica.InviataRevisionata)
                {
                    cssStato = "primary";
                }

                if (statoId == (int)SediinPraticheRegionaliEnums.StatoPratica.Confermata)
                {
                    cssStato = "success";
                }

                return new MvcHtmlString(cssStato);
            }
            catch
            {
                return new MvcHtmlString("");
            }
        }

        public static MvcHtmlString GetLiquidazioneStatoCss(this HtmlHelper helper, int statoId)
        {
            try
            {
                var cssStato = "primary";

                if (statoId == (int)SediinPraticheRegionaliEnums.StatoLiqidazione.InLiquidazione)
                {
                    cssStato = "warning";
                }

                if (statoId == (int)SediinPraticheRegionaliEnums.StatoLiqidazione.Annullata)
                {
                    cssStato = "danger";
                }

                if (statoId == (int)SediinPraticheRegionaliEnums.StatoLiqidazione.Liquidata)
                {
                    cssStato = "success";
                }

                return new MvcHtmlString(cssStato);
            }
            catch
            {
                return new MvcHtmlString("");
            }
        }

        public static MvcHtmlString ToImporto(this HtmlHelper helper, object importo)
        {
            try
            {
                decimal.TryParse(importo?.ToString(), out decimal _imp);

                return new MvcHtmlString(_imp.ToString("n"));
            }
            catch
            {
                return new MvcHtmlString(importo.ToString());
            }
        }

        public static MvcHtmlString ToShortDate(this HtmlHelper helper, object date)
        {
            try
            {
                if (date == null)
                {
                    return new MvcHtmlString("");
                }

                DateTime.TryParse(date?.ToString(), out DateTime _date);

                return new MvcHtmlString(_date.ToShortDateString());
            }
            catch
            {
                return new MvcHtmlString(date?.ToString());
            }
        }

        public static MvcHtmlString ButtonCloseModal(this HtmlHelper helper, bool? scriptTag = true)
        {
            var x = "";

            if (scriptTag.GetValueOrDefault())
            {
                x += "<script>";
            }
            x += "if ($('.modal').length == 0) {";
            x += "$(\"<hr/>\").insertBefore($(\".modal-footer\").addClass(\"text-center\").removeClass(\"modal-footer\"));";
            x += "$(\"#buttonCloseModal\").remove();";
            x += "}";

            if (scriptTag.GetValueOrDefault())
            {
                x += "</script>";
            }

            return new MvcHtmlString(x);
        }

        public static MvcHtmlString RicercaModulo(this HtmlHelper helper, MvcHtmlString partial, string headerText, bool? updateListOnSubmit = true)
        {
            var x = "";

            x += "<div class=\"accordion\" id=\"accordionPanelsRicercae\">";
            x += "<div class=\"accordion-item\">";
            x += "<h2 class=\"accordion-header\" id=\"panelRicerca-headingOne\">";
            x += "<button class=\"accordion-button text-dark\" type=\"button\" data-bs-toggle=\"collapse\" data-bs-target=\"#panelRicerca-collapseOne\" aria-expanded=\"true\" aria-controls=\"panelRicerca-collapseOne\">";
            x += "<h4>" + headerText + "</h4>";
            x += "</button>";
            x += "</h2>";
            x += "<div id=\"panelRicerca-collapseOne\" class=\"accordion-collapse collapse show\" aria-labelledby=\"panelRicerca-headingOne\">";
            x += "<div class=\"accordion-body\">";

            var _doc = new HtmlDocument();
            _doc.LoadHtml(partial.ToHtmlString());

            try
            {
                HtmlNode _nodes = _doc.DocumentNode.Descendants()?
                    .Where(n => n.Name == "button")?
                    .Where(xx => xx.Attributes.Where(d => d.Value == "submit").Count() > 0)
                    .FirstOrDefault();

                if (_nodes != null)
                {
                    HtmlNode div = _doc.CreateElement("button");
                    div.Attributes.Append("type", "reset");
                    div.Attributes.Append("class", "btn btn-danger ml-1");
                    div.InnerHtml = "Reset modulo";
                    _nodes.ParentNode?.InsertAfter(div, _nodes);
                }
            }
            catch
            {
            }

            var _htmlPattial = _doc.DocumentNode.OuterHtml;

            x += _htmlPattial;

            x += "</div>";
            x += "</div>";
            x += "</div>";
            x += "</div>";

            x += "<div id=\"resultRicerca\" class=\"mt-3\">";

            x += "</div>";

            if (updateListOnSubmit.GetValueOrDefault())
            {
                x += "<script>var _updateListRicercaOnSubmit=false; ";
                x += "$(\"#accordionPanelsRicercae\").find(\"button[type='submit']\").on(\"click\", function () { _updateListRicercaOnSubmit=true; });";
                x += " $(\"#accordionPanelsRicercae\").find(\"button[type='reset']\").on(\"click\", function () { $(\"#accordionPanelsRicercae\").find(\"input[type='hidden']\").val(''); $(\"#accordionPanelsRicercae\").find(\".field-validation-error\").html('');  })";
                x += "</script>";
            }
            else
            {
                x += "<script>var _updateListRicercaOnSubmit=true; ";
                x += " $(\"#accordionPanelsRicercae\").find(\"button[type='reset']\").on(\"click\", function () { $(\"#accordionPanelsRicercae\").find(\"input[type='hidden']\").val(''); $(\"#accordionPanelsRicercae\").find(\".field-validation-error\").html('');  })";
                x += "</script>";
            }

            return new MvcHtmlString(x);

        }

        public static MvcHtmlString RicercaModulo(this HtmlHelper helper, string headerText, bool? updateListOnSubmit = true)
        {
            var x = "";

            x += "<div class=\"accordion\" id=\"accordionPanelsRicercae\">";
            x += "<div class=\"accordion-item\">";
            x += "<h2 class=\"accordion-header\" id=\"panelRicerca-headingOne\">";
            x += "<button class=\"accordion-button text-dark\" type=\"button\" style=\"display:block; cursor:default\">";
            x += "<h4>" + headerText + "</h4>";
            x += "</button>";
            x += "</h2>";
            x += "<div id=\"panelRicerca-collapseOne\" class=\"accordion-collapse collapse show\" aria-labelledby=\"panelRicerca-headingOne\">";

            x += "</div>";
            x += "</div>";
            x += "</div>";

            x += "<div id=\"resultRicerca\" class=\"mt-3\">";

            x += "</div>";

            if (updateListOnSubmit.GetValueOrDefault())
            {
                x += "<script>var _updateListRicercaOnSubmit=false; $(\"#accordionPanelsRicercae\").find(\"button[type='submit']\").on(\"click\", function () { _updateListRicercaOnSubmit=true; });</script>";
            }
            else
            {
                x += "<script>var _updateListRicercaOnSubmit=true;</script>";
            }

            return new MvcHtmlString(x);
        }

        public static MvcHtmlString OnSuccess(this HtmlHelper html)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("if (data.isValid)");
            sb.AppendLine("{");
            sb.AppendLine("if (typeof updateListRicerca !== 'undefined' && typeof updateListRicerca === 'function') updateListRicerca();");
            sb.AppendLine("alertSuccess(data.message);");
            sb.AppendLine("}");
            sb.AppendLine("else");
            sb.AppendLine("{");
            sb.AppendLine("alertDanger(data.message);");
            sb.AppendLine("}");

            return new MvcHtmlString(sb.ToString());

        }

        public static MvcHtmlString OnSuccessHideModal(this HtmlHelper html)
        {
            StringBuilder sb = new StringBuilder();

            sb.AppendLine("if (data.isValid)");
            sb.AppendLine("{");
            sb.AppendLine("if (typeof updateListRicerca !== 'undefined' && typeof updateListRicerca === 'function') updateListRicerca();");
            sb.AppendLine("hideModal();");
            sb.AppendLine("alertSuccess(data.message);");
            sb.AppendLine("}");
            sb.AppendLine("else");
            sb.AppendLine("{");
            sb.AppendLine("alertDanger(data.message);");
            sb.AppendLine("}");

            return new MvcHtmlString(sb.ToString());

        }

        public static MvcHtmlString UpdateListRicerca(this HtmlHelper html, string action, bool? includeScripttag = true)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("function updateListRicerca(closeAlert){");
            sb.Append("if (_updateListRicercaOnSubmit==true){ var _s = $(\"form[action='" + action + "']\").serialize();");

            sb.Append("$.post(\"" + action + "?page=\" + $(\"#sediinmvcpaging\").val(), _s, function(data) {");
            sb.Append("$(\"#resultRicerca\").html(data);");
            sb.Append("if (closeAlert==true){ alertClose(); }");
            sb.Append("});}}");

            var _s = sb.ToString();
            if (includeScripttag.GetValueOrDefault())
            {
                _s = "<script>" + sb.ToString() + "</script>";
            }

            return new MvcHtmlString(_s);
        }

        public static MvcHtmlString AutocompleteAzienda(this HtmlHelper helper, string nominaticoField, string cd_matricolaField, string dataValmsgFor, int? provinciaId = null, int? sportelloId = null, int? dipendenteId = null, string callBackFunction = null, string autoCompleteKey = "autocompleteListaAziende", int? width = null, int? maxNumberOfElements = 6)
        {
            var _dic = new Dictionary<string, string>();

            if (provinciaId.GetValueOrDefault() != 0)
            {
                _dic.Add("provinciaId", provinciaId.ToString());
            }

            if (sportelloId.GetValueOrDefault() != 0)
            {
                _dic.Add("sportelloId", sportelloId.ToString());
            }

            if (dipendenteId.GetValueOrDefault() != 0)
            {
                _dic.Add("dipendenteId", dipendenteId.ToString());
            }

            return Autocomplete(helper, nominaticoField, cd_matricolaField, dataValmsgFor, autoCompleteKey, "RagioneSociale", "AziendaId", "/Backend/Azienda/ListaAziende", "phrase", callBackFunction, width, maxNumberOfElements, _dic);
        }

        public static MvcHtmlString AutocompleteDipendente(this HtmlHelper helper, string nominaticoField, string cd_matricolaField, string dataValmsgFor, int? sportelloId = null, string callBackFunction = null, string autoCompleteKey = "autocompleteListaDipendente", int? width = null, int? maxNumberOfElements = 6)
        {
            var _dic = new Dictionary<string, string>();

            if (sportelloId.GetValueOrDefault() != 0)
            {
                _dic.Add("sportelloId", sportelloId.ToString());
            }

            return Autocomplete(helper, nominaticoField, cd_matricolaField, dataValmsgFor, autoCompleteKey, "Nominativo", "DipendenteId", "/Backend/Dipendente/ListaDipendenti", "phrase", callBackFunction, width, maxNumberOfElements, _dic);
        }

        public static MvcHtmlString AutocompleteUtente(this HtmlHelper helper, string nominaticoField, string cd_matricolaField, string dataValmsgFor, string callBackFunction = null, string autoCompleteKey = "autocompleteListaUtente", int? width = null, int? maxNumberOfElements = 6)
        {
            return Autocomplete(helper, nominaticoField, cd_matricolaField, dataValmsgFor, autoCompleteKey, "Email", "UserName", "/Backend/Utenti/ListaUtenti", "phrase", callBackFunction, width, maxNumberOfElements);
        }

        public static MvcHtmlString AutocompleteSportello(this HtmlHelper helper, string nominaticoField, string cd_matricolaField, string dataValmsgFor, string callBackFunction = null, string autoCompleteKey = "autocompleteListaSportello", int? width = null, int? maxNumberOfElements = 6)
        {
            return Autocomplete(helper, nominaticoField, cd_matricolaField, dataValmsgFor, autoCompleteKey, "Nominativo", "SportelloId", "/Backend/Sportello/ListaSportelli", "phrase", callBackFunction, width, maxNumberOfElements);
        }

        public static MvcHtmlString AutocompleteComuniSportello(this HtmlHelper helper, string textField, string valueField, string dataValmsgFor, string callBackFunction = null, string autoCompleteKey = "autocompleteGetComuniSportelliAutocomplete", int? width = null, int? maxNumberOfElements = 6)
        {
            return Autocomplete(helper, textField, valueField, dataValmsgFor, autoCompleteKey, "DENCOM", "ComuneId", "/Backend/Metropolitane/GetComuniSportelliAutocomplete", "phrase", callBackFunction, width, maxNumberOfElements);
        }

        public static MvcHtmlString AutocompleteComuni(this HtmlHelper helper, string textField, string valueField, string dataValmsgFor, string callBackFunction = null, string autoCompleteKey = "autocompleteGetComuniAutocomplete", int? width = null, int? maxNumberOfElements = 6)
        {
            return Autocomplete(helper, textField, valueField, dataValmsgFor, autoCompleteKey, "DENCOM", "ComuneId", "/Backend/Metropolitane/GetComuniAutocomplete", "phrase", callBackFunction, width, maxNumberOfElements);
        }
        public static MvcHtmlString AutocompleteRegioni(this HtmlHelper helper, string textField, string valueField, string dataValmsgFor, string callBackFunction = null, string autoCompleteKey = "autocompleteGetRegioniAutocomplete", int? width = null, int? maxNumberOfElements = 6)
        {
            return Autocomplete(helper, textField, valueField, dataValmsgFor, autoCompleteKey, "DENREG", "RegioneId", "/Backend/Metropolitane/GetRegioniAutocomplete", "phrase", callBackFunction, width, maxNumberOfElements);
        }
        public static MvcHtmlString AutocompleteProvince(this HtmlHelper helper, string textField, string valueField, string dataValmsgFor, string callBackFunction = null, string autoCompleteKey = "autocompleteGetRegioniAutocomplete", int? width = null, int? maxNumberOfElements = 6)
        {
            return Autocomplete(helper, textField, valueField, dataValmsgFor, autoCompleteKey, "DENPRO", "ProvinciaId", "/Backend/Metropolitane/GetProvinceAutocomplete", "phrase", callBackFunction, width, maxNumberOfElements);
        }

        public static MvcHtmlString AutocompleteComuniAzienda(this HtmlHelper helper, string textField, string valueField, string dataValmsgFor, string callBackFunction = null, string autoCompleteKey = "autocompleteGetComuniAziendeAutocomplete", int? width = null, int? maxNumberOfElements = 6)
        {
            return Autocomplete(helper, textField, valueField, dataValmsgFor, autoCompleteKey, "DENCOM", "ComuneId", "/Backend/Metropolitane/GetComuniAziendeAutocomplete", "phrase", callBackFunction, width, maxNumberOfElements);
        }

        public static MvcHtmlString Autocomplete(this HtmlHelper helper, string textField, string valueField, string dataValmsgFor, string autoCompleteKey, string returnText, string returnValue, string action, string parameter, string callBackFunction = null, int? width = null, int? maxNumberOfElements = 6, Dictionary<string, string> para = null)
        {
            StringBuilder sb = new StringBuilder();

            //sb.Append("<script>");
            //sb.Append("removejscssfile('jquery.easy-autocomplete.min.js', 'js')");
            //sb.Append("</script>");

            sb.Append("<link href='/Content/easy-autocomplete/easy-autocomplete.css' rel='stylesheet'>");
            sb.Append("<script src='/Scripts/jquery.easy-autocomplete.min.js'></script>");

            sb.Append("<script>");

            sb.Append("$('#" + textField + "').focus(function() {");
            sb.Append("$('#" + textField + "').on('click', function(e) {");
            sb.Append("$('#" + textField + "').select();");
            sb.Append("});");
            sb.Append("});");

            sb.Append("var _checkField_" + autoCompleteKey + "='';");
            sb.Append("var _valField_" + autoCompleteKey + "='';");
            sb.Append("$('#" + textField + "').change(function() {");
            sb.Append("if (String($('#" + textField + "').val()).toUpperCase() != String(_checkField_" + autoCompleteKey + ").toUpperCase()) {");
            sb.Append("$('#" + valueField + "').val('');");

            if (!string.IsNullOrWhiteSpace(callBackFunction))
            {
                //sb.Append("if (_valField_" + autoCompleteKey + " != myval){");
                sb.Append(callBackFunction + "('');");
                //sb.Append("}");
            }

            sb.Append("}");
            sb.Append("});");

            sb.Append("autocomplete_" + autoCompleteKey + "('#" + textField + "', '#" + valueField + "');");

            var _params = "";
            if (para != null)
            {
                foreach (var item in para)
                {
                    _params += $"&{item.Key}=" + HttpUtility.UrlEncode(item.Value) + "&";
                }
            }
            if (_params != "")
            {
                _params = _params?.Substring(1);
            }

            sb.Append("function autocomplete_" + autoCompleteKey + "(input, input2)");
            sb.Append("{");
            sb.Append("var options = {");
            sb.Append("url: function(phrase) {");
            sb.Append("return '" + action + "?" + _params + parameter + "='+phrase;");
            //sb.Append("return '/Home/ListaMatricole?Nominativo='+phrase;");
            sb.Append("},");
            //sb.Append("getValue: 'NOMINATIVO',");

            sb.Append("getValue: function(element) {");
            sb.Append("return element." + returnText + ";");
            sb.Append("},");

            sb.Append("dataValueFiled: '" + returnValue + "',");
            sb.Append("requestDelay: 350,");

            sb.Append("list:");
            sb.Append("{");

            sb.Append("maxNumberOfElements:" + maxNumberOfElements + ", ");

            //sb.Append("onSelectItemEvent:function(){ setMatricolaRicerca(input, input2) },");
            sb.Append("onClickEvent:function(){ setValue_" + autoCompleteKey + "(input, input2) },");
            sb.Append("onChooseEvent:function(){ setValue_" + autoCompleteKey + "(input, input2) },");

            sb.Append("}");
            sb.Append("};");

            sb.Append("$(input).easyAutocomplete(options);");
            //sb.Append("$('.easy-autocomplete').css('width', '');");

            if (width.HasValue)
            {
                sb.Append("$('.easy-autocomplete').css('width', '" + width + "px');");
            }
            else
            {
                sb.Append("$('.easy-autocomplete').css('width', '');");
            }

            sb.Append("}");

            sb.Append("function setValue_" + autoCompleteKey + "(input, input2){");

            sb.Append("$('[data-valmsg-for=\"" + dataValmsgFor + "\"]').removeClass('field-validation-error');");
            sb.Append("$('[data-valmsg-for=\"" + dataValmsgFor + "\"]').html('');");

            //va in errore? sb.Append("var myval = $(input).getSelectedItemData()." + returnValue + ";");
            sb.Append("var myval = $('#eac-container-' + String(input).replace('#', '') + ' ul li.selected div').data('eacvalue');");
            sb.Append("$(input2).val(myval);");

            //if (!string.IsNullOrWhiteSpace(callBackFunction))
            //{
            //    sb.Append("if (_valField_" + autoCompleteKey + " != myval){");
            //    sb.Append(callBackFunction + "(myval);}");
            //}

            if (!string.IsNullOrWhiteSpace(callBackFunction))
            {
                // sb.Append("if (_valField_" + autoCompleteKey + " != myval){");
                sb.Append("if (myval != ''){");
                sb.Append(callBackFunction + "(myval);");
                sb.Append("}");
            }

            sb.Append("_valField_" + autoCompleteKey + "=myval;");
            sb.Append("_checkField_" + autoCompleteKey + " = String($(input).val()).toUpperCase();");

            sb.Append("}");

            sb.Append("</script>");


            return new MvcHtmlString(sb.ToString());
        }


        public static int? GetSportelloId(this HtmlHelper helper)
        {
            BaseController controller = new BaseController();
            return controller.GetSportelloId.Value;
        }

        public static MvcHtmlString Si_No(this HtmlHelper helper, object value)
        {
            if (value != null)
            {
                if ((bool)value == true)
                {
                    return MvcHtmlString.Create("<i class=\"fas fa-check text-success\"></i>");
                }

                return MvcHtmlString.Create("<i class=\"fas fa-times text-danger\"></i>");

            }

            return MvcHtmlString.Create("-");
        }

    }
}