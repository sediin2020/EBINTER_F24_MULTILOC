using System;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.SignalR;
using Sediin.PraticheRegionali.DOM.Entitys;
using Sediin.PraticheRegionali.Utils;
using Sediin.PraticheRegionali.WebUI.Areas.Backend.Models;
using Sediin.PraticheRegionali.WebUI.Controllers;
using Sediin.PraticheRegionali.WebUI.Filters;
using Sediin.PraticheRegionali.WebUI.Helpers;
using Sediin.PraticheRegionali.WebUI.Hubs;
using Sediin.PraticheRegionali.WebUI.Models;
using static Sediin.PraticheRegionali.WebUI.IdentityHelper;

namespace Sediin.PraticheRegionali.WebUI.Areas.Backend.Controllers
{
    [@Authorize]
    public class UtentiController : BaseController
    {
        [@Authorize(Roles = new Roles[] { Roles.Admin, Roles.Super })]
        public ActionResult Ricerca()
        {
            return AjaxView("Ricerca");
        }

        [@Authorize(Roles = new Roles[] { Roles.Admin, Roles.Super })]
        [HttpPost]
        public ActionResult Ricerca(UtentiRicercaModel model, int? page)
        {
            try
            {
                var _query = UserManager.Users.Where(RicercaFilter(model)).Select(x => new UtentiViewModel
                {
                    UserId = x.Id,
                    UserName = x.UserName,
                    Cognome = x.Cognome,
                    Email = x.Email,
                    Nome = x.Nome,
                    EmailConfermata = x.EmailConfirmed,
                    RuoloId = x.Roles.FirstOrDefault().RoleId,
                    Bloccato = x.LockoutEndDateUtc != null
                }).OrderBy(HttpUtility.UrlDecode(model.UtentiRicercaModel_OrderBy));

                var _result = GeModelWithPaging<UtentiRicercaViewModel, UtentiViewModel>(page, _query.AsEnumerable(), model, 10);

                var _roles = ConfigurationProvider.Instance.GetConfiguration().Roles;

                foreach (var item in _result.Result)
                {
                    item.Ruolo = _roles.FirstOrDefault(x => x.RoleId == item.RuoloId).Rolename;
                    item.RuoloFriendlyName = _roles.FirstOrDefault(x => x.RoleId == item.RuoloId).FriendlyName;
                }

                return AjaxView("RicercaList", _result);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [@Authorize(Roles = new Roles[] { Roles.Admin, Roles.Super })]
        public ActionResult UtenteNuovo(string ruolo)
        {
            try
            {
                UtentiViewModel _user = new UtentiViewModel();

                var _ruolo = GenericHelper.GetRolesFriendlyName(null).FirstOrDefault(x => x.Rolename == ruolo);
                _user.Ruolo = _ruolo.Rolename;
                _user.RuoloFriendlyName = _ruolo.FriendlyName;
                _user.AssociaProvincia = false;


                //_ruolo.Rolename == Roles.Sp_Sindacale.ToString()
                //|| _ruolo.Rolename == Roles.Sp_Datoriale.ToString()
                //|| _ruolo.Rolename == Roles.Sp_Ebinter.ToString();

                if (_user.AssociaProvincia.GetValueOrDefault())
                {
                    var regioneid = ConfigurationProvider.Instance.GetConfiguration().RegioneId;

                    var codreg = unitOfWork.RegioniRepository.Get(x => x.RegioneId == regioneid).FirstOrDefault().CODREG;

                    _user.Provincie = unitOfWork.ProvinceRepository.Get(x => x.CODREG == codreg).OrderBy(o => o.DENPRO);
                }

                return AjaxView("Utente", _user);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [@Authorize(Roles = new Roles[] { Roles.Admin, Roles.Super })]
        public ActionResult Utente(string id)
        {
            try
            {
                UtentiViewModel _user = UserManager.Users.Where(x => x.Id == id).Select(x => new UtentiViewModel
                {
                    UserId = x.Id,
                    UserName = x.UserName,
                    Cognome = x.Cognome,
                    Nome = x.Nome,
                    Email = x.Email,
                    RuoloId = x.Roles.FirstOrDefault().RoleId,
                    Bloccato = x.LockoutEndDateUtc != null,
                    EmailConfermata = x.EmailConfirmed,
                    Ruolo = RoleManager.Roles.FirstOrDefault(c => c.Id == x.Roles.FirstOrDefault().RoleId).Name,
                    ProvinciaId = x.ProvinciaId
                }).FirstOrDefault();

                _user.AssociaProvincia = _user.Ruolo == Roles.Sp_Sindacale.ToString()
                || _user.Ruolo == Roles.Sp_Datoriale.ToString()
                || _user.Ruolo == Roles.Sp_Ebinter.ToString();

                if (_user.AssociaProvincia.GetValueOrDefault())
                {
                    var regioneid = ConfigurationProvider.Instance.GetConfiguration().RegioneId;

                    var codreg = unitOfWork.RegioniRepository.Get(x => x.RegioneId == regioneid).FirstOrDefault().CODREG;

                    _user.Provincie = unitOfWork.ProvinceRepository.Get(x => x.CODREG == codreg).OrderBy(o => o.DENPRO);
                }

                return AjaxView("Utente", _user);
            }
            catch (Exception)
            {
                throw;
            }
        }

        [@Authorize(Roles = new Roles[] { Roles.Admin, Roles.Super })]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Utente(UtentiViewModel model)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.UserId))
                {
                    ModelState.Remove("Password");
                    ModelState.Remove("ConfirmPassword");
                }

                if (!model.AssociaProvincia.GetValueOrDefault())
                {
                    ModelState.Remove("ProvinciaId");
                }

                //update user
                if (!string.IsNullOrWhiteSpace(model.UserId))
                {
                    var _user = UserManager.Users.FirstOrDefault(x => x.Id == model.UserId) ?? throw new Exception("Utente non trovato");
                    _user.Nome = model.Nome;
                    _user.Email = model.Email;
                    _user.Cognome = model.Cognome;
                    _user.ProvinciaId = model.ProvinciaId;
                    if (IsInRole(new Roles[] { Roles.Admin, Roles.Super }))
                    {
                        if (!model.EmailConfermata.GetValueOrDefault())
                        {
                            _user.EmailConfirmed = false;
                        }
                        else
                        {
                            _user.EmailConfirmed = model.EmailConfermata.GetValueOrDefault();
                        }
                    }

                    if (!model.Bloccato.GetValueOrDefault())
                    {
                        _user.LockoutEndDateUtc = null;
                    }
                    else
                    {
                        _user.LockoutEndDateUtc = DateTime.MaxValue;
                        IHubContext context = GlobalHost.ConnectionManager.GetHubContext<SediinPraticheRegionaliHub>();
                        context.Clients.All.onLogOffUtente(model.UserName, "Attenzione, sei stato bloccato dal Amministratore del sistema");
                    }

                    await UserManager.UpdateAsync(_user);

                    return JsonResultTrue("Utente aggiornato");
                }
                else
                {
                    var _ruolo = GenericHelper.GetRolesFriendlyName(null).FirstOrDefault(x => x.Rolename == model.Ruolo);

                    //crea utente
                    var user = new ApplicationUser
                    {
                        UserName = model.UserName,
                        Email = model.Email,
                        Cognome = model.Cognome,
                        Nome = model.Nome,
                        ProvinciaId = model.ProvinciaId
                    };

                    var result = await UserManager.CreateAsync(user, model.Password);

                    if (result.Succeeded)
                    {
                        if (!RoleManager.RoleExists(_ruolo.Rolename))
                        {
                            RoleManager.Create(new IdentityRole
                            {
                                Name = _ruolo.Rolename,
                                Id = _ruolo.RoleId
                            });
                        }

                        //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        UserManager.AddToRole(user.Id, _ruolo.Rolename);

                        // Per altre informazioni su come abilitare la conferma dell'account e la reimpostazione della password, vedere https://go.microsoft.com/fwlink/?LinkID=320771
                        // Inviare un messaggio di posta elettronica con questo collegamento
                        string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);

                        //var callbackUrl = Url.Action("ConfirmEmail", "Registrazione", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);

                        NameValueCollection c = HttpUtility.ParseQueryString(string.Empty);
                        c.Add("userId", user.Id);
                        c.Add("code", code);

                        var callbackUrl = $"{UriPortale("ConfirmEmail", "Registrazione")}?{c.ToString()}";

                        RegistrazioneConfermaModel _resultModel = new RegistrazioneConfermaModel
                        {
                            UrlConferma = callbackUrl,
                            Email = model.Email,
                            Cognome = model.Cognome,
                            Nome = model.Nome,
                            Username = model.UserName
                        };

                        await UserManager.SendEmailAsync(user.Id, "Conferma account", RenderTemplate("Registrazione/Confirm_Mail", _resultModel));

                        return JsonResultTrue("Utente creato");
                    }
                    else
                    {
                        return JsonResultFalse(ErrorsToString(result.Errors));
                    }
                }
            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        [@Authorize(Roles = new Roles[] { Roles.Admin, Roles.Super })]
        public ActionResult RicercaExcel(UtentiRicercaModel model)
        {
            var _query = UserManager.Users.Where(RicercaFilter(model)).Select(x => new
            {
                x.UserName,
                x.Cognome,
                x.Email,
                x.Nome,
                Ruolo = RoleManager.Roles.FirstOrDefault(m => m.Id == x.Roles.FirstOrDefault().RoleId).Name,
                EmailConfermata = x.EmailConfirmed ? "Si" : "No",
                Bloccato = x.LockoutEndDateUtc != null ? "Si" : "No"
            });

            ExcelHelper _excel = new ExcelHelper();
            return _excel.CreateExcel(_query, "Utenti");
        }

        [@Authorize(Roles = new Roles[] { Roles.Admin, Roles.Super })]
        [HttpPost]
        public async Task<ActionResult> EliminaUtente(string id)
        {
            try
            {
                var _user = UserManager.FindById(id);
                var _username = _user.UserName;
                var _result = await UserManager.DeleteAsync(_user);

                if (_result == IdentityResult.Success)
                {
                    IHubContext context = GlobalHost.ConnectionManager.GetHubContext<SediinPraticheRegionaliHub>();
                    context.Clients.All.onLogOffUtente(_username, "Attenzione, la tua utenza e stata eliminata dal Amministratore del sistema");
                    return JsonResultTrue("Utente eliminato");
                }

                return JsonResultFalse(ErrorsToString(_result.Errors));

            }
            catch (Exception ex)
            {
                return JsonResultFalse(ex.Message);
            }
        }

        [@Authorize(Roles = new Roles[] { Roles.Admin, Roles.Super })]
        private Expression<Func<ApplicationUser, bool>> RicercaFilter(UtentiRicercaModel model)
        {
            var _rolesVisibili = ConfigurationProvider.Instance.GetConfiguration().Roles
                .Where(x => (model.UtentiRicercaModel_RuoId != null ? x.Rolename == model.UtentiRicercaModel_RuoId : true) 
                && (bool)x.Attivo).Select(v => v.RoleId);

            TrimAll(model);

            return x => (model.UtentiRicercaModel_Username != null ? x.UserName.ToUpper().Contains(model.UtentiRicercaModel_Username.ToUpper()) : true)
            && (x.Roles.Where(c => _rolesVisibili.Contains(c.RoleId)).Count() > 0)
            //&& (model.UtentiRicercaModel_RuoId != null ? x.Roles.FirstOrDefault().RoleId == model.UtentiRicercaModel_RuoId : true)
            && (model.UtentiRicercaModel_Email != null ? x.Email.Contains(model.UtentiRicercaModel_Email) : true)
            && (model.UtentiRicercaModel_EmailConfermata != null ? (model.UtentiRicercaModel_EmailConfermata == "1" ? x.EmailConfirmed : !x.EmailConfirmed) : true)
            && (model.UtentiRicercaModel_Bloccato != null ? (model.UtentiRicercaModel_Bloccato == "1" ? x.LockoutEndDateUtc != null : x.LockoutEndDateUtc == null) : true);
        }

        public ActionResult ModificaPassword()
        {
            return AjaxView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ModificaPassword(UtentiModificaPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                throw new Exception(ModelStateErrorToString(ModelState));
            }

            var _user = UserManager.Users.FirstOrDefault(x => x.UserName == User.Identity.Name);

            if (_user == null)
            {
                throw new Exception("Utente non valido");
            }

            var _result = UserManager.ChangePassword(_user.Id, model.PasswordVecchia, model.PasswordNuova);

            if (_result.Succeeded)
            {
                return JsonResultTrue("Password cambiata");
            }
            else
            {
                return JsonResultFalse(ErrorsToString(_result.Errors));
            }
        }

        public JsonResult ListaUtenti(string phrase)
        {
            Expression<Func<Utente, bool>> _filter = x =>
            (phrase != null ? (x.Email).Contains(phrase) : true);

            return GetListaUtenti(_filter);
        }

        private JsonResult GetListaUtenti(Expression<Func<Utente, bool>> filter)
        {
            var _result = unitOfWork.UtentiRepository.Get(filter);

            if (_result.Count() > 0)
            {
                return Json(_result
                       .OrderBy(p => p.Email == null || p.Email == "")
                       .Select(x => new { x.UserName, x.Email, x.Cognome, x.Nome, Nominativo = x.Cognome + " " + x.Nome }), JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("", JsonRequestBehavior.AllowGet);
            }
        }



    }
}