﻿using HtmlAgilityPack;
using LJMB.Common;
using LJMB.Logging;
using Page.Scraper.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using Transfermarkt.Core.Actors;

namespace Transfermarkt.Core.ParseHandling.Pages
{
    public class ContinentPage : Page<IValue, HtmlNode>
    {
        //TODO: logger should come from the top level layer and not instantiated in here
        public ILogger Logger { get; set; } = LoggerFactory.GetLogger(LogLevel.Milestone);
        public int? Year { get; set; }

        public ContinentPage() : base(new HAPConnection()) {
            //TODO: if this constructor is called, the Year prop will not be used, as at the moment of this call is yet to be set
            Init();
        }

        //TODO: section names should be hold as variables in the page and passed to its child sections
        public ContinentPage(HAPConnection connection, ILogger logger, int? year) : base(connection)
        {
            this.Logger = logger;
            this.Year = year;

            Init();
        }

        private void Init() {
            this.Domain = new Continent();

            this.Sections = new List<ISection>
            {
                new ContinentPageSection(this, Logger),
                new ContinentCompetitionsPageSection(this, Logger, Year)
            };

            // TODO create global string with placeholders for event texts: $"{EVT}: {TEXT}"
            this.OnBeforeParse += (o, e) =>
            {
                Logger.LogMessage(LogLevel.Milestone, new List<string> { $"EVT: Started parsing.", $"URL: {e.Url}" });
            };

            this.OnAfterParse += (o, e) =>
            {
                Logger.LogMessage(LogLevel.Milestone, new List<string> { $"EVT: Finished parsing.", $"URL: {e.Url}" });
            };
        }
    }

    public class ContinentPageSection : ElementsSection<HtmlNode>
    {
        public static readonly string SectionName = "Continent Details";
        public HAPConnection Conn => (HAPConnection)this.Page.Connection;

        public ContinentPageSection(IPage<IDomain, HtmlNode> page, ILogger logger) : base(SectionName, page)
        {
            this.Parsers = new List<IElementParser<IElement<IValue, IConverter<IValue>>, IValue, HtmlNode>>() {
                new Parsers.HtmlAgilityPack.Continent.NameParser(),
                new Parsers.HtmlAgilityPack.Continent.ContinentCodeParser()
            };

            this.GetElementsNodes = () =>
            {
                IList<(HtmlNode key, HtmlNode value)> elements = new List<(HtmlNode, HtmlNode)>();
                Conn.GetNodeFunc = () => { return Conn.doc.DocumentNode; };

                elements.Add((null, Conn.GetNode()));

                return elements;
            };

            this.Parsers.ToList().ForEach(p => p.OnSuccess += (o, e) => logger.LogMessage(LogLevel.Info, new List<string> { $"EVT: Parsing element Success.", $"DO: {e.Element.name}" }));
            this.Parsers.ToList().ForEach(p => p.OnFailure += (o, e) => logger.LogException(LogLevel.Warning, new List<string> { $"EVT: Parsing Error on node {e.Node?.Name}.", $"DO: {e.Element.name}", $"INNER_TEXT: {e.Node?.InnerText}" }, e.Exception));
        }
    }

    public class ContinentCompetitionsPageSection : ChildsSection<HtmlNode, CompetitionPage>
    {
        public static readonly string SectionName = "Continent - Competitions Section";
        public string BaseURL { get; } = ConfigManager.GetAppSetting<string>(Keys.Config.BaseURL);
        public int? Season { get; }
        public HAPConnection Conn => (HAPConnection)this.Page.Connection;

        private Type nationalityType;

        public ContinentCompetitionsPageSection(IPage<IDomain, HtmlNode> page, ILogger logger, int? year) : base(SectionName, page, page.Connection)
        {
            this.Season = year;
            this.ChildPage = new CompetitionPage();

            this.GetUrls = () =>
            {
                IList<Link<HtmlNode, CompetitionPage>> urls = new List<Link<HtmlNode, CompetitionPage>>();

                Conn.GetNodeFunc = () => { return Conn.doc.DocumentNode; };

                HtmlNode table = Conn.GetNode().SelectSingleNode("//div[@id='yw1']/table[@class='items']");
                if (table == null)
                {
                    return null;
                }

                var rows = table.SelectNodes(".//tbody/tr[@class='odd']|.//tbody/tr[@class='even']");
                // each row is a Competiton

                nationalityType = typeof(Actors.Nationality);

                foreach (var row in rows)
                {
                    HtmlNodeCollection cols = row.SelectNodes("td");

                    try
                    {
                        Link<HtmlNode, CompetitionPage> competitionUrl = GetCompetitionLink(cols);
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

        private Link<HtmlNode, CompetitionPage> GetCompetitionLink(HtmlNodeCollection cols)
        {
            var country = cols[1]
                .SelectNodes("img")
                .FirstOrDefault(n => n.Attributes["class"]?.Value == "flaggenrahmen")?.Attributes["Title"].Value;
            var aLeagueName = cols[0]
                .SelectNodes("table//td[2]/a")
                .FirstOrDefault();

            var nat = ConvertersConfig.GetNationality(country);

            Link<HtmlNode, CompetitionPage> link = new Link<HtmlNode, CompetitionPage> { Title = $"{country}-{aLeagueName.InnerText}", Url = aLeagueName.Attributes["href"].Value };
            //TODO: these values shouldn't be hardcoded in code
            link.Identifiers.Add(nationalityType.Name, nat?.ToString());
            link.Identifiers.Add("League Name", aLeagueName.InnerText);

            return link;
        }

        private string TransformUrl(string url, string baseURL)
        {
            return string.Format("{0}{1}{2}{3}", baseURL, url, "/plus/?saison_id=", Season);
        }

    }
}
