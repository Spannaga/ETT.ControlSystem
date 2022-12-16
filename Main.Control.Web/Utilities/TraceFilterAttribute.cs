using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Main.Control.Core.Models;

namespace Main.Control.Web.Utilities
{
    [AttributeUsage(AttributeTargets.All, Inherited= false ,AllowMultiple =true)]
    public sealed class TraceFilterAttribute : ActionFilterAttribute
    {
        private string Parameter { get; set; }
        private ActionDescriptor CurrentAction { get; set; }
        private bool isValidIP;

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext == null)
            {
                
            }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {

            if (Utility.GetBool(Utility.GetAppSettings("UseMainControlIpRestrict")))
            {
                using (var client = new SalesHttpClient())
                {
                    var logUri = "Sales/GetAllValidIPAddress?projectId=" + 0;

                    var logResponseList = client.GetAsync(logUri).Result;

                    if (logResponseList.IsSuccessStatusCode)
                    {
                        List<string> ipAddressList = logResponseList.Content.ReadAsAsync<List<string>>().Result;

                        string _controller = filterContext.RouteData.Values["Controller"].ToString().ToLower();
                        string _action = filterContext.RouteData.Values["Action"].ToString().ToLower();

                        if (ipAddressList != null && ipAddressList.Any())
                        {
                            if (!ipAddressList.Any(ip => ip == GetIPAddress()))
                            {
                                if ((_controller == "admin" && _action == "noaccess"))
                                {
                                    isValidIP = true;
                                }
                                else
                                {
                                    isValidIP = false;
                                }
                            }
                            else
                            {
                                isValidIP = true;
                            }
                        }
                        else
                        {
                            isValidIP = false;
                        }
                    }
                    else
                    {
                        string msg = logResponseList.Content.ReadAsStringAsync().Result;
                    }
                }

                if (!isValidIP && !Utility.GetBool(Utility.GetAppSettings("IsLocal")))
                {
                    filterContext.Result = new RedirectResult(Utility.GetAppSettings("MainControlNoAccess"));
                }

                ScActivityLog scActivityLog = new ScActivityLog();

                HttpRequestBase request = filterContext.HttpContext.Request;

                //Get activity log details
                scActivityLog = GetActivityLogDetails(filterContext);

                using (var client = new SalesHttpClient())
                {
                    var logUri = "Sales/SaveActivityLog?scActivityLogDetails=" + scActivityLog;

                    var logResponseList = client.PostAsJsonAsync(logUri, scActivityLog).Result;
                    if (logResponseList.IsSuccessStatusCode)
                    {
                        long isSaved = logResponseList.Content.ReadAsAsync<long>().Result;
                    }
                    else
                    {
                        string msg = logResponseList.Content.ReadAsStringAsync().Result;
                    }
                }
            }
        }

        #region Get Activity Log Details
        /// <summary>
        /// Get Activity Log Details
        /// </summary>
        /// <param name="filterContext"></param>
        /// <returns></returns>
        public ScActivityLog GetActivityLogDetails(ActionExecutingContext filterContext)
        {
            HttpRequestBase request = filterContext.HttpContext.Request;
            var parameters = string.Join(";", filterContext.ActionParameters.Select(x => x.Key + "=" + x.Value).ToArray());
            Parameter = parameters;
            CurrentAction = filterContext.ActionDescriptor;
            var controller = filterContext.Controller.GetType().Name.Replace("Controller", "");
            var message = string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", controller, filterContext.ActionDescriptor.ActionName, string.IsNullOrEmpty(parameters) ? "void" : parameters);

            return new ScActivityLog
            {
                ActionType = string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", controller, filterContext.ActionDescriptor.ActionName),
                ActivityDate = DateTime.Now,
                ControllerName = filterContext.Controller.GetType().Name.Replace("Controller", ""),
                ActionName = filterContext.ActionDescriptor.ActionName,
                Activity = message,
                Memo = message,
                IPAddress = request.UserHostAddress,
                UserId = Utility.GetUserIdFromSession()
            };
        }
        #endregion

        #region Get IP Address
        /// <summary>
        /// Gets the IP address.
        /// </summary>
        /// <returns></returns>
        private static string GetIPAddress()
        {
            string _ipAddress = HttpContext.Current.Request.UserHostAddress.ToString();
            //map IP client address
            bool _isLive = Convert.ToBoolean(Utility.GetAppSettings("IsLive"));
            bool _isAWS_ELB = Convert.ToBoolean(Utility.GetAppSettings("Is_AWS_ELB"));
            if (_isLive && _isAWS_ELB)
            {
                _ipAddress = HttpContext.Current.Request.Headers["X-Forwarded-For"].ToString();
            }
            return _ipAddress;
        }
        #endregion
    }
}