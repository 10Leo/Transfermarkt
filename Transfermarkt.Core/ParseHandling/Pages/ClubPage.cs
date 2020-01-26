using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Converters;

namespace Transfermarkt.Core.ParseHandling.Pages
{
    public class ClubPage : IPage<IDomain, HtmlNode, IElement>
    {
        private readonly string url;
        private HtmlDocument doc;

        public IDomain Domain { get; set; }

        public IReadOnlyList<ISection<HtmlNode, IElement>> Sections { get; set; }

        public ClubPage(string url)
        {
            this.url = url;

            IConverter<object> c = new IntConverter();

            this.Sections = new List<ISection<HtmlNode, IElement>>
            {
                new ClubPageSection(),
                new ClubPlayersPageSection()
            };

            Connect();
        }

        #region Contract

        public void Parse()
        {
            this.Domain = new Club();

            foreach (var elementParser in Sections[0].Elements)
            {
                var parsedObj = elementParser.Parse(doc.DocumentNode);
                var e = this.Domain.SetElement(parsedObj);
            }


            HtmlNode table = GetTable();
            if (table == null)
            {
                return;
            }

            var headers = table.SelectNodes(".//thead/tr[th]");
            var rows = table.SelectNodes(".//tbody/tr[td]");
            HtmlNodeCollection headerCols = headers[0].SelectNodes("th");

            //each row is a player
            foreach (var row in rows)
            {
                Player player = new Player();
                this.Domain.Children.Add(player);

                //each column is an attribute
                HtmlNodeCollection cols = row.SelectNodes("td");

                for (int i = 0; i < cols.Count; i++)
                {
                    var header = headerCols[i];
                    var element = cols[i];

                    foreach (var elementParser in Sections[1].Elements)
                    {
                        if (elementParser.CanParse(header))
                        {
                            var parsedObj = elementParser.Parse(element);
                            var e = player.SetElement(parsedObj);
                        }
                    }
                }
            }
        }

        public void Save()
        {
        }

        #endregion

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

        private void LogSuccess(Object o, CustomEventArgs e)
        {
            Console.WriteLine(".");
        }

        private void LogFailure(Object o, CustomEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }

    class ClubPageSection : ISection<HtmlNode, IElement>
    {
        public IReadOnlyList<IElementParser<HtmlNode, IElement, dynamic>> Elements { get; set; }

        public ClubPageSection()
        {
            this.Elements = new List<IElementParser<HtmlNode, IElement, object>>() {
                new Parsers.HtmlAgilityPack.Club.NameParser{ Converter = new StringConverter() }
            };
        }
    }

    class ClubPlayersPageSection : ISection<HtmlNode, IElement>
    {
        public IReadOnlyList<IElementParser<HtmlNode, IElement, dynamic>> Elements { get; set; }

        public ClubPlayersPageSection()
        {
            this.Elements = new List<IElementParser<HtmlNode, IElement, object>>() {
                new Parsers.HtmlAgilityPack.Player.NameParser{ Converter = new StringConverter() },
                new Parsers.HtmlAgilityPack.Player.HeightParser{ Converter = new IntConverter() },
                new Parsers.HtmlAgilityPack.Player.MarketValueParser{ Converter = new DecimalConverter() }
            };
        }
    }
}
