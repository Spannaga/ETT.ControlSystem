using ProcessBulkUpload.Utilities;
using Main.Control.Services.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;


namespace Main.Control.Services.Utilities
{
    public class ETTAPIRequestUtilities
    {
        static string emailAddress = string.Empty;
        public static void GenerateAuthHeader(ETTAPIClient client, string requestUri, string methodType)
        {
            emailAddress = client.EmailAddress;
            string appId = Utility.GetAppSettings("ETTAppId");

            client.DefaultRequestHeaders.Clear();
            if (!string.IsNullOrWhiteSpace(appId))
            {
                if (!client.DefaultRequestHeaders.Contains("UserAuthorization"))
                {
                    string authorizeHeader = appId;
                    client.DefaultRequestHeaders.Add("UserAuthorization", authorizeHeader);
                }
            }

            var utcDate = DateTime.Now.ToUniversalTime().ToString("MM/dd/yyyy hh:mm:ss.fff tt");
            string utcDateString = string.Format("{0:U}", utcDate); //"Thursday, May 21, 2015 4:33:50 AM"

            client.DefaultRequestHeaders.Add("Timestamp", utcDateString);
            string authenticationHeader = Utility.GetAppSettings("ETTAPIPublicKey") + ":" + ComputeHash(Utility.GetAppSettings("ETTAPIPrivateKey"), BuildAuthSignature(methodType, utcDateString, requestUri.ToLower()));
            client.DefaultRequestHeaders.Add("Authentication", authenticationHeader);

            string uniqueId = Guid.NewGuid().ToString();
            client.DefaultRequestHeaders.Add("UniqueId", uniqueId);

            if (!string.IsNullOrWhiteSpace(emailAddress))
            {
                client.DefaultRequestHeaders.Add("Email", emailAddress);
            }
        }

        public static string BuildAuthSignature(string methodType, string date, string absolutePath)
        {
            string message = string.Empty;
            absolutePath = "/" + absolutePath;
            var uri = HttpContext.Current != null ? HttpContext.Current.Server.UrlDecode(absolutePath) : absolutePath;
            message = string.Join("\n", methodType, date, uri);

            return message;
        }

        public static string ComputeHash(string privateKey, string message)
        {
            var key = Encoding.UTF8.GetBytes(privateKey);
            string hashString;

            using (var hmac = new HMACSHA256(key))
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(message));
                hashString = Convert.ToBase64String(hash);
            }

            return hashString;
        }
    }
}
