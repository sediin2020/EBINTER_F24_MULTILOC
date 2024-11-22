using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using LambdaSqlBuilder;
using Newtonsoft.Json;
using Sediin.PraticheRegionali.DOM.Entitys;
using Sediin.PraticheRegionali.DOM.Models;
using Sediin.PraticheRegionali.Utils;
using Sediin.PraticheRegionali.WebUI.Areas.Backend.Models;
using Sediin.PraticheRegionali.WebUI.Controllers;
using Sediin.PraticheRegionali.WebUI.Filters;
using static Sediin.PraticheRegionali.WebUI.IdentityHelper;

namespace Sediin.PraticheRegionali.WebUI.Areas.Backend.Controllers
{
    [@Authorize(Roles = new Roles[] { Roles.Admin, Roles.Super, Roles.Sp_CAF, Roles.Sp_Sindacale, Roles.Sp_Consulente, Roles.Sp_Datoriale, Roles.Sp_Ebinter, Roles.Azienda })]
    public class UniemensController : BaseController
    {
        #region ricerca

        public ActionResult Ricerca()
        {
            int? sportelloId = null;

            if (GetSportelloId.HasValue)
            {
                sportelloId = (int)GetSportelloId.Value;
            }

            UniemensRicercaModel model = new UniemensRicercaModel { UniemensRicercaModel_SportelloId = sportelloId };
            
            if (IsInRole(Roles.Azienda))
            {
                if (GetAziendaId.HasValue)
                {
                    model.UniemensRicercaModel_AziendaId = (int)GetAziendaId.Value;
                }
            }

            return AjaxView("Ricerca", model);
        }

        [HttpPost]
        public ActionResult Ricerca(UniemensRicercaModel model, int? page)
        {
            int totalRows = 0;
            int? sportelloId = null;

            if (!string.IsNullOrEmpty(model.UniemensRicercaModel_RagioneSociale) && !model.UniemensRicercaModel_AziendaId.HasValue)
            {
                throw new Exception("Azienda non trovata! Riprovare");
            }

            if (GetSportelloId.HasValue)
            {
                sportelloId = (int)GetSportelloId.Value;

                if (!string.IsNullOrEmpty(model.UniemensRicercaModel_NominativoDipendente) && !model.UniemensRicercaModel_DipendenteId.HasValue)
                {
                    throw new Exception("Dipendente non trovato! Riprovare");
                }
            }

            var _query = unitOfWork.UniemensRepository.Get(ref totalRows, RicercaFilter2(model), model.Ordine, null, page, model.PageSize, sportelloId, model.UniemensRicercaModel_DipendenteId);

            var _result = GeModelWithPaging<UniemensRicercaViewModel, Uniemens>(page, _query, model, totalRows, model.PageSize);
            return AjaxView("RicercaList", _result);
        }

        public ActionResult RicercaExcel(UniemensRicercaModel model)
        {
            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(RicercaDataTable(model), "Contributi");
        }

        private SqlLam<Uniemens> RicercaFilter2(UniemensRicercaModel model)
        {
            var f = new SqlLam<Uniemens>();

            if (model.UniemensRicercaModel_AziendaId.HasValue)
            {
                f.And(x => x.AziendaId == model.UniemensRicercaModel_AziendaId);
            }

            if (model.UniemensRicercaModel_Anno.HasValue)
            {
                f.And(x => x.Anno == model.UniemensRicercaModel_Anno);
            }

            if (model.UniemensRicercaModel_UniemensId.HasValue)
            {
                f.And(x => x.UniemensId == model.UniemensRicercaModel_UniemensId);
            }

            return f;
        }

        private Expression<Func<Uniemens, bool>> RicercaFilter(UniemensRicercaModel model)
        {
            var _issportello = IsInRole(new Roles[] { Roles.Sp_Consulente, Roles.Sp_CAF, Roles.Sp_Sindacale, Roles.Sp_Datoriale, Roles.Sp_Ebinter });

            var _idaz = new List<int>();

            if (model.UniemensRicercaModel_UniemensId == null)
            {
                if (model.UniemensRicercaModel_AziendaId != null)
                {
                    _idaz.Add(model.UniemensRicercaModel_AziendaId.Value);
                }
                else
                {
                    var _dip = unitOfWork.DipendenteRepository.Get(x => _issportello ? x.SportelloId == (int)GetSportelloId.Value : true)
                            .Where(x => _issportello && model.UniemensRicercaModel_DipendenteId != null ? x.DipendenteId == model.UniemensRicercaModel_DipendenteId : true);

                    foreach (var item in _dip)
                    {
                        var _azDip = unitOfWork.DipendenteAziendaRepository.Get(x => x.DipendenteId == item.DipendenteId).FirstOrDefault();
                        if (_azDip != null) { _idaz.Add(_azDip.AziendaId); }
                    }
                }

                return x => ((_issportello ? (model.UniemensRicercaModel_DipendenteId != null ? (_idaz.Contains(x.AziendaId)) : (x.Azienda.SportelloId == (int)GetSportelloId.Value) || (_idaz.Contains(x.AziendaId))) : true)
                && (model.UniemensRicercaModel_AziendaId != null ? x.AziendaId == model.UniemensRicercaModel_AziendaId : true)
                && (model.UniemensRicercaModel_Anno != null ? x.Anno == model.UniemensRicercaModel_Anno : true)
                && (model.UniemensRicercaModel_UniemensId != null ? x.UniemensId == model.UniemensRicercaModel_UniemensId : true));
            }

            return x => x.UniemensId == model.UniemensRicercaModel_UniemensId;
        }


        DataTable RicercaDataTable(UniemensRicercaModel model, string mese = null)
        {
            DataTable table = new DataTable("Grid");

            var rowIndex = 0;

            foreach (var item in unitOfWork.UniemensRepository.Get(RicercaFilter(model)))
            {
                var uniemensModel = JsonConvert.DeserializeObject<UniemensModel>(item.UniemensBson);

                //crea DataTable
                if (rowIndex == 0)
                {
                    var uniemens = uniemensModel.mensilita.FirstOrDefault();

                    table.Columns.Add("Ragione sociale");
                    table.Columns.Add("Matricola Inps");
                    table.Columns.Add("Dipendente");
                    table.Columns.Add("Codice Fiscale");
                    table.Columns.Add("Mese", typeof(int));
                    table.Columns.Add("Anno", typeof(int));
                    table.Columns.Add("Versamenti", typeof(decimal));
                    table.Columns.Add("Movimenti", typeof(decimal));
                    table.Columns.Add("Imponibile", typeof(decimal));

                    foreach (var colonne in ConfigurationProvider.Instance.GetConfiguration().Uniemens.Colonna)
                    {
                        table.Columns.Add(colonne, typeof(decimal));
                    }

                    rowIndex++;
                }

                var _dovutiDipendente = uniemensModel.mensilita.Select(x => x.dovuti).ToArray();

                foreach (var mensilita in uniemensModel.mensilita.Where(x => mese != null ? x.mese == mese : true))
                {
                    for (int i = 0; i < mensilita.dovuti.Count(); i++)
                    {
                        var _dov = mensilita.dovuti[i];
                        DataRow row = table.NewRow();
                        row["Ragione sociale"] = $"{item.Azienda?.RagioneSociale}";
                        row["Matricola Inps"] = $"{item.Azienda?.MatricolaInps}";
                        row["Dipendente"] = $"{_dov.cognome} {_dov.nome}";
                        row["Codice Fiscale"] = _dov.codice_fiscale;

                        row["Mese"] = mensilita.mese;
                        row["Anno"] = item.Anno;
                        row["Versamenti"] = mensilita.totali.entrate;
                        row["Movimenti"] = mensilita.totali.movimenti;
                        row["Imponibile"] = _dov.imponibile;

                        foreach (var colonne1 in ConfigurationProvider.Instance.GetConfiguration().Uniemens.Colonna)
                        {
                            decimal _importo = 0;

                            if (_dov.quote?.FirstOrDefault(x => x.quota == colonne1) != null)
                            {
                                decimal.TryParse(_dov.quote.FirstOrDefault(x => x.quota == colonne1)?.importo?.ToString(), out decimal import);

                                _importo = import;
                            }

                            row[colonne1] = _importo;
                        }

                        table.Rows.Add(row);
                    }
                }
            }

            return table;
        }

        #endregion

        public ActionResult Dipendenti(int uniemensId, string mese)
        {
            UniemensMensilitaViewModel model = new UniemensMensilitaViewModel();
            model.Uniemens = unitOfWork.UniemensRepository.Get(x => x.UniemensId == uniemensId).FirstOrDefault();
            model.Mensilita = JsonConvert.DeserializeObject<UniemensModel>(model.Uniemens?.UniemensBson)?.mensilita?.Where(x => x.mese == mese);
            return AjaxView("Dipendenti", model);
        }

        public ActionResult DipendentiExcel(int uniemensId, string mese)
        {
            UniemensRicercaModel model = new UniemensRicercaModel();
            model.UniemensRicercaModel_UniemensId = uniemensId;

            var _uniemens = unitOfWork.UniemensRepository.Get(RicercaFilter(model)).FirstOrDefault();
            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(RicercaDataTable(model, mese), $"Contributi_Dipendenti_{_uniemens.Azienda.MatricolaInps}_{mese}_{_uniemens.Anno}");
        }
    }
}