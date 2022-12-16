using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using System.Security.Principal;
using Main.Control.Core.Models;
using Main.Control.Service.Utilities;
using Main.Control.Utilities.Infrastructure;
using Newtonsoft.Json;

namespace Main.Control.Web.Utilities
{
    public class SessionAliveAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            if (filterContext.HttpContext.Session != null && filterContext.HttpContext.Session.IsNewSession)
            {
                if (!string.IsNullOrEmpty(AppCookies.CookieSession))
                {
                    //CookieSessionItems _cookieSessionItems = (CookieSessionItems)Utility.DeSerialize(HttpUtility.UrlDecode(AppCookies.CookieSession), typeof(CookieSessionItems));
                    try
                    {
                        CookieSessionItems _cookieSessionItems = (CookieSessionItems)JsonConvert.DeserializeObject(AppCookies.CookieSession, typeof(CookieSessionItems));
                        if (_cookieSessionItems != null && !Utility.IsStringEmpty(_cookieSessionItems.AdminUserName))
                        {
                            filterContext.HttpContext.User = new GenericPrincipal(new GenericIdentity(_cookieSessionItems.AdminUserName, "Forms"), null);
                            //set forms auth cookie
                            if (_cookieSessionItems.RememberMe == Constants.True)
                            {
                                FormsAuthentication.SetAuthCookie(_cookieSessionItems.AdminUserName, true);
                            }
                            else
                            {
                                FormsAuthentication.SetAuthCookie(_cookieSessionItems.AdminUserName, false);
                            }
                            filterContext.HttpContext.Session[SessionItemKey.AdminUserId] = _cookieSessionItems.AdminUserId;
                            filterContext.HttpContext.Session[SessionItemKey.AdminUserName] = _cookieSessionItems.AdminUserName;
                            filterContext.HttpContext.Session[SessionItemKey.AdminEmailAddress] = _cookieSessionItems.AdminEmailAddress;
                            filterContext.HttpContext.Session[SessionItemKey.AdminRole] = _cookieSessionItems.AdminRole;
                            filterContext.HttpContext.Session[SessionItemKey.AdminSkuType] = _cookieSessionItems.AdminSKUType;
                            filterContext.HttpContext.Session[SessionItemKey.ProjectType] = _cookieSessionItems.ProjectType;
                            filterContext.HttpContext.Session[SessionItemKey.IsAdmin] = _cookieSessionItems.IsAdmin;
                            filterContext.HttpContext.Session[SessionItemKey.AdminDisplayName] = _cookieSessionItems.AdminDisplayName;
                        }
                    }
                    catch (Exception ex)
                    {
                        var emsg = ex.Message;
                    }
                }
            }
            else if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                if (!string.IsNullOrEmpty(AppCookies.CookieSession))
                {
                    // CookieSessionItems _cookieSessionItems = (CookieSessionItems)Utility.DeSerialize(HttpUtility.UrlDecode(AppCookies.CookieSession), typeof(CookieSessionItems));
                    CookieSessionItems _cookieSessionItems = (CookieSessionItems)JsonConvert.DeserializeObject(AppCookies.CookieSession, typeof(CookieSessionItems));
                    if (_cookieSessionItems != null && !Utility.IsStringEmpty(_cookieSessionItems.AdminUserName))
                    {
                        filterContext.HttpContext.User = new GenericPrincipal(new GenericIdentity(_cookieSessionItems.AdminUserName, "Forms"), null);
                        //set forms auth cookie
                        if (_cookieSessionItems.RememberMe == Constants.True)
                        {
                            FormsAuthentication.SetAuthCookie(_cookieSessionItems.AdminUserName, true);
                        }
                        else
                        {
                            FormsAuthentication.SetAuthCookie(_cookieSessionItems.AdminUserName, false);
                        }
                    }
                }
            }
        }
    }
}