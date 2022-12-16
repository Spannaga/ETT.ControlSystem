using System;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.IO;
using System.Xml;
using System.Xml.Serialization;

using Main.Control.Core.Models;
using Main.Control.Utilities.Infrastructure;
using System.Web.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Security.Cryptography;
using Amazon.S3;
using Amazon.S3.Model;
using Main.Control.Service.Utilities;
using Amazon;

namespace Main.Control.Web.Utilities
{
    public class Utility
    {
        #region Check Empty String
        /// <summary>
        /// Compare string for empty value
        /// </summary>
        /// <param name="value">string to check empty</param>
        /// <returns>bool status</returns>
        public static bool IsStringEmpty(string value)
        {
            return string.IsNullOrEmpty(value);
        }
        #endregion

        #region Get App Settings
        /// <summary>
        /// Get the appsetting value from the web.config
        /// </summary>
        /// <param name="appKey">appsettings key</param>
        /// <returns></returns>
        public static string GetAppSettings(string appKey)
        {
            object value = null;
            value = ConfigurationManager.AppSettings[appKey];
            if (value != null)
            {
                return value.ToString();
            }
            return string.Empty;
        }
        #endregion

        #region Get IP Address
        /// <summary>
        /// Gets the IP address.
        /// </summary>
        /// <returns></returns>
        public static string GetIPAddress()
        {
            string _ipAddress = HttpContext.Current.Request.UserHostAddress.ToString();
            //map IP client address
            bool _isLive = Convert.ToBoolean(ConfigurationManager.AppSettings["IsLive"].ToString());
            bool _isAWS_ELB = Convert.ToBoolean(ConfigurationManager.AppSettings["Is_AWS_ELB"].ToString());
            if (_isLive && _isAWS_ELB)
            {
                _ipAddress = HttpContext.Current.Request.Headers["X-Forwarded-For"].ToString();
            }
            return _ipAddress;
        }
        #endregion

        #region Convert to Long
        /// <summary>
        /// Convert to Long.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static long GetLong(object value)
        {
            long result = 0;
            if (value != null)
            {
                long.TryParse(value.ToString(), out result);
            }
            return result;
        }
        #endregion

        #region Convert to Int
        /// <summary>
        /// Convert to Int.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static int GetInt(object value)
        {
            int result = 0;
            if (value != null)
            {
                int.TryParse(value.ToString(), out result);
            }
            return result;
        }
        #endregion

        #region Convert to Double
        /// <summary>
        /// Convert to double.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static double GetDouble(object value)
        {
            double result = 0;
            if (value != null)
            {
                double.TryParse(value.ToString(), out result);
            }
            return result;
        }
        #endregion

        #region Convert to Decimal
        /// <summary>
        /// Convert to Decimal.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static decimal GetDecimal(object value)
        {
            decimal result = 0;
            if (value != null)
            {
                decimal.TryParse(value.ToString(), out result);
            }
            return result;
        }
        #endregion

        #region Convert to Bool
        /// <summary>
        /// Convert to Bool.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static bool GetBool(object value)
        {
            bool result = false;
            if (value != null)
            {
                bool.TryParse(value.ToString(), out result);
            }
            return result;
        }
        #endregion

        #region Convert to decimal 2 Digits
        /// <summary>
        /// Round the Value to two digits
        /// </summary>
        /// <param name="Value">The value.</param>
        /// <returns></returns>
        public static decimal GetDecimal2Digits(object value)
        {
            decimal result = decimal.MinValue;
            if (value != null)
            {
                decimal.TryParse(value.ToString(), out result);
                result = Math.Round(result, 2);
                decimal.TryParse(result.ToString("#.00"), out result);
            }
            return result;
        }
        #endregion

        #region Serialize

        public static string Serialize(object seralizedObject)
        {
            string serializedData = string.Empty;
            if (seralizedObject != null)
            {
                StringWriter strWriter = new StringWriter();
                XmlSerializer serializer = new XmlSerializer((seralizedObject).GetType());
                serializer.Serialize(strWriter, seralizedObject);
                serializedData = strWriter.ToString();
                serializedData = serializedData.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", string.Empty);
                serializedData = serializedData.Replace("xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"", string.Empty);
                serializedData = serializedData.Replace("xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\"", string.Empty);
                StringReader str = new StringReader(serializedData);
                XmlDocument doc = new XmlDocument();
                doc.Load(str);
                StringWriter sw = new StringWriter();
                XmlTextWriter xw = new XmlTextWriter(sw);
                doc.WriteTo(xw);
                serializedData = sw.ToString();
            }
            return serializedData;
        }
        #endregion

        #region Deserialize

        public static Object DeSerialize(string xml, Type type)
        {
            try
            {
                object serializedObject;
                using (StringReader reader = new StringReader(xml))
                {
                    XmlSerializer serializer = new XmlSerializer(type);
                    serializedObject = serializer.Deserialize(reader);
                }
                return serializedObject;
            }
            catch
            {
                throw;
            }
        }
        #endregion

        #region Get Admin User Id from Session
        /// <summary>
        /// Gets the user id from session.
        /// </summary>
        /// <returns></returns>
        public static long GetAdminUserIdFromSession()
        {
            long _userId = 0;
            if (HttpContext.Current != null && HttpContext.Current.Session != null &&
                HttpContext.Current.Session[SessionItemKey.AdminUserId] != null)
            {
                _userId = Utility.GetLong(HttpContext.Current.Session[SessionItemKey.AdminUserId]);
                return _userId;
            }

            return _userId;
        }
        #endregion

        #region Get Admin User Name from Session
        /// <summary>
        /// Gets the user name from session.
        /// </summary>
        /// <returns></returns>
        public static string GetAdminUserNameFromSession()
        {
            string _userName = string.Empty;
            if (HttpContext.Current != null && HttpContext.Current.Session != null &&
                HttpContext.Current.Session[SessionItemKey.AdminUserName] != null)
            {
                _userName = HttpContext.Current.Session[SessionItemKey.AdminUserName].ToString();
                return _userName;
            }

            return _userName;
        }
        #endregion

        #region Add to AdminSession
        /// <summary>
        /// Add the required variables to session while signin /register.
        /// </summary>
        /// <param name="user">The admin.</param>
        public static void AddToSession(AdminUser admin)
        {
            AddToSession(admin, false);
        }

        public static void AddToSession(AdminUser admin, bool rememberPwd)
        {
            ////set forms auth cookie
            //FormsAuthentication.SetAuthCookie(admin.AdminUserName, rememberPwd);

            //set admin details to session
            HttpContext.Current.Session[SessionItemKey.AdminUserId] = admin.AdminUserId;
            HttpContext.Current.Session[SessionItemKey.AdminUserName] = admin.AdminUserName;
            HttpContext.Current.Session[SessionItemKey.AdminRole] = admin.AdminRoles;
            HttpContext.Current.Session[SessionItemKey.AdminSkuType] = admin.AdminSKUType;
            HttpContext.Current.Session[SessionItemKey.ProjectType] = admin.ProjectType;
            HttpContext.Current.Session[SessionItemKey.IsAdmin] = admin.IsAdmin;
            HttpContext.Current.Session[SessionItemKey.AdminDisplayName] = admin.AdminUserName;
            HttpContext.Current.Session[SessionItemKey.AdminEmailAddress] = admin.AdminEmailAddress;

            //Add Session Items to cookies Container
            CookieSessionItems _cookieSessionItems = new CookieSessionItems
            {
                AdminUserId = admin.AdminUserId.ToString(),
                AdminUserName = admin.AdminUserName,
                AdminRole = admin.AdminRoles,
                AdminSKUType = admin.AdminSKUType,
                ProjectType = admin.ProjectType,
                IsAdmin = admin.IsAdmin.ToString(),
                AdminDisplayName = admin.AdminUserName,
                IpAddress = admin.IpAddress,
                AdminEmailAddress = admin.AdminEmailAddress
            };
            //Add a cookie for remember, if remember me checked, else delete the cookie
            if (rememberPwd)
            {
                _cookieSessionItems.RememberMe = "True";
                AppCookies.SetValue(SessionItemKey.AdminUserName, admin.AdminUserId, DateTime.MaxValue);
                AppCookies.SetValue(SessionItemKey.CookieSession, JsonConvert.SerializeObject(_cookieSessionItems), DateTime.MaxValue);
                //   AppCookies.SetValue(SessionItemKey.CookieSession, HttpUtility.UrlEncode(Utility.Serialize(_cookieSessionItems)), DateTime.MaxValue);
            }
            else
            {
                _cookieSessionItems.RememberMe = "False";
                AppCookies.Remove(SessionItemKey.AdminUserName);
                //AppCookies.SetValue(SessionItemKey.CookieSession, HttpUtility.UrlEncode(Utility.Serialize(_cookieSessionItems)));
                AppCookies.SetValue(SessionItemKey.CookieSession, JsonConvert.SerializeObject(_cookieSessionItems));
            }
        }
        #endregion

        #region Get User Id from Session
        /// <summary>
        /// Gets the user id from session.
        /// </summary>
        /// <returns></returns>
        public static long GetUserIdFromSession()
        {
            long _userId = 0;
            if (HttpContext.Current != null && HttpContext.Current.Session != null &&
                HttpContext.Current.Session[SessionItemKey.AdminUserId] != null)
            {
                _userId = Utility.GetLong(HttpContext.Current.Session[SessionItemKey.AdminUserId]);
                return _userId;
            }

            return _userId;
        }
        #endregion

        #region Check Admin From Session
        /// <summary>
        /// Gets the user id from session.
        /// </summary>
        /// <returns></returns>
        public static bool CheckAdminFromSession()
        {
            bool _isAdmin = false;
            if (HttpContext.Current != null && HttpContext.Current.Session != null &&
                HttpContext.Current.Session[SessionItemKey.IsAdmin] != null)
            {
                _isAdmin = Utility.GetBool(HttpContext.Current.Session[SessionItemKey.IsAdmin]);
                return _isAdmin;
            }

            return _isAdmin;
        }
        #endregion

        #region Get User SKU Type from Session
        /// <summary>
        /// Gets the user SKU Type from session.
        /// </summary>
        /// <returns></returns>
        public static string GetAdminUserSKUTypeFromSession()
        {
            string _adminSkuType = string.Empty;
            if (HttpContext.Current != null && HttpContext.Current.Session != null &&
                HttpContext.Current.Session[SessionItemKey.AdminSkuType] != null)
            {
                _adminSkuType = HttpContext.Current.Session[SessionItemKey.AdminSkuType].ToString();
                return _adminSkuType;
            }
            return _adminSkuType;
        }
        #endregion

        #region Convert to DateTime
        /// <summary>
        /// Convert to double.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static DateTime GetDateTime(object value)
        {
            DateTime result = new DateTime();
            if (value != null)
            {
                DateTime.TryParse(value.ToString(), out result);
            }
            return result;
        }
        #endregion

        #region Remove Mask from PhoneNumber
        /// <summary>
        /// Remove Mask from PhoneNumber
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string RemoveMaskfromPhoneNumber(object value)
        {
            string result = GetSringValue(value);
            if (!string.IsNullOrEmpty(result))
            {

                result = result.Replace("-", "");
                result = result.Replace(".", "");
                result = result.Replace("(", "");
                result = result.Replace(")", "");
                result = result.Replace(" ", "");

            }
            return result;
        }
        #endregion

        #region Convert to String
        /// <summary>
        /// Convert to String.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string GetSringValue(object value)
        {
            string result = string.Empty;
            if (value != null)
            {
                result = value.ToString();
            }
            return result;
        }
        #endregion

        #region Add Mobile Verify UserId
        /// <summary>
        /// Add Mobile Verify UserId
        /// </summary>
        /// <param name="userId"></param>
        public static void AddMobileVerifyUserId(long userId, string mobileNumber, string emailAddress)
        {
            //set admin details to session
            HttpContext.Current.Session[SessionItemKey.MobileVerifyUserId] = userId;
            HttpContext.Current.Session[SessionItemKey.UserMobileNumber] = mobileNumber;
            HttpContext.Current.Session[SessionItemKey.MobileVerifyEmail] = emailAddress;
        }

        #endregion


        #region Get Mobile Verify UserId from Session
        /// <summary>
        /// Get Mobile Verify UserId from Session
        /// </summary>
        /// <returns></returns>
        public static long GetMobileVerifyUserIdFromSession()
        {
            long _userId = 0;
            if (HttpContext.Current != null && HttpContext.Current.Session != null &&
                HttpContext.Current.Session[SessionItemKey.MobileVerifyUserId] != null)
            {
                _userId = Utility.GetLong(HttpContext.Current.Session[SessionItemKey.MobileVerifyUserId]);
                return _userId;
            }

            return _userId;
        }
        #endregion

        #region Get Mobile Number from Session
        /// <summary>
        /// Get Mobile Verify UserId from Session
        /// </summary>
        /// <returns></returns>
        public static string GetMobileNumberFromSession()
        {
            string _mobileNumber = "";
            if (HttpContext.Current != null && HttpContext.Current.Session != null &&
                HttpContext.Current.Session[SessionItemKey.UserMobileNumber] != null)
            {
                _mobileNumber = Utility.GetSringValue(HttpContext.Current.Session[SessionItemKey.UserMobileNumber]);
                return _mobileNumber;
            }

            return _mobileNumber;
        }
        #endregion

        #region Get Mobile Number from Session
        /// <summary>
        /// Get Mobile Verify UserId from Session
        /// </summary>
        /// <returns></returns>
        public static string GetMobileEmailFromSession()
        {
            string _email = "";
            if (HttpContext.Current != null && HttpContext.Current.Session != null &&
                HttpContext.Current.Session[SessionItemKey.MobileVerifyEmail] != null)
            {
                _email = Utility.GetSringValue(HttpContext.Current.Session[SessionItemKey.MobileVerifyEmail]);
                return _email;
            }
            return _email;
        }
        #endregion



        #region Get Mobile Number from Session
        /// <summary>
        /// Get Mobile Verify UserId from Session
        /// </summary>
        /// <returns></returns>
        public static string GetAdminRoleFromSession()
        {
            string _adminRole = "";
            if (HttpContext.Current != null && HttpContext.Current.Session != null &&
                HttpContext.Current.Session[SessionItemKey.AdminRole] != null)
            {
                _adminRole = Utility.GetSringValue(HttpContext.Current.Session[SessionItemKey.AdminRole]);
                return _adminRole;
            }
            return _adminRole;
        }
        #endregion

        #region HandleErrorsAndExceptions
        /// <summary>
        /// Handle Errors And Exceptions
        /// </summary>
        /// <param name="ModelState"></param>
        /// <param name="message"></param>
        public static void HandleErrorsAndExceptions(ModelStateDictionary ModelState, string message)
        {
            var obj = new { Message = "", Errors = new Dictionary<string, string[]>(), ExceptionMessage = "", ExceptionType = "", StackTrace = "" };
            var x = JsonConvert.DeserializeAnonymousType(message, obj);
            if (x.Errors != null)
            {
                //var errors = x.Errors.Select(kvp => new Exception(string.Format("{0}: {1}", kvp.Key, string.Join(". ", kvp.Value))));
                Utility.AddDictionaryErrorMessageToModelState(ModelState, x.Errors);
            }
            else if (!Utility.IsStringEmpty(x.ExceptionMessage))
            {

                throw new Exception("ExceptionMessage: " + x.ExceptionMessage + Environment.NewLine + "ExceptionType: " + x.ExceptionType + Environment.NewLine + "StackTrace: " + x.StackTrace);
            }
        }
        #endregion

        #region Add xVal Error Messages To ModelState
        /// <summary>
        /// Add xVal Error Messages To ModelState
        /// </summary>
        /// <param name="modelState"></param>
        /// <param name="valMessages"></param>
        public static void AddDictionaryErrorMessageToModelState(ModelStateDictionary modelState, Dictionary<string, string[]> errors)
        {
            if (errors != null && errors.Count() > 0)
            {
                foreach (var thisErrorInfo in errors)
                {
                    if (!modelState.ContainsKey(thisErrorInfo.Key))
                        modelState.AddModelError(thisErrorInfo.Key, string.Join(". ", thisErrorInfo.Value));
                }
            }
        }
        #endregion

        #region Get Import Connection String
        public static string GetImportConnectionString(string _strPath, string _fileType)
        {
            string full = Path.GetFullPath(_strPath);
            string file = Path.GetFileName(full);
            string dir = Path.GetDirectoryName(full);
            bool isJETOLDEDB = GetBool(System.Configuration.ConfigurationManager.AppSettings["InvokeJetOLEDB"].ToString());
            string connString = string.Empty;

            if (_fileType.ToLower() == ".csv")
            {
                if (isJETOLDEDB)
                {
                    connString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + dir + ";Extended Properties=\"text;HDR=YES;FMT=Delimited\"";
                }
                else
                {
                    connString = "Provider=Microsoft.ACE.OLEDB.12.0;"
                          + "Data Source=\"" + dir + "\\\";"
                          + "Extended Properties=\"Text;HDR=Yes;FMT=Delimited;IMEX=1\"";
                }
            }
            else if (_fileType.ToLower() == ".xls")
            {
                if (isJETOLDEDB)
                {
                    connString = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + _strPath + ";" + "Extended Properties=\"Excel 8.0;HDR=Yes;\"";
                }
                else
                {
                    //connString = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + _strPath + ";" + "Extended Properties=\"Excel 8.0;HDR=Yes;\"";
                    connString = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + _strPath + ";" + "Extended Properties=\"Excel 12.0;HDR=Yes;\"";
                }
            }
            else if (_fileType.ToLower() == ".xlsx")
            {
                if (isJETOLDEDB)
                {

                    connString = "Provider=Microsoft.Jet.OLEDB.4.0;" + "Data Source=" + _strPath + ";" + "Extended Properties=\"Excel 8.0;HDR=Yes;\"";
                }
                else
                {
                    connString = "Provider=Microsoft.ACE.OLEDB.12.0;" + "Data Source=" + _strPath + ";" + "Extended Properties=\"Excel 12.0;HDR=Yes;\"";
                }
            }

            return connString;
        }
        #endregion

        #region Remove Space and Special Chars
        public static string RemoveSpaceandSpecialChars(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = value.Replace("#", "");
                value = value.Replace("$", "");
                value = value.Replace("/", "");
                value = value.Replace("'", "");
                value = value.Replace(",", "");
                value = value.Replace("(", "");
                value = value.Replace(")", "");
                value = value.Replace(".", "");
                //value = value.Replace(" ", "");
                value = value.Replace("'", "");
                value = value.Replace("’", "");
            }
            return value;
        }
        #endregion

        #region Encrypt Plain Text To Cipher Text
        public static string EncryptPlainTextToCipherText(string plainText)
        {
            string SecurityKey = GetAppSettings("PaywowTAndASessionEncryptionKey");
            //Getting the bytes of Input String.
            var toEncryptedArray = Encoding.UTF8.GetBytes(plainText);
            byte[] securityKeyArray;
            using (var objMd5CryptoService = new MD5CryptoServiceProvider())
            {
                //Getting the bytes from the Security Key and Passing it to compute the Corresponding Hash Value.
                securityKeyArray = objMd5CryptoService.ComputeHash(Encoding.UTF8.GetBytes(SecurityKey));
                //De-allocating the memory after doing the Job.
                objMd5CryptoService.Clear();
            }
            byte[] resultArray;
            using (var objTripleDesCryptoService = new AesCryptoServiceProvider
            {
                Key = securityKeyArray,//Assigning the Security key to the TripleDES Service Provider.
                Mode = CipherMode.ECB,//Mode of the Crypto service is Electronic Code Book.
                Padding = PaddingMode.PKCS7//Padding Mode is PKCS7 if there is any extra byte is added.
            })
            {
                var objCrytpoTransform = objTripleDesCryptoService.CreateEncryptor();
                //Transform the bytes array to resultArray
                resultArray = objCrytpoTransform.TransformFinalBlock(toEncryptedArray, 0, toEncryptedArray.Length);
                //Releasing the Memory Occupied by TripleDES Service Provider for Encryption.
                objTripleDesCryptoService.Clear();
            }
            //Convert and return the encrypted data/byte into string format.
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        #endregion

        #region Encrypt Plain Text To Cipher Text
        public static string EPSEncryptPlainTextToCipherText(string plainText)
        {
            string SecurityKey = GetAppSettings("ExpressPaySlipSessionEncryptionKey");
            //Getting the bytes of Input String.
            var toEncryptedArray = Encoding.UTF8.GetBytes(plainText);
            byte[] securityKeyArray;
            using (var objMd5CryptoService = new MD5CryptoServiceProvider())
            {
                //Getting the bytes from the Security Key and Passing it to compute the Corresponding Hash Value.
                securityKeyArray = objMd5CryptoService.ComputeHash(Encoding.UTF8.GetBytes(SecurityKey));
                //De-allocating the memory after doing the Job.
                objMd5CryptoService.Clear();
            }
            byte[] resultArray;
            using (var objTripleDesCryptoService = new AesCryptoServiceProvider
            {
                Key = securityKeyArray,//Assigning the Security key to the TripleDES Service Provider.
                Mode = CipherMode.ECB,//Mode of the Crypto service is Electronic Code Book.
                Padding = PaddingMode.PKCS7//Padding Mode is PKCS7 if there is any extra byte is added.
            })
            {
                var objCrytpoTransform = objTripleDesCryptoService.CreateEncryptor();
                //Transform the bytes array to resultArray
                resultArray = objCrytpoTransform.TransformFinalBlock(toEncryptedArray, 0, toEncryptedArray.Length);
                //Releasing the Memory Occupied by TripleDES Service Provider for Encryption.
                objTripleDesCryptoService.Clear();
            }
            //Convert and return the encrypted data/byte into string format.
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        #endregion

        #region Encrypt Plain Text To Cipher Text
        public static string EEFEncryptPlainTextToCipherText(string plainText)
        {
            string SecurityKey = GetAppSettings("ExpressEFileSessionEncryptionKey");
            //Getting the bytes of Input String.
            var toEncryptedArray = Encoding.UTF8.GetBytes(plainText);
            byte[] securityKeyArray;
            using (var objMd5CryptoService = new MD5CryptoServiceProvider())
            {
                //Getting the bytes from the Security Key and Passing it to compute the Corresponding Hash Value.
                securityKeyArray = objMd5CryptoService.ComputeHash(Encoding.UTF8.GetBytes(SecurityKey));
                //De-allocating the memory after doing the Job.
                objMd5CryptoService.Clear();
            }
            byte[] resultArray;
            using (var objTripleDesCryptoService = new AesCryptoServiceProvider
            {
                Key = securityKeyArray,//Assigning the Security key to the TripleDES Service Provider.
                Mode = CipherMode.ECB,//Mode of the Crypto service is Electronic Code Book.
                Padding = PaddingMode.PKCS7//Padding Mode is PKCS7 if there is any extra byte is added.
            })
            {
                var objCrytpoTransform = objTripleDesCryptoService.CreateEncryptor();
                //Transform the bytes array to resultArray
                resultArray = objCrytpoTransform.TransformFinalBlock(toEncryptedArray, 0, toEncryptedArray.Length);
                //Releasing the Memory Occupied by TripleDES Service Provider for Encryption.
                objTripleDesCryptoService.Clear();
            }
            //Convert and return the encrypted data/byte into string format.
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        #endregion

        #region  Upload Image in WebStorage

        public static void UploadImageInWebStorage(string fileName, Stream stream)
        {
            AmazonS3Client client = WebStorageConnection();
            PutObjectRequest request = new PutObjectRequest();
            PutObjectRequest clientrequest = new PutObjectRequest();
            clientrequest.BucketName = Utility.GetAppSettings(Constants.BucketName);
            clientrequest.CannedACL = S3CannedACL.PublicReadWrite;
            clientrequest.Key = fileName;
            clientrequest.InputStream = stream;
            client.PutObject(clientrequest);
        }

        #endregion

        #region s3 Web Storage Connection

        public static AmazonS3Client WebStorageConnection()
        {
            string accessKey = GetAppSettings(Constants.AWSAccessKey);
            string secretKey = GetAppSettings(Constants.AWSSecretKey);

            AmazonS3Config config = new AmazonS3Config();
            config.RegionEndpoint = RegionEndpoint.USEast1;
            AmazonS3Client client = new AmazonS3Client(accessKey, secretKey, config);  // connection established

            return client;
        }

        #endregion

        #region Url of the file
        public static string GetS3FullUrl(string filePath)
        {
            AmazonS3Client client = WebStorageConnection();
            GetPreSignedUrlRequest request = new GetPreSignedUrlRequest();
            request.BucketName = GetAppSettings(Constants.BucketName);
            request.Key = filePath;
            request.Protocol = Protocol.HTTPS;
            request.Expires = DateTime.Now.AddHours(1);
            string url = client.GetPreSignedURL(request);
            url = url.Split('?')[0];
            return url;
        }
        #endregion

        #region Encrypt Plain Text To Cipher Text
        public static string PWEncryptPlainTextToCipherText(string plainText)
        {
            string SecurityKey = GetAppSettings("PayWowSessionEncryptionKey");
            //Getting the bytes of Input String.
            var toEncryptedArray = Encoding.UTF8.GetBytes(plainText);
            byte[] securityKeyArray;
            using (var objMd5CryptoService = new MD5CryptoServiceProvider())
            {
                //Getting the bytes from the Security Key and Passing it to compute the Corresponding Hash Value.
                securityKeyArray = objMd5CryptoService.ComputeHash(Encoding.UTF8.GetBytes(SecurityKey));
                //De-allocating the memory after doing the Job.
                objMd5CryptoService.Clear();
            }
            byte[] resultArray;
            using (var objTripleDesCryptoService = new AesCryptoServiceProvider
            {
                Key = securityKeyArray,//Assigning the Security key to the TripleDES Service Provider.
                Mode = CipherMode.ECB,//Mode of the Crypto service is Electronic Code Book.
                Padding = PaddingMode.PKCS7//Padding Mode is PKCS7 if there is any extra byte is added.
            })
            {
                var objCrytpoTransform = objTripleDesCryptoService.CreateEncryptor();
                //Transform the bytes array to resultArray
                resultArray = objCrytpoTransform.TransformFinalBlock(toEncryptedArray, 0, toEncryptedArray.Length);
                //Releasing the Memory Occupied by TripleDES Service Provider for Encryption.
                objTripleDesCryptoService.Clear();
            }
            //Convert and return the encrypted data/byte into string format.
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        #endregion

        #region Encrypt Plain Text To Cipher Text
        public static string PWComplianceEncryptPlainTextToCipherText(string plainText)
        {
            string SecurityKey = GetAppSettings("PayWowComplianceSessionEncryptionKey");
            //Getting the bytes of Input String.
            var toEncryptedArray = Encoding.UTF8.GetBytes(plainText);
            byte[] securityKeyArray;
            using (var objMd5CryptoService = new MD5CryptoServiceProvider())
            {
                //Getting the bytes from the Security Key and Passing it to compute the Corresponding Hash Value.
                securityKeyArray = objMd5CryptoService.ComputeHash(Encoding.UTF8.GetBytes(SecurityKey));
                //De-allocating the memory after doing the Job.
                objMd5CryptoService.Clear();
            }
            byte[] resultArray;
            using (var objTripleDesCryptoService = new AesCryptoServiceProvider
            {
                Key = securityKeyArray,//Assigning the Security key to the TripleDES Service Provider.
                Mode = CipherMode.ECB,//Mode of the Crypto service is Electronic Code Book.
                Padding = PaddingMode.PKCS7//Padding Mode is PKCS7 if there is any extra byte is added.
            })
            {
                var objCrytpoTransform = objTripleDesCryptoService.CreateEncryptor();
                //Transform the bytes array to resultArray
                resultArray = objCrytpoTransform.TransformFinalBlock(toEncryptedArray, 0, toEncryptedArray.Length);
                //Releasing the Memory Occupied by TripleDES Service Provider for Encryption.
                objTripleDesCryptoService.Clear();
            }
            //Convert and return the encrypted data/byte into string format.
            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }
        #endregion

        #region Encrypt Plain Text To Cipher Text By Security Key        
        /// <summary>
        /// Encrypts the plain text to cipher text by security key.
        /// </summary>
        /// <param name="plainText">The plain text.</param>
        /// <param name="securityKey">The security key.</param>
        /// <returns></returns>
        public static string EncryptPlainTextToCipherTextBySecurityKey(string plainText, string securityKey)
        {
            if (!string.IsNullOrEmpty(plainText) && !string.IsNullOrEmpty(securityKey))
            {
                //Getting the bytes of Input String.
                var toEncryptedArray = Encoding.UTF8.GetBytes(plainText);
                byte[] securityKeyArray;
                using (var objMd5CryptoService = new MD5CryptoServiceProvider())
                {
                    //Getting the bytes from the Security Key and Passing it to compute the Corresponding Hash Value.
                    securityKeyArray = objMd5CryptoService.ComputeHash(Encoding.UTF8.GetBytes(securityKey));
                    //De-allocating the memory after doing the Job.
                    objMd5CryptoService.Clear();
                }
                byte[] resultArray;
                using (var objTripleDesCryptoService = new AesCryptoServiceProvider
                {
                    Key = securityKeyArray,//Assigning the Security key to the TripleDES Service Provider.
                    Mode = CipherMode.ECB,//Mode of the Crypto service is Electronic Code Book.
                    Padding = PaddingMode.PKCS7//Padding Mode is PKCS7 if there is any extra byte is added.
                })
                {
                    var objCrytpoTransform = objTripleDesCryptoService.CreateEncryptor();
                    //Transform the bytes array to resultArray
                    resultArray = objCrytpoTransform.TransformFinalBlock(toEncryptedArray, 0, toEncryptedArray.Length);
                    //Releasing the Memory Occupied by TripleDES Service Provider for Encryption.
                    objTripleDesCryptoService.Clear();
                }
                //Convert and return the encrypted data/byte into string format.
                return Convert.ToBase64String(resultArray, 0, resultArray.Length);
            }
            return string.Empty;
        }
        #endregion

        #region Download s3 file as byte array
        public static byte[] DownloadS3File(string key)
        {
            byte[] bytes = null;
            AmazonS3Client s3client = Utility.WebStorageConnection();
            MemoryStream file = new MemoryStream();
            string s3BaseUrl = GetAppSettings(Constants.S3BaseUrl);
            string s3AnotherFileBaseUrl = GetAppSettings(Constants.S3AnotherFileBaseUrl);
            if (key.Contains(s3BaseUrl))
            {
                key = key.Replace(s3BaseUrl, "");
            }
            if (key.Contains(s3AnotherFileBaseUrl))
            {
                key = key.Replace(s3AnotherFileBaseUrl, "");
            }
            GetObjectResponse objres = s3client.GetObject(new GetObjectRequest()
            {
                BucketName = Utility.GetAppSettings(Constants.BucketName),
                Key = key
            });
            try
            {
                BufferedStream stream2 = new BufferedStream(objres.ResponseStream);
                byte[] buffer = new byte[0x2000];
                int count = 0;
                while ((count = stream2.Read(buffer, 0, buffer.Length)) > 0)
                {
                    file.Write(buffer, 0, count);
                }
                bytes = file.ToArray();
            }
            finally
            {
                s3client.Dispose();
                file.Dispose();
                objres.Dispose();
            }
            return bytes;
        }
        #endregion

        #region Get Two Factor Key With Email
        /// <summary>
        /// Get Two Factor Key With Email
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public static string GetTwoFactorKeyWithEmail(string emailAddress)
        {
            return $"{Utility.GetAppSettings(Constants.TwoFactorKey)}+{emailAddress}";
        }
        #endregion
    }
}