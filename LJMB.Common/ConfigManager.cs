using System;
using System.Configuration;

namespace LJMB.Common
{
    public static class ConfigManager
    {
        public static T GetAppSetting<T>(string key) where T : IConvertible
        {
            T result = default(T);

            if (ConfigurationManager.AppSettings[key] == null)// || String.IsNullOrEmpty(td.Attributes[key].Value) == false)
            {
                return result;
            }

            string value = ConfigurationManager.AppSettings[key];

            try
            {
                result = (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                result = default(T);
            }

            return result;
        }
    }
}
