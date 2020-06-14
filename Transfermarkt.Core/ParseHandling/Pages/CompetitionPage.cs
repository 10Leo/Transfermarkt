﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Converters;
using Transfermarkt.Logging;

namespace Transfermarkt.Core.ParseHandling.Pages
{
    public class CompetitionPage : Page<IValue, HtmlNode>
    {
        public CompetitionPage(HAPConnection connection, ILogger logger, string year) : base(connection)
        {
            this.Domain = new Competition();

            this.Sections = new List<ISection<IElement<IValue>, IValue, HtmlNode>>
            {
                new CompetitionPageSection(connection, logger),
                new CompetitionClubsPageSection(connection, logger, year)
            };

            this.OnBeforeParse += (o, e) => {
                logger.LogMessage(LogLevel.Milestone, new List<string> { $"EVT: Started parsing.", $"URL: {e.Url}" });
            };

            this.OnAfterParse += (o, e) => {
                logger.LogMessage(LogLevel.Milestone, new List<string> { $"EVT: Finished parsing.", $"URL: {e.Url}" });
            };
        }
    }

    class CompetitionPageSection : ElementsSection<HtmlNode, IValue>
    {
        public CompetitionPageSection(HAPConnection connection, ILogger logger)
        {
            this.Parsers = new List<IElementParser<IElement<IValue>, IValue, HtmlNode>>() {
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

                    elements.Add((null, connection.GetNode()));

                return elements;
            };

            this.Parsers.ToList().ForEach(p => p.OnSuccess += (o, e) => logger.LogMessage(LogLevel.Info, new List<string> { $"EVT: Parsing element Success.", $"DO: {e.Element.name}" }));
            this.Parsers.ToList().ForEach(p => p.OnFailure += (o, e) => logger.LogException(LogLevel.Warning, new List<string> { $"EVT: Parsing Error on node {e.Node?.Name}.", $"DO: {e.Element.name}", $"INNER_TEXT: {e.Node?.InnerText}" }, e.Exception));
        }
    }

    class CompetitionClubsPageSection : ChildsSection<HtmlNode, IValue>
    {
        public string BaseURL { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.BaseURL);
        public string SimpleClubUrlFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.SimpleClubUrlFormat);
        public string PlusClubUrlFormat { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.PlusClubUrlFormatV2);
        public string IdentifiersGetterPattern { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.IdentifiersGetterPattern);
        public string IdentifiersSetterPattern { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.IdentifiersSetterPattern);
        public string Season { get; }

        public CompetitionClubsPageSection(HAPConnection connection, ILogger logger, string year)
        {
            this.Season = year;
            this.Name = "Clubs";
            this.Page = new ClubPage(connection, logger, year);

            this.GetUrls = () =>
            {
                IList<Link> urls = new List<Link>();

                connection.GetNodeFunc = () => { return connection.doc.DocumentNode; };
                
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
                        Link clubUrl = GetClubLink(cols[2]);
                        clubUrl.Url = TransformUrl(clubUrl.Url, BaseURL, SimpleClubUrlFormat, PlusClubUrlFormat, IdentifiersGetterPattern, IdentifiersSetterPattern);

                        urls.Add(clubUrl);
                    }
                    catch (Exception ex)
                    {
                        logger.LogException(LogLevel.Error, new List<string> { "EVT: Error collecting Club urls" }, ex);
                    }
                }

                return urls;
            };
        }

        private Link GetClubLink(HtmlNode node)
        {
            var a = node
                .SelectNodes("a")
                .FirstOrDefault(n => n.Attributes["class"]?.Value == "vereinprofil_tooltip");

            return new Link{
                Title = a.InnerText,
                Url = a.Attributes["href"].Value };
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
                throw new Exception($"Error transforming url: '{url}.'");
            }

            IDictionary<string, string> dic = new Dictionary<string, string>();
            for (int i = 1; i < matches[0].Groups.Count; i++)
            {
                Group group = matches[0].Groups[i];
                dic.Add(group.Name, group.Value);
            }

            if (dic.ContainsKey("SEASON"))
            {
                dic["SEASON"] = Season;
            }

            for (int i = 0; i < dic.Count; i++)
            {
                var k = dic.Keys.ElementAt(i);
                finalClubUrl = finalClubUrl.Replace("{" + k + "}", dic[k]);
            }

            return string.Format("{0}{1}", baseURL, finalClubUrl);
        }
    }
}
