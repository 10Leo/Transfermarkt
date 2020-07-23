using HtmlAgilityPack;
using Page.Scraper.Contracts;
using System.Collections.Generic;
using System.Linq;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.ParseHandling.Converters;
using Transfermarkt.Logging;

namespace Transfermarkt.Core.ParseHandling.Pages
{
    public class ClubPage : Page<IValue, HtmlNode>
    {
        public ILogger Logger { get; set; } = LoggerFactory.GetLogger(LogLevel.Milestone);

        public ClubPage() : base(new HAPConnection())
        {
            Init();
        }

        private void Init()
        {
            this.Domain = new Club();

            this.Sections = new List<ISection>
            {
                new ClubPageSection(this, Logger),
                new ClubPlayersPageSection(this, Logger)
            };

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

    class ClubPageSection : ElementsSection<HtmlNode>
    {
        public HAPConnection Conn => (HAPConnection)this.Page.Connection;

        public ClubPageSection(IPage<IDomain, HtmlNode> page, ILogger logger) : base("Club Details", page)
        {
            this.Parsers = new List<IElementParser<IElement<IValue>, IValue, HtmlNode>>() {
                new Parsers.HtmlAgilityPack.Club.CountryParser{ Converter = new NationalityConverter() },
                new Parsers.HtmlAgilityPack.Club.NameParser{ Converter = new StringConverter() },
                new Parsers.HtmlAgilityPack.Club.SeasonParser{ Converter = new IntConverter() },
                new Parsers.HtmlAgilityPack.Club.ImgUrlParser{ Converter = new StringConverter() },
                new Parsers.HtmlAgilityPack.Club.CountryImgParser{ Converter = new StringConverter() }
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

    class ClubPlayersPageSection : ChildsSamePageSection<Player, HtmlNode>
    {
        public HAPConnection Conn => (HAPConnection)this.Page.Connection;

        public ClubPlayersPageSection(IPage<IDomain, HtmlNode> page, ILogger logger) : base("Club - Players Section", page)
        {
            this.Parsers = new List<IElementParser<IElement<IValue>, IValue, HtmlNode>>() {
                new Parsers.HtmlAgilityPack.Player.NameParser{ Converter = new StringConverter() },
                new Parsers.HtmlAgilityPack.Player.ShortNameParser{ Converter = new StringConverter() },
                new Parsers.HtmlAgilityPack.Player.BirthDateParser{ Converter = new DateConverter() },
                new Parsers.HtmlAgilityPack.Player.NationalityParser{ Converter = new NationalityConverter() },
                new Parsers.HtmlAgilityPack.Player.HeightParser{ Converter = new IntConverter() },
                new Parsers.HtmlAgilityPack.Player.PreferredFootParser{ Converter = new FootConverter() },
                new Parsers.HtmlAgilityPack.Player.PositionParser{ Converter = new PositionConverter() },
                new Parsers.HtmlAgilityPack.Player.ShirtNumberParser{ Converter = new IntConverter() },
                new Parsers.HtmlAgilityPack.Player.CaptainParser{ Converter = new IntConverter() },
                new Parsers.HtmlAgilityPack.Player.ClubArrivalDateParser{ Converter = new DateConverter() },
                new Parsers.HtmlAgilityPack.Player.ContractExpirationDateParser{ Converter = new DateConverter() },
                new Parsers.HtmlAgilityPack.Player.MarketValueParser{ Converter = new DecimalConverter() },
                new Parsers.HtmlAgilityPack.Player.ImgUrlParser{ Converter = new StringConverter() },
                new Parsers.HtmlAgilityPack.Player.ProfileUrlParser{ Converter = new StringConverter() }
            };

            this.GetChildsNodes = () =>
            {
                IList<List<(HtmlNode key, HtmlNode value)>> playersNodes = new List<List<(HtmlNode, HtmlNode)>>();

                Conn.GetNodeFunc = () => { return Conn.doc.DocumentNode; };

                HtmlNode table = Conn.GetNode().SelectSingleNode("//table[@class='items']");
                if (table == null)
                {
                    return playersNodes;
                }

                var headers = table.SelectNodes(".//thead/tr[th]");
                var rows = table.SelectNodes(".//tbody/tr[td]");
                HtmlNodeCollection headerCols = headers[0].SelectNodes("th");

                //each row is a player
                foreach (var row in rows)
                {
                    List<(HtmlNode key, HtmlNode value)> attribs = new List<(HtmlNode key, HtmlNode value)>();

                    playersNodes.Add(attribs);

                    //TODO: consider passing the whole tr instead of tds
                    //each column is an attribute
                    HtmlNodeCollection cols = row.SelectNodes("td");

                    for (int i = 0; i < cols.Count; i++)
                    {
                        var header = headerCols[i];
                        var element = cols[i];

                        attribs.Add((header, element));
                    }
                }

                return playersNodes;
            };

            this.Parsers.ToList().ForEach(p => p.OnSuccess += (o, e) => logger.LogMessage(LogLevel.Info, new List<string> { $"EVT: Success parsing.", $"DO: {e.Element.name}" }));
            this.Parsers.ToList().ForEach(p => p.OnFailure += (o, e) => logger.LogException(LogLevel.Warning, new List<string> { $"EVT: Error parsing on node {e.Node?.Name}.", $"DO: {e.Element.name}", $"INNER_TEXT: {e.Node?.InnerText}", $"INNER_HTML: {e.Node?.InnerHtml}" }, e.Exception));
        }
    }
}
