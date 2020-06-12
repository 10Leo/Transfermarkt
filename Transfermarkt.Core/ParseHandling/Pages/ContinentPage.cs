using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Converters;
using Transfermarkt.Logging;

namespace Transfermarkt.Core.ParseHandling.Pages
{
    public class ContinentPage : Page<IValue, HtmlNode>
    {
        public ContinentPage(HAPConnection connection, ILogger logger) : base(connection)
        {
            this.Domain = new Continent();

            this.Sections = new List<ISection<IElement<IValue>, IValue, HtmlNode>>
            {
                new ContinentPageSection(connection, logger),
                new ContinentCompetitionsPageSection(connection, logger)
            };

            this.OnBeforeParse += (o, e) => {
                logger.LogMessage(LogLevel.Milestone, new List<string> { $"EVT: Started parsing.", $"URL: {e.Url}" });
            };

            this.OnAfterParse += (o, e) => {
                logger.LogMessage(LogLevel.Milestone, new List<string> { $"EVT: Finished parsing.", $"URL: {e.Url}" });
            };
        }
    }

    class ContinentPageSection : ElementsSection<HtmlNode, IValue>
    {
        public ContinentPageSection(HAPConnection connection, ILogger logger)
        {
            this.Parsers = new List<IElementParser<IElement<IValue>, IValue, HtmlNode>>() {
                new Parsers.HtmlAgilityPack.Continent.NameParser{ Converter = new StringConverter() },
                new Parsers.HtmlAgilityPack.Continent.ContinentCodeParser{ Converter = new ContinentCodeConverter() },
            };

            this.GetElementsNodes = () =>
            {
                IList<(HtmlNode key, HtmlNode value)> elements = new List<(HtmlNode, HtmlNode)>();
                connection.GetNodeFunc = () => { return connection.doc.DocumentNode; };

                elements.Add((null, connection.GetNode()));

                return elements;
            };

            this.Parsers.ToList().ForEach(p => p.OnSuccess += (o, e) => logger.LogMessage(LogLevel.Info, new List<string> { $"EVT: Parsing element Success.", $"DO: {e.Element.name}" }));
            this.Parsers.ToList().ForEach(p => p.OnFailure += (o, e) => logger.LogException(LogLevel.Warning, new List<string> { $"EVT: Parsing Error on node {e.Node?.Name}.", $"DO: {e.Element.name}", $"INNER_TEXT: {e.Node?.InnerText}" }, e.Exception));
        }
    }

    class ContinentCompetitionsPageSection : ChildsSection<HtmlNode, IValue>
    {
        public string BaseURL { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.BaseURL);

        public ContinentCompetitionsPageSection(HAPConnection connection, ILogger logger)
        {
            this.Name = "Competitions";
            this.Page = new CompetitionPage(connection, logger);

            this.GetUrls = () =>
            {
                IList<Link> urls = new List<Link>();

                connection.GetNodeFunc = () => { return connection.doc.DocumentNode; };

                HtmlNode table = connection.GetNode().SelectSingleNode("//div[@id='yw1']/table[@class='items']");
                if (table == null)
                {
                    return null;
                }

                var rows = table.SelectNodes(".//tbody/tr[@class='odd']|.//tbody/tr[@class='even']");
                // each row is a Competiton
                foreach (var row in rows)
                {
                    HtmlNodeCollection cols = row.SelectNodes("td");

                    try
                    {
                        var competitionUrl = GetCompetitionLink(cols);
                        competitionUrl.Url = TransformUrl(competitionUrl.Url, BaseURL);

                        urls.Add(competitionUrl);
                    }
                    catch (Exception ex)
                    {
                        logger.LogException(LogLevel.Error, new List<string> { "EVT: Error collecting Competition urls" }, ex);
                    }
                }

                return urls;
            };
        }

        private Link GetCompetitionLink(HtmlNodeCollection cols)
        {
            var country = cols[1]
                .SelectNodes("img")
                .FirstOrDefault(n => n.Attributes["class"]?.Value == "flaggenrahmen")?.Attributes["Title"].Value;
            var a = cols[0]
                .SelectNodes("table//td[2]/a")
                .FirstOrDefault();
            return new Link { Title = $"{country}-{a.InnerText}", Url = a.Attributes["href"].Value };
        }

        private string TransformUrl(string url, string baseURL)
        {
            return string.Format("{0}{1}", baseURL, url);
        }

    }
}
