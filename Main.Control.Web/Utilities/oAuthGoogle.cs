using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;

namespace Main.Control.Web.Utilities
{
    public class oAuthGoogle
    {

        public enum Method { GET, POST };
        public const string AUTHORIZE = "https://accounts.google.com/o/oauth2/auth";
        public const string ACCESS_TOKEN = "https://accounts.google.com/o/oauth2/token";
        string CALLBACK_URL = ConfigurationManager.AppSettings["GoogleCallbackUrl"];

        public const string SCOPE = "https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fuserinfo.email+https%3A%2F%2Fwww.googleapis.com%2Fauth%2Fuserinfo.profile";
        public const string STATE = "%2Fprofile";
        public const string response_type = "code";
        //public const string SCOPE = "https://www.googleapis.com/auth/analytics.readonly";

        private string _consumerKey = ConfigurationManager.AppSettings["GoogleConsumerKey"];
        private string _consumerSecret = ConfigurationManager.AppSettings["GoogleConsumerSecret"];
        private string _token = "";

        #region Properties

        public string ConsumerKey
        {
            get
            {
                if (_consumerKey.Length == 0)
                {
                    _consumerKey = ConfigurationManager.AppSettings["GoogleConsumerKey"]; //Your application ID
                }
                return _consumerKey;
            }
            set { _consumerKey = value; }
        }

        public string ConsumerSecret
        {
            get
            {
                if (_consumerSecret.Length == 0)
                {
                    _consumerSecret = ConfigurationManager.AppSettings["GoogleConsumerSecret"]; //Your application secret
                }
                return _consumerSecret;
            }
            set { _consumerSecret = value; }
        }

        public string Token { get { return _token; } set { _token = value; } }

        #endregion

        /// <summary>
        /// Get the link to Facebook's authorization page for this application.
        /// </summary>
        /// <returns>The url with a valid request token, or a null string.</returns>
        public string AuthorizationLinkGet()
        {
            return string.Format("{0}?scope={1}&state={2}&redirect_uri={3}&response_type={4}&client_id={5}&approval_prompt=force", AUTHORIZE, SCOPE, STATE, CALLBACK_URL, response_type, this.ConsumerKey);
        }


        /// <summary>
        /// Exchange the Facebook "code" for an access token.
        /// </summary>
        /// <param name="authToken">The oauth_token or "code" is supplied by Facebook's authorization page following the callback.</param>
        public void AccessTokenGet(string authToken)
        {
            this.Token = authToken;
            // string accessTokenUrl = string.Format("{0}?code={1}&client_id={2}&client_secret={3}&redirect_uri={4}&grant_type=authorization_code", ACCESS_TOKEN, authToken, ConsumerKey, ConsumerSecret, CALLBACK_URL);

            string _postData = string.Format("code={0}&client_id={1}&client_secret={2}&redirect_uri={3}&grant_type=authorization_code", authToken, ConsumerKey, ConsumerSecret, CALLBACK_URL);

            string response = WebRequest(Method.POST, ACCESS_TOKEN, _postData);

            if (response.Length > 0)
            {
                //Store the returned access_token
                //NameValueCollection qs = HttpUtility.ParseQueryString(response);

                JavaScriptSerializer serializer = new JavaScriptSerializer();
                GoogleAccessTokenJson _googleAccessToken = (GoogleAccessTokenJson)serializer.Deserialize(response, typeof(GoogleAccessTokenJson));

                if (_googleAccessToken != null)
                {
                    this.Token = _googleAccessToken.access_token;
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

        //public string AuthorizationLinkGet()
        //{
        //    throw new NotImplementedException();
        //}

    }
}