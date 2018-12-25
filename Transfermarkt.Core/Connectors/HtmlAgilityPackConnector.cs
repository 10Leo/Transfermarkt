using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.Contracts;

namespace Transfermarkt.Core.Connectors
{
    public class HtmlAgilityPackConnector : IConnector
    {

        private HtmlDocument doc;

        public void ConnectToPage(string url)
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
                Debug.WriteLine(ex.StackTrace);
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.StackTrace);
                throw;
            }

            Validate();
        }

        private void Validate()
        {
            if (doc.ParseErrors != null && doc.ParseErrors.Count() > 0)
            {
                // Handle any parse errors as required
                throw new Exception("Erros no parse do doc");
            }

            if (doc.DocumentNode == null)
            {
                throw new Exception("Document Node null");
            }
        }
    }
}
