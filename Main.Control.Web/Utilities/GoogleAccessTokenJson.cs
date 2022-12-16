using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Main.Control.Web.Utilities
{
    class GoogleAccessTokenJson
    {
        public string access_token { get; set; }
        public string token_type { get; set; }
        public string expires_in { get; set; }
        public string id_token { get; set; }

    }
}