using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling
{
    public class HAPConnection : Connection<HtmlNode>
    {
        public HtmlDocument doc = null;

        public override bool IsConnected => doc != null;

        public override HtmlNode Connect(string url)
        {
            try
            {
                string htmlString = "";
                //TODO: consider separate the loading of the file from it's loading by HAP.
                using (WebClient client = new WebClient())
                {
                    Uri uri = new Uri(url);
                    client.Encoding = System.Text.Encoding.GetEncoding("UTF-8");
                    client.Headers.Add(HttpRequestHeader.UserAgent, "AvoidError");
                    htmlString = client.DownloadString(uri);
                }
                doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlString);
            }
            catch (System.Net.WebException ex)
            {
                //Debug.WriteLine(ex.StackTrace);
                System.Environment.Exit(-1);
            }
            catch (Exception ex)
            {
                //Debug.WriteLine(ex.StackTrace);
            }

            return doc.DocumentNode;
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
