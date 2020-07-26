using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Core.ParseHandling
{
    public static class Connector
    {
        public static string Connect(string url)
        {
            string htmlString = string.Empty;

            try
            {
                using (WebClient client = new WebClient())
                {
                    Uri uri = new Uri(url);
                    client.Encoding = System.Text.Encoding.GetEncoding("UTF-8");
                    client.Headers.Add(HttpRequestHeader.UserAgent, "AvoidError");
                    htmlString = client.DownloadString(uri);
                }
            }
            catch (WebException ex)
            {
                throw new Exception("Error accessing internet. '{url}'", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Error connecting to '{url}'.", ex);
            }

            return htmlString;
        }
    }
}
