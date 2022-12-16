using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;

namespace Main.Control.Web.Utilities
{
    public class SalesHttpClient : HttpClient
    {
        public SalesHttpClient()
        {
            if (!Utility.IsStringEmpty(Utility.GetAppSettings("SalesAPIBaseAddress")))
            {
                this.BaseAddress = new Uri(Utility.GetAppSettings("SalesAPIBaseAddress"));
            }
            else
            {
                this.BaseAddress = new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority));
            }
            this.DefaultRequestHeaders.Accept.Clear();
            this.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public SalesHttpClient(HttpMessageHandler handler)
            : base(handler)
        {
            this.BaseAddress = new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority));
        }
    }
}