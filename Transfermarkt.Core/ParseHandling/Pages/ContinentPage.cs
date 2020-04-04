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
                logger.LogMessage(LogLevel.Milestone, new List<string> { $"Started parsing {e.Url}" });
            };

            this.OnAfterParse += (o, e) => {
                logger.LogMessage(LogLevel.Milestone, new List<string> { $"Finished parsing {e.Url}" });
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

            this.Parsers.ToList().ForEach(p => p.OnSuccess += (o, e) => logger.LogMessage(LogLevel.Info, new List<string> { $"Success parsing {e.Element.name}." }));
            this.Parsers.ToList().ForEach(p => p.OnFailure += (o, e) => logger.LogException(LogLevel.Warning, new List<string> { $"Error parsing {e.Element.name} on node {e.Node.Name}.", $"INNER_TEXT: {e.Node?.InnerText}" }, e.Exception));
        }
    }

    class ContinentCompetitionsPageSection : ChildsSection<HtmlNode, IValue>
    {
        public string BaseURL { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.BaseURL);

        public ContinentCompetitionsPageSection(HAPConnection connection, ILogger logger)
        {
            this.Page = new CompetitionPage(connection, logger);

            this.GetUrls = () =>
            {
                IList<string> urls = new List<string>();

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
                        string competitionUrl = GetCompetitionUrl(cols[0]);
                        string finalCompetitionUrl = TransformUrl(competitionUrl, BaseURL);

                        urls.Add(finalCompetitionUrl);
                    }
                    catch (Exception ex)
                    {
                        logger.LogException(LogLevel.Error, new List<string> { "Error collecting Competition urls" }, ex);
                    }
                }

                return urls;
            };
        }

        private string GetCompetitionUrl(HtmlNode node)
        {
            return node
                .SelectNodes("table//td[2]/a")
                .FirstOrDefault()
                .Attributes["href"].Value;
        }

        private string TransformUrl(string url, string baseURL)
        {
            return string.Format("{0}{1}", baseURL, url);
        }

    }
}
