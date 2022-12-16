using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Main.Control.Services.Utilities;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon;

namespace Main.Control.Services.Utilities
{
    public class Utilities
    {
        #region EncryptPassword
        /// <summary>
        /// Encrypts the password.
        /// </summary>
        /// <param name="pPassword">The password.</param>
        /// <param name="pSalt">The salt.</param>
        /// <returns></returns>
        public static string EncryptPassword(string pPassword, string pSalt)
        {
            string saltAndPwd = String.Concat(pPassword, pSalt);
            string hashedPwd = FormsAuthentication.HashPasswordForStoringInConfigFile(saltAndPwd, "SHA1");
            return hashedPwd;

        }
        #endregion

        #region Create Salt
        /// <summary>
        /// Creates the salt.
        /// </summary>
        /// <param name="saltSize">Size of the salt.</param>
        /// <returns></returns>
        public static string CreateSalt(int saltSize)
        {
            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();
            byte[] buff = new byte[saltSize];
            rng.GetBytes(buff);
            return Convert.ToBase64String(buff);
        }
        #endregion

        #region Encrypt
        /// <summary>
        /// Encrypts the specified password text.
        /// </summary>
        /// <param name="pstrText">The password text.</param>
        /// <returns></returns>
        public static string Encrypt(string password, string salt)
        {
            string pstrEncrKey = salt;
            byte[] byKey = { };
            byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
            byKey = System.Text.Encoding.UTF8.GetBytes(pstrEncrKey.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.UTF8.GetBytes(password);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            return Convert.ToBase64String(ms.ToArray());
        }
        #endregion

        #region Decrypt
        /// <summary>
        /// Decrypts the specified password.
        /// </summary>
        /// <param name="password">The password.</param>
        /// <param name="salt">The salt.</param>
        /// <returns></returns>
        public static string Decrypt(string password, string salt)
        {
            password = password.Replace(" ", "+");
            string pstrDecrKey = salt;
            byte[] byKey = { };
            byte[] IV = { 18, 52, 86, 120, 144, 171, 205, 239 };
            byte[] inputByteArray = new byte[password.Length];

            byKey = System.Text.Encoding.UTF8.GetBytes(pstrDecrKey.Substring(0, 8));
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            inputByteArray = Convert.FromBase64String(password);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            System.Text.Encoding encoding = System.Text.Encoding.UTF8;
            return encoding.GetString(ms.ToArray());
        }
        #endregion

        #region Create Random Number
        /// <summary>
        /// Creating the Random Number
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int RandomNumber(int min, int max)
        {
            Random random = new Random();
            //returns the generated Number
            return random.Next(min, max);
        }
        #endregion

        #region Random String
        /// <summary>
        /// Generates a random string with the given length
        /// </summary>
        /// <param name="size">Size of the string</param>
        /// <param name="lowerCase">If true, generate lowercase string</param>
        /// <returns>Random string</returns>
        public static string RandomString(int size, bool lowerCase)
        {
            //Generating the Random string
            StringBuilder builder = new StringBuilder();
            Random random = new Random();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }
            if (lowerCase)
                return builder.ToString().ToLower();
            return builder.ToString();
        }
        #endregion

        #region Get Password
        /// <summary> 
        /// Get password
        /// </summary>
        /// <returns></returns>
        public static string GetPassword()
        {
            //Get the password with Random Number and String
            StringBuilder builder = new StringBuilder();
            //   builder.Append(Utilities.RandomString(4, true));
            builder.Append(Utilities.RandomNumber(100000, 999999));
            //   builder.Append(Utilities.RandomString(2, false));
            return builder.ToString();
        }
        #endregion

        #region Generate Verification Code
        /// <summary>
        /// Generates the Verification Code
        /// </summary>
        /// <param name="size">No. of chars to be generated in the Verification Code.</param>
        /// <returns>Returns the randomly generated Verification Code.</returns>
        public static string GetVerificationCode(int size)
        {
            int PasswordLength = 20;

            String _allowedChars = "123456789";
            Byte[] randomBytes = new Byte[PasswordLength];

            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

            // Get Bytes
            rng.GetBytes(randomBytes);

            char[] chars = new char[PasswordLength];

            int allowedCharCount = _allowedChars.Length;

            for (int i = 0; i < PasswordLength; i++)
            {
                chars[i] = _allowedChars[(int)randomBytes[i] % allowedCharCount];
            }

            return new string(chars);
        }
        #endregion

        #region Compare Text
        /// <summary>
        /// Compate two text for equivalence
        /// </summary>
        /// <param name="value"></param>
        /// <param name="valueToCompare"></param>
        /// <returns></returns>
        public static bool CompareText(string value, string valueToCompare)
        {
            //check whether two strings are same
            if (value != valueToCompare)
                return false;
            else
                return true; ;
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

        #region Get IP Address
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

        #region Convert To GMT
        /// <summary>
        /// Convert the User's Time to GMT.
        /// </summary>
        /// <param name="timeToConvert">The time to convert.</param>
        /// <param name="sourceTimeZone">The source time zone.</param>
        /// <returns></returns>
        public static DateTime ConvertToGMT(DateTime timeToConvert)
        {
            TimeZoneInfo _defaultTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Central Standard Time");
            return Utilities.ConvertToGMT(timeToConvert, _defaultTimeZone);
        }

        public static DateTime ConvertToGMT(DateTime timeToConvert, TimeZoneInfo sourceTimeZone)
        {
            TimeZoneInfo _gMTTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Greenwich Standard Time");
            return TimeZoneInfo.ConvertTime(timeToConvert, sourceTimeZone, _gMTTimeZone);
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

        #region Get the Month Difference
        /// <summary>
        /// Get the Month Difference
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public static int MonthDifference(DateTime startDate, DateTime endDate)
        {
            int monthsApart = 12 * (startDate.Year - endDate.Year) + startDate.Month - endDate.Month;
            return Math.Abs(monthsApart);
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

        #region Read to End 
        /// <summary>
        /// Read to End 
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static byte[] ReadToEnd(System.IO.Stream stream)
        {
            long originalPosition = stream.Position;
            stream.Position = 0;

            try
            {
                byte[] readBuffer = new byte[4096];

                int totalBytesRead = 0;
                int bytesRead;

                while ((bytesRead = stream.Read(readBuffer, totalBytesRead, readBuffer.Length - totalBytesRead)) > 0)
                {
                    totalBytesRead += bytesRead;

                    if (totalBytesRead == readBuffer.Length)
                    {
                        int nextByte = stream.ReadByte();
                        if (nextByte != -1)
                        {
                            byte[] temp = new byte[readBuffer.Length * 2];
                            Buffer.BlockCopy(readBuffer, 0, temp, 0, readBuffer.Length);
                            Buffer.SetByte(temp, totalBytesRead, (byte)nextByte);
                            readBuffer = temp;
                            totalBytesRead++;
                        }
                    }
                }

                byte[] buffer = readBuffer;
                if (readBuffer.Length != totalBytesRead)
                {
                    buffer = new byte[totalBytesRead];
                    Buffer.BlockCopy(readBuffer, 0, buffer, 0, totalBytesRead);
                }
                return buffer;
            }
            finally
            {
                stream.Position = originalPosition;
            }
        }
        #endregion

        #region Get Clean File Name
        public static string GetCleanFileName(string text)
        {
            StringBuilder sb = new StringBuilder();
            var lastWasInvalid = false;
            foreach (char c in text)
            {
                if (char.IsLetterOrDigit(c) || c == '.')
                {
                    sb.Append(c);
                    lastWasInvalid = false;
                }
                else
                {
                    if (!lastWasInvalid)
                    {
                        sb.Append("_");
                    }
                    lastWasInvalid = true;
                }
            }

            return sb.ToString().ToLowerInvariant().Trim();
        }
        #endregion

        #region Create/Get The Download Folder
        /// <summary>
        /// Creates the download folder after delete.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public static string CreateDownloadFolderAfterDelete(string fileName, long userId)
        {
            string _filePath = ConfigurationManager.AppSettings["UserDownloadsPath"].ToString();
            if (!Directory.Exists(_filePath))
            {
                Directory.CreateDirectory(_filePath);
            }
            _filePath = Path.Combine(_filePath, userId.ToString().PadLeft(6, '0'));
            if (!Directory.Exists(_filePath))
            {
                Directory.CreateDirectory(_filePath);
            }
            _filePath = Path.Combine(_filePath, fileName);
            if (File.Exists(_filePath))
            {
                File.SetAttributes(_filePath, FileAttributes.Normal);
                FileInfo _chkFileInfo = new FileInfo(_filePath);
                if (!IsFileLocked(_chkFileInfo))
                {
                    File.Delete(_filePath);

                    FileInfo _fileInfo = new FileInfo(_filePath);
                    FileStream _fileStream = _fileInfo.Create();
                    _fileStream.Flush();
                    _fileStream.Close();
                    _fileStream.Dispose();
                }
            }
            else
            {
                FileInfo _fileInfo = new FileInfo(_filePath);
                FileStream _fileStream = _fileInfo.Create();
                _fileStream.Flush();
                _fileStream.Close();
                _fileStream.Dispose();
            }
            //File.SetAttributes(_filePath, FileAttributes.Normal);
            return _filePath;
        }
        #endregion

        #region Create/Get The Download Folder
        /// <summary>
        /// Creates the download folder after delete.
        /// </summary>
        /// <param name="fileName">Name of the file.</param>
        /// <param name="userId">The user id.</param>
        /// <returns></returns>
        public static string CreateTempDownloadFolderAfterDelete(string fileName, long userId)
        {
            string _filePath = ConfigurationManager.AppSettings["TempDownloadsPath"].ToString();
            if (!Directory.Exists(_filePath))
            {
                Directory.CreateDirectory(_filePath);
            }
            _filePath = Path.Combine(_filePath, "temp");
            if (!Directory.Exists(_filePath))
            {
                Directory.CreateDirectory(_filePath);
            }
            _filePath = Path.Combine(_filePath, userId.ToString().PadLeft(6, '0'));
            if (!Directory.Exists(_filePath))
            {
                Directory.CreateDirectory(_filePath);
            }
            _filePath = Path.Combine(_filePath, fileName);
            if (File.Exists(_filePath))
            {
                File.SetAttributes(_filePath, FileAttributes.Normal);
                FileInfo _chkFileInfo = new FileInfo(_filePath);
                if (!IsFileLocked(_chkFileInfo))
                {
                    File.Delete(_filePath);

                    FileInfo _fileInfo = new FileInfo(_filePath);
                    FileStream _fileStream = _fileInfo.Create();
                    _fileStream.Flush();
                    _fileStream.Close();
                    _fileStream.Dispose();
                }
            }
            else
            {
                FileInfo _fileInfo = new FileInfo(_filePath);
                FileStream _fileStream = _fileInfo.Create();
                _fileStream.Flush();
                _fileStream.Close();
                _fileStream.Dispose();
            }
            //File.SetAttributes(_filePath, FileAttributes.Normal);
            return _filePath;
        }
        #endregion

        #region Is File Locked
        public static bool IsFileLocked(FileInfo file)
        {
            FileStream stream = null;

            try
            {
                stream = file.Open(FileMode.Open, FileAccess.ReadWrite, FileShare.None);
            }
            catch (IOException)
            {
                //the file is unavailable because it is:
                //still being written to
                //or being processed by another thread
                //or does not exist (has already been processed)
                return true;
            }
            finally
            {
                if (stream != null)
                    stream.Close();
            }

            //file is not locked
            return false;
        }
        #endregion

        #region Serialize
        /// <summary>
        /// Serializes the specified seralized object.
        /// </summary>
        /// <param name="seralizedObject">The seralized object.</param>
        /// <returns></returns>
        public static string Serialize(object seralizedObject)
        {
            string serializedData = string.Empty;
            if (seralizedObject != null)
            {
                MemoryStream memoryStream = new MemoryStream();
                XmlSerializer xs = new XmlSerializer((seralizedObject).GetType());
                XmlTextWriter xmlTextWriter = new XmlTextWriter(memoryStream, Encoding.UTF8);

                xs.Serialize(xmlTextWriter, seralizedObject);
                memoryStream = (MemoryStream)xmlTextWriter.BaseStream;
                serializedData = UTF8ByteArrayToString(memoryStream.ToArray());
                return serializedData;
            }
            return serializedData;
        }

        /// <summary>
        /// To convert a Byte Array of Unicode values (UTF-8 encoded) to a complete String.
        /// </summary>
        /// <param name="characters">Unicode Byte Array to be converted to String</param>
        /// <returns>String converted from Unicode Byte Array</returns>
        private static string UTF8ByteArrayToString(Byte[] characters)
        {
            UTF8Encoding encoding = new UTF8Encoding();
            string constructedString = encoding.GetString(characters);
            return (constructedString);
        }
        #endregion

        #region Convert Time Format
        public static string ConvertTimeFormat(DateTime _date, string _format)
        {
            return ((DateTime)_date).ToString(_format);
        }
        #endregion

        public static int GetMonthDifference(DateTime startDate,
                                     DateTime endDate)
        {
            // if dates were passed in wrong order, swap 'em
            if (startDate > endDate)
            {
                var temp = startDate;
                startDate = endDate;
                endDate = temp;
            }

            var count = 0;
            var tempDate = startDate;

            while ((tempDate = GetNextMonth(tempDate)) <= endDate)
            {
                count++;
            }

            System.Console.WriteLine("From {0} to {1} is {2} month{3}.",
              startDate.ToShortDateString(), endDate.ToShortDateString(),
              count, count == 1 ? "" : "s");

            return count;
        }

        public static DateTime GetNextMonth(DateTime date)
        {
            var month = date.Month;
            var day = date.Day;
            var year = date.Year;

            var nextDateMonth = month == 12 ? 1 : month + 1;
            var nextDateYear = month == 12 ? year + 1 : year;

            DateTime nextDate;

            while (!DateTime.TryParse(nextDateMonth + "/" + day
              + "/" + nextDateYear, out nextDate))
            {
                // if it didn't parse right, 
                // then the month must not have that many days
                day--;
            }

            return nextDate;
        }


        #region Get PDF Key
        /// <summary>
        /// Get PDF Key
        /// </summary>
        /// <returns></returns>
        public static string GetPDFKey()
        {
            string pdfkey = GetAppSettings("ASPPdfKey");
            MainWebService _spanWebService = new MainWebService();
            if (!string.IsNullOrEmpty(_spanWebService.PdfKey()))
            {
                pdfkey = _spanWebService.PdfKey();
            }
            else
            {
                pdfkey = ConfigurationManager.AppSettings["ASPPdfKey"];
            }
            return pdfkey;
        }
        #endregion

        #region Generate Mobile Verification Code
        /// <summary>
        /// Generates the Mobile Verification Code
        /// </summary>
        /// <returns>Returns the randomly generated Verification Code.</returns>
        public static string GetMobileVerificationCode()
        {
            int PasswordLength = 4;

            String _allowedChars = "123456789";
            Byte[] randomBytes = new Byte[PasswordLength];

            RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider();

            // Get Bytes
            rng.GetBytes(randomBytes);

            char[] chars = new char[PasswordLength];

            int allowedCharCount = _allowedChars.Length;

            for (int i = 0; i < PasswordLength; i++)
            {
                chars[i] = _allowedChars[(int)randomBytes[i] % allowedCharCount];
            }

            return new string(chars);
        }
        #endregion

        public static List<string> GetEmailList(string emailAddress)
        {
            List<string> emails = new List<string>();
            if (!string.IsNullOrWhiteSpace(emailAddress))
            {
                emails = emailAddress.Split(';').ToList();
            }
            emails.RemoveAll(x => string.IsNullOrWhiteSpace(x));
            return emails;
        }

        #region  Upload Image in WebStorage

        public static void UploadImageInWebStorage(string fileName, Stream stream)
        {
            AmazonS3Client client = WebStorageConnection();
            PutObjectRequest request = new PutObjectRequest();
            PutObjectRequest clientrequest = new PutObjectRequest();
            clientrequest.BucketName = GetAppSettings("BucketName");
            clientrequest.CannedACL = S3CannedACL.PublicReadWrite;
            clientrequest.Key = fileName;
            clientrequest.InputStream = stream;
            client.PutObject(clientrequest);
        }

        #endregion

        #region s3 Web Storage Connection

        public static AmazonS3Client WebStorageConnection()
        {
            string accessKey = GetAppSettings("AWSAccessKey");
            string secretKey = GetAppSettings("AWSSecretKey");

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
            request.BucketName = GetAppSettings("BucketName");
            request.Key = filePath;
            request.Protocol = Protocol.HTTPS;
            request.Expires = DateTime.Now.AddHours(1);
            string url = client.GetPreSignedURL(request);
            url = url.Split('?')[0];
            return url;
        }
        #endregion

        #region Download s3 file as byte array
        public static byte[] DownloadS3File(string key)
        {
            byte[] bytes = null;
            AmazonS3Client s3client = Utilities.WebStorageConnection();
            MemoryStream file = new MemoryStream();
            GetObjectResponse objres = s3client.GetObject(new GetObjectRequest()
            {
                BucketName = Utilities.GetAppSettings("ETTBucketName"),
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

        #region Convert to decimal 2 Digits

        public static decimal Get2DecimalPoints(object Value)
        {
            decimal result = decimal.MinValue;
            if (Value != null)
            {
                string strVal = Value.ToString();
                var strDecVal = strVal.Insert(strVal.Length - 2, ".");
                result = Utilities.GetDecimal2Digits(strDecVal);
            }
            return result;
        }
        #endregion
    }
}
