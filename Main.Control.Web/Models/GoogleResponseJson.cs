namespace Main.Control.Web.ViewModels
{
    public class GoogleResponseJson
    {
        //See this url for any response fields http://code.google.com/apis/accounts/docs/OAuth2Login.html
        public string id { get; set; }
        public string email { get; set; }
        public string verified_email { get; set; }
        public string name { get; set; }
        public string given_name { get; set; }
        public string family_name { get; set; }
        public string picture { get; set; }
        public string locale { get; set; }
        public string timezone { get; set; }
        public string gender { get; set; }
    }
}