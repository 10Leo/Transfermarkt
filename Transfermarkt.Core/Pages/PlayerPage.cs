using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.Contracts;
using Transfermarkt.Core.Parsers.Player;

namespace Transfermarkt.Core.Pages
{
    public class PlayerPage : IPage<HtmlNode, string>
    {
        private HtmlDocument doc;
        private readonly string url;
        private Player player = new Player();

        public IList<IElementParser<HtmlNode, string>> Elements { get; set; } = new List<IElementParser<HtmlNode, string>>();

        public PlayerPage(string url)
        {
            this.url = url;

            Connect();

            RegisterParsers();
        }

        #region Contract

        public void Parse()
        {
            HtmlNode table = GetTable();
            if (table == null)
            {
                return;
            }

            var headers = table.SelectNodes(".//thead/tr[th]");
            var rows = table.SelectNodes(".//tbody/tr[td]");
            HtmlNodeCollection headerCols = headers[0].SelectNodes("th");

            foreach (var row in rows)
            {
                //each column is an attribute
                HtmlNodeCollection cols = row.SelectNodes("td");

                for (int i = 0; i < cols.Count; i++)
                {
                    Elements.ToList().ForEach(e => {
                        var header = headerCols[i];
                        var element = cols[i];
                        if (e.CanParse(header))
                        {
                            if(e is IMarketValueParser<HtmlNode>)
                            {
                                //player.MarketValue = e.Parse(element);
                            }
                        }
                    });
                }
            }
        }

        public void Save()
        {
        }

        #endregion

        private void RegisterParsers()
        {
            //TODO: Register parsers or let them regist themselfes?
            var eParser = new MarketValueParser();
            eParser.OnSuccess += (e, a) => Console.WriteLine(".");
            eParser.OnFailure += (e, a) => Console.WriteLine("Error");
            Elements.Add(eParser);
        }

        private void Connect()
        {
            //TODO: transform this in a service (generic) that connects to the page
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
        }

        private HtmlNode GetTable()
        {
            return doc.DocumentNode.SelectSingleNode("//table[@class='items']");
        }
    }
}
