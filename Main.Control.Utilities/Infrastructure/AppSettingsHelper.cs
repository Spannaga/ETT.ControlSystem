using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace Main.Control.Utilities
{
    public class AppSettingsHelper
    {
        private readonly NameValueCollection appSettings;
        private NameValueCollection nameValueCollection;

        public AppSettingsHelper()
        {
            this.appSettings = new NameValueCollection();
        }

        public AppSettingsHelper(NameValueCollection nameValueCollection)
        {
            // TODO: Complete member initialization
            this.nameValueCollection = nameValueCollection;
        }

        public string GetString(string name)
        {
            return getValue(name, true, null);
        }

        public string GetString(string name, string defaultValue)
        {
            return getValue(name, false, defaultValue);
        }

        public string[] GetStringArray(string name, string separator)
        {
            return getStringArray(name, separator, true, null);
        }

        public string[] GetStringArray(string name, string separator, string[] defaultValue)
        {
            return getStringArray(name, separator, false, defaultValue);
        }

        private string[] getStringArray(string name, string separator, bool requireValue, string[] defaultValue)
        {
            string value = getValue(name, requireValue, null);

            if (value != null)
                return value.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries);

            return defaultValue;
        }

        //INFO: As other types of fields are needed, add strongly typed methods like "GetBoolean"

        private string getValue(string name, bool requireValue, string defaultValue)
        {
            string value = appSettings[name];

            if (value != null)
                return value;

            if (requireValue)
                throw new InvalidOperationException(string.Format("Could not find required app setting '{0}'", name));

            return defaultValue;
        }
    }
}
