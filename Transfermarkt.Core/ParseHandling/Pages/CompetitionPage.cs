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
    public class CompetitionPage : Page<HtmlNode>
    {
        public CompetitionPage(HAPConnection connection) : base(connection)
        {
            this.Domain = new Competition();

            this.Sections = new List<ISection<IDomain, HtmlNode, IElement>>
            {
                new CompetitionPageSection(connection),
                new CompetitionClubsPageSection(connection)
            };
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

    class CompetitionPageSection : IElementsSection<IDomain, HtmlNode, IElement>
    {
        public Func<IList<(HtmlNode key, HtmlNode value)>> GetElementsNodes { get; set; }

        public IReadOnlyList<IElementParser<HtmlNode, IElement, object>> Parsers { get; set; }

        public CompetitionPageSection(HAPConnection connection)
        {
            this.Parsers = new List<IElementParser<HtmlNode, IElement, object>>() {
                new Parsers.HtmlAgilityPack.Competition.CountryParser{ Converter = new NationalityConverter() },
                new Parsers.HtmlAgilityPack.Competition.NameParser{ Converter = new StringConverter() },
                new Parsers.HtmlAgilityPack.Competition.SeasonParser{ Converter = new IntConverter() },
                new Parsers.HtmlAgilityPack.Competition.ImgUrlParser{ Converter = new StringConverter() },
                new Parsers.HtmlAgilityPack.Competition.CountryImgParser{ Converter = new StringConverter() },
            };

            this.GetElementsNodes = () =>
            {
                IList<(HtmlNode key, HtmlNode value)> elements = new List<(HtmlNode, HtmlNode)>();
                connection.GetNodeFunc = () => { return connection.doc.DocumentNode; };

                foreach (var elementParser in Parsers)
                {
                    elements.Add((connection.GetNode(), connection.GetNode()));
                }

                return elements;
            };
        }


        public void Parse(IPage<IDomain, HtmlNode, IElement> page)
        {
            if (Parsers != null && Parsers.Count > 0)
            {
                IList<(HtmlNode key, HtmlNode value)> elementsNodes = GetElementsNodes?.Invoke();

                if (elementsNodes != null && elementsNodes.Count > 0)
                {
                    foreach (var (key, value) in elementsNodes)
                    {
                        foreach (var parser in Parsers)
                        {
                            if (parser.CanParse(key))
                            {
                                var parsedObj = parser.Parse(value);
                                var e = page.Domain.SetElement(parsedObj);
                            }
                        }
                    }
                }
            }
        }
    }

    class CompetitionClubsPageSection : IChildsSection<IDomain, HtmlNode, IElement>
    {
        protected static IConfigurationManager config = new ConfigManager();

        public string BaseURL { get; } = config.GetAppSetting("BaseURL");
        public string SimpleClubUrlFormat { get; } = config.GetAppSetting("SimpleClubUrlFormat");
        public string PlusClubUrlFormat { get; } = config.GetAppSetting("PlusClubUrlFormatV2");
        public string IdentifiersGetterPattern { get; } = config.GetAppSetting("IdentifiersGetterPattern");
        public string IdentifiersSetterPattern { get; } = config.GetAppSetting("IdentifiersSetterPattern");
        public IPage<IDomain, HtmlNode, IElement> Page { get; set; }

        public Func<IList<string>> GetUrls { get; set; }
        public Func<IList<(IDomain child, List<(HtmlNode key, HtmlNode value)>)>> GetChildsNodes { get; set; }

        public CompetitionClubsPageSection(HAPConnection connection)
        {
            this.Page = new ClubPage(connection);

            this.GetUrls = () =>
            {
                IList<string> urls = new List<string>();

                HtmlNode table = connection.GetNode().SelectSingleNode("//div[@id='yw1']/table[@class='items']");
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
                        string finalClubUrl = TransformUrl(clubUrl, BaseURL, SimpleClubUrlFormat, PlusClubUrlFormat, IdentifiersGetterPattern, IdentifiersSetterPattern);

                        urls.Add(finalClubUrl);
                    }
                    catch (Exception ex)
                    {
                        //TODO
                    }
                }

                return urls;
            };
        }

        private string GetClubUrl(HtmlNode node)
        {
            return node
                .SelectNodes("a")
                .FirstOrDefault(n => n.Attributes["class"]?.Value == "vereinprofil_tooltip")
                .Attributes["href"].Value;
        }

        private string TransformUrl(string url, string baseURL, string simpleClubUrlFormat, string plusClubUrlFormat, string identifiersGetterPattern, string identifiersSetterPattern)
        {
            IList<string> identifiers = new List<string>();

            string simpleClubUrlPattern = simpleClubUrlFormat;
            string finalClubUrl = plusClubUrlFormat;

            MatchCollection ids = Regex.Matches(simpleClubUrlFormat, identifiersGetterPattern);
            foreach (Match idMatch in ids)
            {
                identifiers.Add(idMatch.Groups[1].Value);
            }

            foreach (string identifier in identifiers)
            {
                simpleClubUrlPattern = simpleClubUrlPattern.Replace("{" + identifier + "}", identifiersSetterPattern.Replace("{ID}", identifier));
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

            return string.Format("{0}{1}", baseURL, finalClubUrl);
        }

        public void Parse(IPage<IDomain, HtmlNode, IElement> page)
        {
            if (Page != null)
            {
                IList<string> pagesNodes = GetUrls?.Invoke();

                if (pagesNodes != null && pagesNodes.Count > 0)
                {
                    foreach (var pageUrl in pagesNodes)
                    {
                        var pageDomain = Page.Parse(pageUrl);
                        page.Domain?.Children.Add(pageDomain);

                        Type t = Page.Domain.GetType();
                        Page.Domain = (IDomain)Activator.CreateInstance(t);
                    }
                }
            }
        }
    }
}
