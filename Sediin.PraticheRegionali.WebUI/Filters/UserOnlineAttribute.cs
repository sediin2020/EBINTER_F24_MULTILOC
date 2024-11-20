using Sediin.PraticheRegionali.DOM.DAL;
using Sediin.PraticheRegionali.DOM.Entitys;
using Sediin.PraticheRegionali.WebUI.Controllers;
using Sediin.PraticheRegionali.WebUI.Hubs;
using Sediin.PraticheRegionali.WebUI.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.ConstrainedExecution;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Windows.Input;
using System.Net.Http;
using System.Net;

namespace Sediin.PraticheRegionali.WebUI.Filters
{
    /// <summary>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class UserOnlineAttribute : ActionFilterAttribute
    {
        public static List<(string, DateTime)> Useronline;

        //static object _lock = new object();

        void LogUser(ActionExecutingContext filtercontext)
        {
            Task.Run(async () =>
            {
                try
                {
                    HttpContextBase httpContext = filtercontext.HttpContext;

                    if (httpContext.Request.Path.ToUpper().Contains("Metropolitane".ToUpper()))
                    {
                        return;
                    }

                    if (httpContext.Request.Path.ToUpper().Contains("NavigationHistory".ToUpper()))
                    {
                        return;
                    }

                    if (httpContext.Request.Path.ToUpper().Contains("Statistiche".ToUpper()))
                    {
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(httpContext.User?.Identity.Name))
                    {
                        return;
                    }

                    BaseController baseController = new BaseController();

                    var _ip = baseController.GetIP(filtercontext.HttpContext);

                    UnitOfWork unitOfWork = new UnitOfWork();

                    ////check exist db
                    //var _ipexists = unitOfWork.NavigatioHistoryRepository
                    //.Get(x => x.UserHostAddress == _ip && x.JsonIpInfo != "" && x.JsonIpInfo != null);

                    //var _JsonIpInfo = "";

                    //if (_ipexists.Count() > 0)
                    //{
                    //    _JsonIpInfo = _ipexists.FirstOrDefault().JsonIpInfo;
                    //}
                    //else
                    //{
                    //    try
                    //    {
                    //        WebClient httpClient = new WebClient();
                    //        _JsonIpInfo = httpClient.DownloadString($"https://ipapi.co/{_ip}/json/");

                    //    }
                    //    catch
                    //    {

                    //    }
                    //}

                    NavigatioHistory navigatioHistory = new NavigatioHistory();
                    navigatioHistory.Data = DateTime.Now;
                    navigatioHistory.UserHostAddress = _ip;
                    navigatioHistory.CurrentUrl = httpContext.Request?.Url.PathAndQuery;
                    navigatioHistory.Username = httpContext.User?.Identity.Name;
                    //navigatioHistory.JsonIpInfo = _JsonIpInfo;

                    var browser = httpContext?.Request.Browser;

                    if (browser != null)
                    {
                        navigatioHistory.BrowserIsMobileDevice = browser.IsMobileDevice;
                        navigatioHistory.BrowserJScriptVersionMajor = browser.JScriptVersion.Major;
                        navigatioHistory.BrowserJScriptVersionMinor = browser.JScriptVersion.Minor;
                        navigatioHistory.BrowserMajorVersion = browser.MajorVersion;
                        navigatioHistory.BrowserMobileDeviceModel = browser.MobileDeviceModel;
                        navigatioHistory.BrowserName = browser.Browser;
                        navigatioHistory.BrowserVersion = browser.Version;
                    }

                    unitOfWork.NavigatioHistoryRepository.Insert(navigatioHistory);
                    await unitOfWork.SaveAsync();
                }
                catch
                {
                }
            });
        }

        internal static void LogOffUser(string id)
        {
            RemoveUser(id);

            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<SediinPraticheRegionaliHub>();
            context.Clients.All.onLogOffUtente(id, "Attenzione, sei stato espulso dal Amministratore");
        }

        internal static void RemoveUser(string id)
        {
            try
            {
                //Monitor.Enter(_lock);

                IHubContext context = GlobalHost.ConnectionManager.GetHubContext<SediinPraticheRegionaliHub>();

                try
                {
                    Useronline.Remove(Useronline.FirstOrDefault(x => x.Item1 == id));
                }
                catch
                {

                }
                finally
                {
                    context = GlobalHost.ConnectionManager.GetHubContext<SediinPraticheRegionaliHub>();
                    context.Clients.All.updateUserOnline(Useronline.Select(x => x.Item1).Distinct().Count());
                }
            }
            catch
            {

            }
            finally
            {
                //Monitor.Exit(_lock);
            }
        }


        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            Task.Run(() =>
            {
                try
                {
                    if (Useronline == null)
                    {
                        Useronline = new List<(string, DateTime)>();
                    }

                    if (Useronline != null)
                    {
                        foreach (var item in Useronline)
                        {
                            if (item.Item2.AddMinutes(20) < DateTime.Now)
                            {
                                Useronline.Remove(item);
                            }
                        }
                    }

                    if (filterContext.HttpContext.User != null)
                    {
                        LogUser(filterContext);

                        if (filterContext.RouteData?.Values["action"]?.ToString() != "LogOff" && filterContext.HttpContext.User != null && filterContext.HttpContext.User.Identity.IsAuthenticated)
                        {
                            var _user = Useronline.FirstOrDefault(x => x.Item1 == filterContext.HttpContext.User?.Identity?.Name);
                            if (_user.Item1 != null)
                            {
                                Useronline.Remove(_user);
                            }

                            Useronline.Add((filterContext.HttpContext?.User?.Identity?.Name, DateTime.Now));
                        }
                    }

                    IHubContext context = GlobalHost.ConnectionManager.GetHubContext<SediinPraticheRegionaliHub>();
                    context.Clients.All.updateUserOnline(Useronline.Select(x => x.Item1).Distinct().Count());
                }
                catch
                {
                }
                finally
                {
                }
            });
        }
    }
}


