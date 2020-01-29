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

        public override HtmlNode Connect(string url)
        {
            try
            {
                string htmlCode = "";
                using (WebClient client = new WebClient())
                {
                    client.Encoding = System.Text.Encoding.GetEncoding("UTF-8");
                    client.Headers.Add(HttpRequestHeader.UserAgent, "AvoidError");
                    htmlCode = client.DownloadString(url);
                }
                doc = new HtmlAgilityPack.HtmlDocument();
                doc.LoadHtml(htmlCode);
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
    }
}
