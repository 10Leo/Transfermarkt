using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.Contracts;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Contracts.Page;
using Transfermarkt.Core.ParseHandling.Converters;
using Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Competition;

namespace Transfermarkt.Core.ParseHandling.Pages
{
    public class CompetitionPage : IPage<IDomain, HtmlNode, IElement>
    {
        private static IConfigurationManager config = new ConfigManager();

        public string BaseURL { get; } = config.GetAppSetting("BaseURL");
        public string SimpleClubUrlFormat { get; } = config.GetAppSetting("SimpleClubUrlFormat");
        public string PlusClubUrlFormat { get; } = config.GetAppSetting("PlusClubUrlFormatV2");
        public string IdentifiersGetterPattern { get; } = config.GetAppSetting("IdentifiersGetterPattern");
        public string IdentifiersSetterPattern { get; } = config.GetAppSetting("IdentifiersSetterPattern");

        public IDomain Domain { get; set; }

        public IReadOnlyList<ISection<IDomain, HtmlNode, IElement>> Sections { get; set; }

        public CompetitionPage()
        {
            this.Sections = new List<ISection<IDomain, HtmlNode, IElement>>
            {
                new CompetitionPageSection(),
                //new CompetitionClubsPageSection()
            };
        }

        #region Contract

        public IDomain Parse(string url)
        {
            var doc = Connect(url);

            this.Domain = new Competition();

            if (Sections[0].Parsers != null)
            {
                foreach (var elementParser in Sections[0].Parsers)
                {
                    var parsedObj = elementParser.Parse(doc.DocumentNode);
                    var e = this.Domain.SetElement(parsedObj);
                }
            }


            if (Sections[0].Pages != null)
            {
                foreach (var childPage in Sections[0].Pages)
                {

                    HtmlNode table = doc.DocumentNode.SelectSingleNode("//div[@id='yw1']/table[@class='items']");
                    if (table == null)
                    {
                        return null;
                    }

                    var rows = table.SelectNodes(".//tbody/tr[td]");
                    // each row is a club
                    foreach (var row in rows)
                    {
                        //each column is an attribute
                        HtmlNodeCollection cols = row.SelectNodes("td");

                        try
                        {
                            string clubUrl = GetClubUrl(cols[2]);
                            string finalClubUrl = TransformUrl(clubUrl);
                            
                            this.Domain?.Children.Add(childPage.Parse($"{BaseURL}{finalClubUrl}"));
                        }
                        catch (Exception ex)
                        {
                            //TODO
                        }
                    }
                }
            }

            return this.Domain;
        }

        public void Save()
        {
        }

        #endregion Contract

        private HtmlDocument Connect(string url)
        {
            HtmlDocument doc = null;
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
            return doc;
        }

        private string GetClubUrl(HtmlNode node)
        {
            return node
                .SelectNodes("a")
                .FirstOrDefault(n => n.Attributes["class"]?.Value == "vereinprofil_tooltip")
                .Attributes["href"].Value;
        }

        private string TransformUrl(string url)
        {
            IList<string> identifiers = new List<string>();

            string simpleClubUrlPattern = SimpleClubUrlFormat;
            string finalClubUrl = PlusClubUrlFormat;

            MatchCollection ids = Regex.Matches(SimpleClubUrlFormat, IdentifiersGetterPattern);
            foreach (Match idMatch in ids)
            {
                identifiers.Add(idMatch.Groups[1].Value);
            }

            foreach (string identifier in identifiers)
            {
                simpleClubUrlPattern = simpleClubUrlPattern.Replace("{" + identifier + "}", IdentifiersSetterPattern.Replace("{ID}", identifier));
            }

            MatchCollection matches = Regex.Matches(url, simpleClubUrlPattern);
            if (!(matches.Count > 0 && matches[0].Groups.Count >= identifiers.Count))
            {
                //TODO: logging
            }

            for (int i = 1; i < matches[0].Groups.Count; i++)
            {
                Group group = matches[0].Groups[i];
                finalClubUrl = finalClubUrl.Replace("{" + group.Name + "}", group.Value);
            }

            return finalClubUrl;
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

    class CompetitionPageSection : ISection<IDomain, HtmlNode, IElement>
    {
        public IReadOnlyList<IElementParser<HtmlNode, IElement, object>> Parsers { get; set; }
        public IReadOnlyList<IPage<IDomain, HtmlNode, IElement>> Pages { get; set; }

        public CompetitionPageSection()
        {
            this.Parsers = new List<IElementParser<HtmlNode, IElement, object>>() {
                new Parsers.HtmlAgilityPack.Competition.CountryParser{ Converter = new NationalityConverter() },
                new Parsers.HtmlAgilityPack.Competition.NameParser{ Converter = new StringConverter() },
                new Parsers.HtmlAgilityPack.Competition.SeasonParser{ Converter = new IntConverter() },
                new Parsers.HtmlAgilityPack.Competition.ImgUrlParser{ Converter = new StringConverter() },
                new Parsers.HtmlAgilityPack.Competition.CountryImgParser{ Converter = new StringConverter() },
            };

            this.Pages = new List<IPage<IDomain, HtmlNode, IElement>>
            {
                new ClubPage()
            };
        }
    }

    //class CompetitionClubsPageSection : ISection<HtmlNode, IElement>
    //{
    //    public IReadOnlyList<IElementParser<HtmlNode, IElement, object>> Elements { get; set; }
    //}
}
