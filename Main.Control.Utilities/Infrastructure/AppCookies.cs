using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Globalization;
using System.Web;
using System.Collections;


namespace Main.Control.Utilities.Infrastructure
{

    public class AppCookies
    {
        #region Declaration
        private static HttpRequestBase _request;
        private static HttpResponseBase _response;
        #endregion

        public static bool Exists(string key)
        {
            var httpContext = new HttpContextWrapper(HttpContext.Current);
            _request = httpContext.Request;
            _response = httpContext.Response;
            Check.IsNotEmpty(key, "key");

            return _request.Cookies[key] != null;
        }

        public static void Remove(string key)
        {
            var httpContext = new HttpContextWrapper(HttpContext.Current);
            _request = httpContext.Request;
            _response = httpContext.Response;
            Check.IsNotNull(_request, "request");
            Check.IsNotNull(_response, "response");

            Check.IsNotEmpty(key, "key");
            // Set the value of the cookie to null and
            // set its expiration to some time in the past
            if (_request != null && _request.Cookies[key] != null)
            {
                _request.Cookies[key].Value = null;
                _request.Cookies[key].Expires = System.DateTime.Now.AddMonths(-1); // last month
                HttpCookie cookie = new HttpCookie(key)
                {
                    Expires = DateTime.Now.AddDays(-1) // or any other time in the past
                };
                _request.Cookies.Set(cookie);
            }
            if (_response.Cookies[key] != null)
            {
                _response.Cookies[key].Value = null;
                _response.Cookies[key].Expires = System.DateTime.Now.AddMonths(-1); // last month
                HttpCookie cookie = new HttpCookie(key)
                {
                    Expires = DateTime.Now.AddDays(-1) // or any other time in the past
                };
                _response.Cookies.Set(cookie);
            }
        }

        public static void Clear()
        {
            var httpContext = new HttpContextWrapper(HttpContext.Current);
            _request = httpContext.Request;
            _response = httpContext.Response;
            Check.IsNotNull(_request, "request");
            Check.IsNotNull(_response, "response");
            foreach (var _cookie in _request.Cookies)
            {

            }
            _request.Cookies.Clear();
            _response.Cookies.Clear();
        }

        public static string GetValue(string key)
        {
            var httpContext = new HttpContextWrapper(HttpContext.Current);
            _request = httpContext.Request;
            _response = httpContext.Response;
            Check.IsNotNull(_request, "request");
            Check.IsNotNull(_response, "response");

            Check.IsNotEmpty(key, "key");

            HttpCookie cookie = _request.Cookies[key];
            return cookie != null ? cookie.Value : null;
        }

        public static T GetValue<T>(string key)
        {
            string val = GetValue(key);

            if (val == null)
                return default(T);

            Type type = typeof(T);
            bool isNullable = type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
            if (isNullable)
                type = new NullableConverter(type).UnderlyingType;

            return (T)Convert.ChangeType(val, type, CultureInfo.InvariantCulture);
        }

        public static void SetValue(string key, object value)
        {
            var httpContext = new HttpContextWrapper(HttpContext.Current);
            _request = httpContext.Request;
            _response = httpContext.Response;
            Check.IsNotNull(_request, "request");
            Check.IsNotNull(_response, "response");

            Check.IsNotEmpty(key, "key");

            string strValue = CheckAndConvertValue(value);

            HttpCookie cookie = new HttpCookie(key, strValue);
            _response.Cookies.Set(cookie);
        }

        public static void SetValue(string key, object value, DateTime expires)
        {
            var httpContext = new HttpContextWrapper(HttpContext.Current);
            _request = httpContext.Request;
            _response = httpContext.Response;
            Check.IsNotNull(_request, "request");
            Check.IsNotNull(_response, "response");

            Check.IsNotEmpty(key, "key");

            string strValue = CheckAndConvertValue(value);

            HttpCookie cookie = new HttpCookie(key, strValue) { Expires = expires };
            _response.Cookies.Set(cookie);
        }

        private static string CheckAndConvertValue(object value)
        {
            if (value == null)
                return null;

            if (value is string)
                return value.ToString();

            // only allow value types and nullable<value types>

            Type type = value.GetType();
            bool isTypeAllowed = false;

            if (type.IsValueType)
                isTypeAllowed = true;
            else
            {
                bool isNullable = type.IsGenericType && type.GetGenericTypeDefinition().Equals(typeof(Nullable<>));
                if (isNullable)
                {
                    NullableConverter converter = new NullableConverter(type);
                    Type underlyingType = converter.UnderlyingType;
                    if (underlyingType.IsValueType)
                        isTypeAllowed = true;
                }
            }

            if (!isTypeAllowed)
                throw new NotSupportedException("Only value types and Nullable<ValueType> are allowed!");

            return (string)Convert.ChangeType(value, typeof(string), CultureInfo.InvariantCulture);
        }

        public static string CookieSession
        {
            get { return GetValue("CookieSession"); }
            set { SetValue("CookieSession", value); }
        }
    }
}
