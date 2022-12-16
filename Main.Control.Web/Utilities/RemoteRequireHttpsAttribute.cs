using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Main.Control.Web.Utilities
{
    public class RemoteRequireHttpsAttribute : System.Web.Mvc.RequireHttpsAttribute
    {
        public override void OnAuthorization(System.Web.Mvc.AuthorizationContext filterContext)
        {
            if ((filterContext == null))
            {
                throw new ArgumentNullException("filterContext");
            }

            //get the client forwared protocol for the AWS ELB
            bool _isLive = Convert.ToBoolean(ConfigurationManager.AppSettings["IsLive"].ToString());
            bool _isAWS_ELB = Convert.ToBoolean(ConfigurationManager.AppSettings["Is_AWS_ELB"].ToString());
            string _forwaredProtocol = "HTTP";
            if (_isLive && _isAWS_ELB && filterContext.HttpContext != null && filterContext.HttpContext.Request != null && filterContext.HttpContext.Request.Headers != null && filterContext.HttpContext.Request.Headers["X-Forwarded-Proto"] != null)
            {
                _forwaredProtocol = filterContext.HttpContext.Request.Headers["X-Forwarded-Proto"].ToString().ToUpper();
            }

            if ((filterContext.HttpContext != null) && (filterContext.HttpContext.Request.IsLocal || ConfigurationManager.AppSettings["IsSSLEnabled"].ToLower() == "false" || _forwaredProtocol == "HTTPS"))
            {
                return;
            }

            if (_forwaredProtocol == "HTTP" && ConfigurationManager.AppSettings["IsSSLEnabled"].ToLower() == "true" && _isAWS_ELB && _isLive)
            {
                string url = "https://" + filterContext.HttpContext.Request.Url.Host + filterContext.HttpContext.Request.RawUrl;
                filterContext.Result = new RedirectResult(url);
            }
            else
            {
                base.OnAuthorization(filterContext);
            }
        }
        protected override void HandleNonHttpsRequest(AuthorizationContext filterContext)
        {
            string _taxyear = ConfigurationManager.AppSettings["IsRemoteRequired"].ToString();
            if (!string.IsNullOrWhiteSpace(_taxyear) && _taxyear.ToLower() == "true")
            {
                if (!string.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
                {
                    throw new InvalidOperationException("Only secured request are allowed!");
                }
            }
            string url = "https://" + filterContext.HttpContext.Request.Url.Host + filterContext.HttpContext.Request.RawUrl;
            filterContext.Result = new RedirectResult(url);
        }
    }
}