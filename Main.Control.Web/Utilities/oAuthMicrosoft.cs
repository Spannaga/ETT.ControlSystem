using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;
using Main.Control.Service.Utilities;
using Main.Control.Web.Models;
using Newtonsoft.Json;

namespace Main.Control.Web.Utilities
{
    public class oAuthMicrosoft
    {
        public enum Method { GET, POST };
        public const string AUTHORIZE = "https://login.microsoftonline.com/common/oauth2/v2.0/authorize";
        public const string ACCESS_TOKEN = "https://login.microsoftonline.com/common/oauth2/v2.0/token";
        string _clientId = ConfigurationManager.AppSettings[Constants.MicrosoftClientId];
        string _clientSecret = ConfigurationManager.AppSettings[Constants.MicrosoftClientSecret];
        public const string SCOPE = "offline_access%20user.read%20mail.read";
        public const string STATE = "12345";
        public const string response_type = "code";
        string CALLBACK_URL = ConfigurationManager.AppSettings[Constants.MicrosoftCallbackUrl];
        string _token = "";
        /// <summary>
        /// Get the link to Facebook's authorization page for this application.
        /// </summary>
        /// <returns>The url with a valid request token, or a null string.</returns>
        public string AuthorizationLinkGet()
        {
            return string.Format("{0}?scope={1}&state={2}&redirect_uri={3}&response_type={4}&client_id={5}&response_mode=query", AUTHORIZE, SCOPE, STATE, CALLBACK_URL, response_type, this.ConsumerKey);
        }

        public string ConsumerKey
        {
            get
            {
                if (_clientId.Length == 0)
                {
                    _clientId = ConfigurationManager.AppSettings[Constants.MicrosoftClientId]; //Your application ID
                }
                return _clientId;
            }
            set { _clientId = value; }
        }

        public string ConsumerSecret
        {
            get
            {
                if (_clientSecret.Length == 0)
                {
                    _clientSecret = ConfigurationManager.AppSettings[Constants.MicrosoftClientSecret]; //Your application secret
                }
                return _clientSecret;
            }
            set { _clientSecret = value; }
        }

        public string Token { get { return _token; } set { _token = value; } }


        /// <summary>
        /// Exchange the Facebook "code" for an access token.
        /// </summary>
        /// <param name="authToken">The oauth_token or "code" is supplied by Facebook's authorization page following the callback.</param>
        public void AccessTokenGet(string authToken)
        {
            this.Token = authToken;

            string _postData = string.Format("code={0}&client_id={1}&client_secret={2}&redirect_uri={3}&grant_type=authorization_code&scope={4}", authToken, ConsumerKey, ConsumerSecret, CALLBACK_URL, SCOPE);

            string response = WebRequest(Method.POST, ACCESS_TOKEN, _postData);

            if (response.Length > 0)
            {
                JObject jsonMicrosoftResponse = JObject.Parse(response);
                if (jsonMicrosoftResponse != null && jsonMicrosoftResponse["access_token"] != null)
                {
                    this.Token = jsonMicrosoftResponse["access_token"].ToString();
                }
            }
        }

        /// <summary>
        /// Web Request Wrapper
        /// </summary>
        /// <param name="method">Http Method</param>
        /// <param name="url">Full url to the web resource</param>
        /// <param name="postData">Data to post in querystring format</param>
        /// <returns>The web server response.</returns>
        public string WebRequest(Method method, string url, string postData)
        {

            HttpWebRequest webRequest = null;
            StreamWriter requestWriter = null;
            string responseData = "";

            webRequest = System.Net.WebRequest.Create(url) as HttpWebRequest;
            webRequest.Method = method.ToString();
            webRequest.ServicePoint.Expect100Continue = false;
            webRequest.UserAgent = "[You user agent]";
            webRequest.Timeout = 20000;

            if (method == Method.POST)
            {
                webRequest.ContentType = "application/x-www-form-urlencoded";


                //POST the data.
                requestWriter = new StreamWriter(webRequest.GetRequestStream());

                try
                {
                    requestWriter.Write(postData);
                }
                catch
                {
                    throw;
                }

                finally
                {
                    requestWriter.Close();
                    requestWriter = null;
                }
            }

            responseData = WebResponseGet(webRequest);
            webRequest = null;
            return responseData;
        }

        /// <summary>
        /// Process the web response.
        /// </summary>
        /// <param name="webRequest">The request object.</param>
        /// <returns>The response data.</returns>
        public string WebResponseGet(HttpWebRequest webRequest)
        {
            StreamReader responseReader = null;
            string responseData = "";

            try
            {
                responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
                responseData = responseReader.ReadToEnd();
            }
            catch
            {
                throw;
            }
            finally
            {
                webRequest.GetResponse().GetResponseStream().Close();
                responseReader.Close();
                responseReader = null;
            }
            return responseData;
        }

        public microsoftResponseJson GraphApiCall(string accessToken)
        {
            microsoftResponseJson microsoftResponseJson = new microsoftResponseJson();
            using (var apiClient = new GraphApiClient())
            {
                apiClient.DefaultRequestHeaders.Add("Authorization", "Bearer " +accessToken);
                var response = apiClient.GetAsync("").Result;
                if (response.IsSuccessStatusCode)
                {
                    string serializedString = response.Content.ReadAsStringAsync().Result;
                    // deserialze the string
                    microsoftResponseJson = (microsoftResponseJson)JsonConvert.DeserializeObject(serializedString, typeof(microsoftResponseJson));
                }
            }
            return microsoftResponseJson;
        }
    }
}