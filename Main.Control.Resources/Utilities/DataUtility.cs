using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Main.Control.Resources.Utilities
{
    public class DataUtility
    {
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

        #region Convert to String
        /// <summary>
        /// Convert to String.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static string GetStringValue(object value)
        {
            string result = string.Empty;
            if (value != null)
            {
                result = value.ToString();
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

    }
}
