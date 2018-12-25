using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Transfermarkt.Core.Connectors.Util
{
    public static class AgilityPackUtil
    {
        public static T GetAttributeValue<T>(this HtmlNode td, string key, T defaultValue) where T : IConvertible
        {
            T result = defaultValue;//default(T);

            if (td.Attributes[key] == null)// || String.IsNullOrEmpty(td.Attributes[key].Value) == false)
            {
                return defaultValue;
            }

            string value = td.Attributes[key].Value;

            try
            {
                result = (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                result = defaultValue;//default(T);
            }

            return result;
        }
    }
}
