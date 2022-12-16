using System;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.ComponentModel;
using Main.Control.Utilities.Infrastructure;
using System.Collections.Generic;


namespace Main.Control.Web.Utilities
{
    public class TaxAuthorizeAttribute : AuthorizeAttribute
    {
        public string CheckUserStatus { get; set; }
        public bool IgnoreHoldingCheck { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            // auth failed, redirect to Sign In
            if (!filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                filterContext.Result = new HttpUnauthorizedResult();
            }
            else if (!IsValidSession(filterContext)) //if not valid session, take to Sign In
            {
                filterContext.HttpContext.Session.Abandon();
                //Redirect to Session Expired
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = ControllerNames.Admin, action = ActionNames.SignIn }));
            }
            else if (!IsValidRole(filterContext)) //if not in Role, take to Sign In with Message
            {
                //Abandon Session
                //filterContext.HttpContext.Session.Abandon();
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = ControllerNames.Admin, action = ActionNames.NoAccess }));
            }
            else if (IsValidAuthCookie(filterContext)) //|| !IsValidAuthCookie(filterContext)) //if not valid session, take to Sign In
            {
                filterContext.HttpContext.Session.Abandon();
                //Redirect to Session Expired
                filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary(new { controller = ControllerNames.Admin, action = ActionNames.SignIn }));
            }
        }

        //Check for Valid Auth Cookie
        private bool IsValidAuthCookie(AuthorizationContext filterContext)
        {
            if (filterContext.HttpContext.Request.Cookies[".ASPXAUTH"] != null)
            {
                HttpCookie authCookie = filterContext.HttpContext.Request.Cookies[".ASPXAUTH"];
                if (authCookie != null && !string.IsNullOrWhiteSpace(authCookie.Value))
                {
                    System.Web.Security.FormsAuthenticationTicket formsAuthenticationTicket = System.Web.Security.FormsAuthentication.Decrypt(authCookie.Value);
                    string _userData = formsAuthenticationTicket.UserData;
                    string _etzUserId = filterContext.HttpContext.Session["AdminUserId"] != null ? filterContext.HttpContext.Session["AdminUserId"].ToString() : string.Empty;
                    if (string.IsNullOrWhiteSpace(_userData) || (!string.IsNullOrWhiteSpace(_userData) && _userData != _etzUserId))
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }

                return true;
            }
            return false;
        }


        //Check for Valid Session
        private bool IsValidSession(AuthorizationContext filterContext)
        {
            if ((filterContext.HttpContext.Session == null) || (filterContext.HttpContext.Session != null && filterContext.HttpContext.Session.Keys.Count == 0))
            {
                return false;
            }
            return true;
        }

        //Check for Valid Role
        private bool IsValidRole(AuthorizationContext filterContext)
        {
            //Check for Roles

            //if (!string.IsNullOrEmpty(Roles))
            //{
            //    if (filterContext.HttpContext.Session != null && filterContext.HttpContext.Session[SessionItemKey.Administrator] != null)
            //    {
            //        PURoles role = (PURoles)Enum.Parse(typeof(PURoles), filterContext.HttpContext.Session[SessionItemKey.Administrator].ToString(), true);
            //        if (Roles != "" && (!Roles.Contains(role.ToString())))
            //        {
            //            return false;
            //        }
            //    }
            //    else
            //    {
            //        return false;
            //    }
            //}
            return true;
        }

        protected void SetCachePolicy(AuthorizationContext filterContext)
        {
            // ** IMPORTANT **
            // Since we're performing authorization at the action level, the authorization code runs
            // after the output caching module. In the worst case this could allow an authorized user
            // to cause the page to be cached, then an unauthorized user would later be served the
            // cached page. We work around this by telling proxies not to cache the sensitive page,
            // then we hook our custom authorization code into the caching mechanism so that we have
            // the final say on whether a page should be served from the cache.
            HttpCachePolicyBase cachePolicy = filterContext.HttpContext.Response.Cache;
            cachePolicy.SetProxyMaxAge(new TimeSpan(0));
            cachePolicy.AddValidationCallback(CacheValidateHandler, null /* data */);
        }

        private void CacheValidateHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
        {
            validationStatus = OnCacheAuthorization(new HttpContextWrapper(context));
        }
    }
}