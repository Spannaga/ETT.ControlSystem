using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;

namespace Main.Control.Web.Utilities
{
    public class GraphApiClient:HttpClient
    {
        public GraphApiClient()
        {
            this.BaseAddress = new Uri("https://graph.microsoft.com/v1.0/me");
            this.DefaultRequestHeaders.Accept.Clear();
            this.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}