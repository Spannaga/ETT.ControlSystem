using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using ProcessBulkUpload.Utilities;

namespace Main.Control.Services.Utilities
{
    public class ETTAPIClient : HttpClient
    {
        public ETTAPIClient()
        {
            if (!string.IsNullOrWhiteSpace(Utility.GetAppSettings("ETTAPI")))
            {
                string apiURL = Utility.GetAppSettings("ETTAPI");
                //string appURL = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority);
                Uri apiURI = new Uri(apiURL);
                this.BaseAddress = apiURI;
            }
            else
            {
                this.BaseAddress = new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority));
            }
            this.DefaultRequestHeaders.Accept.Clear();

            this.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public string EmailAddress { get; set; }
    }
}
