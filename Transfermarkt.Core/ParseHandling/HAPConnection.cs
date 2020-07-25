using HtmlAgilityPack;
using Page.Scraper.Contracts;
using System;
using System.Net;

namespace Transfermarkt.Core.ParseHandling
{
    public class HAPConnection : Connection<HtmlNode>
    {
        public HtmlDocument doc = null;

        public override bool IsConnected => doc != null;

        public override HtmlNode Connect(string url)
        {
            string htmlString = Connector.Connect(url);

            doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(htmlString);

            return doc?.DocumentNode;
        }

        public override void Reset()
        {
            doc = null;
        }
    }

    static class HtmlAgilityPackUtil
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
