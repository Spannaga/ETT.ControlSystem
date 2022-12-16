using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Main.Control.Services.Utilities
{
    public class PaymentAPIClient : HttpClient
    {
        public PaymentAPIClient()
        {
            this.BaseAddress = new Uri(Utilities.GetAppSettings("PaymentAPI"));
            this.Timeout = new TimeSpan(0, 5, 0);
            this.DefaultRequestHeaders.Accept.Clear();

            this.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }
    }
}
