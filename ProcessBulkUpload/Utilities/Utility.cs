
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ProcessBulkUpload.Utilities
{
    public static class Utility
    {

        #region Convert to double
        public static double GetDouble(object Value)
        {
            double result = 0;
            if (Value != null)
            {
                double.TryParse(Value.ToString(), out result);
            }
            return result;
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

        #region Convert to decimal 2 Digits

        public static decimal GetDecimal2Digits(object Value)
        {
            decimal result = decimal.MinValue;
            if (Value != null)
            {
                decimal.TryParse(Value.ToString(), out result);
                result = Math.Round(result, 2);
                decimal.TryParse(result.ToString("#.00"), out result);
            }

            return result;
        }
        #endregion

        #region Get Full Name
        public static string GetFullName(string FirstName, string MiddleIntial, string LastName, string Suffix)
        {
            string fullName = string.Empty;

            if (!string.IsNullOrWhiteSpace(FirstName))
            {
                fullName = FirstName.Trim();
            }
            if (!string.IsNullOrWhiteSpace(MiddleIntial))
            {
                fullName += (" " + MiddleIntial.Trim());
            }
            if (!string.IsNullOrWhiteSpace(LastName))
            {
                fullName += (" " + LastName.Trim());
            }
            if (!string.IsNullOrWhiteSpace(Suffix))
            {
                fullName += (" " + Suffix.Trim());
            }

            return !string.IsNullOrWhiteSpace(fullName) ? fullName.Trim() : string.Empty;
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

        #region Get Import Connection String
        public static string GetImportConnectionString(string _strPath, string _fileType)
        {
            string full = Path.GetFullPath(_strPath);
            string file = Path.GetFileName(full);
            string dir = Path.GetDirectoryName(full);
            bool isJETOLDEDB = GetBool(ConfigurationManager.AppSettings["InvokeJetOLEDB"].ToString());
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

        public static string RemoveSpecialChars(string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                value = value.Replace("#", "");
                value = value.Replace("$", "");
                value = value.Replace("'", "");
                value = value.Replace(",", "");
                value = value.Replace("(", "");
                value = value.Replace(")", "");
                value = value.Replace(".", "");
                value = value.Replace("  ", "");
                value = value.Replace("'", "");
                value = value.Replace("’", "");
            }
            return value;
        }
        #region Convert to Long
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
    }
}
