﻿using HtmlAgilityPack;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Converters;
using Transfermarkt.Logging;

namespace Transfermarkt.Core.ParseHandling.Pages
{
    public class ClubPage : Page<HtmlNode>
    {
        public ClubPage(HAPConnection connection, ILogger logger) : base(connection)
        {
            this.Domain = new Club();

            this.Sections = new List<ISection<IDomain, HtmlNode, IElement>>
            {
                new ClubPageSection(connection, logger),
                new ClubPlayersPageSection(connection, logger)
            };

            this.OnBeforeParse += (o, e) => {
                logger.LogMessage(LogLevel.Milestone, $"Started parsing {e.Url}.");
            };

            this.OnAfterParse += (o, e) => {
                logger.LogMessage(LogLevel.Milestone, $"Finished parsing {e.Url}.");
            };
        }
    }

    class ClubPageSection : ElementsSection<HtmlNode>
    {
        public ClubPageSection(HAPConnection connection, ILogger logger)
        {
            this.Parsers = new List<IElementParser<HtmlNode, IElement, object>>() {
                new Parsers.HtmlAgilityPack.Club.CountryParser{ Converter = new NationalityConverter() },
                new Parsers.HtmlAgilityPack.Club.NameParser{ Converter = new StringConverter() },
                new Parsers.HtmlAgilityPack.Club.SeasonParser{ Converter = new IntConverter() },
                new Parsers.HtmlAgilityPack.Club.ImgUrlParser{ Converter = new StringConverter() },
                new Parsers.HtmlAgilityPack.Club.CountryImgParser{ Converter = new StringConverter() }
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

            this.Parsers.ToList().ForEach(p => p.OnSuccess += (o, e) => logger.LogMessage(LogLevel.Info, $"[Success parsing {e.Element.InternalName}]"));
            this.Parsers.ToList().ForEach(p => p.OnFailure += (o, e) => logger.LogException(LogLevel.Warning, $"[Error parsing {e.Element.InternalName} on node {e.Node.Name}], innertext: [{e.Node?.InnerText}], innerhtml: [{e.Node?.InnerHtml}]", e.Exception));
        }
    }

    class ClubPlayersPageSection : ChildsSamePageSection<Player, HtmlNode>
    {
        public ClubPlayersPageSection(HAPConnection connection, ILogger logger)
        {
            this.Parsers = new List<IElementParser<HtmlNode, IElement, object>>() {
                new Parsers.HtmlAgilityPack.Player.NameParser{ Converter = new StringConverter() },
                new Parsers.HtmlAgilityPack.Player.ShortNameParser{ Converter = new StringConverter() },
                new Parsers.HtmlAgilityPack.Player.BirthDateParser{ Converter = new DateConverter() },
                new Parsers.HtmlAgilityPack.Player.NationalityParser{ Converter = new NationalityConverter() },
                new Parsers.HtmlAgilityPack.Player.HeightParser{ Converter = new IntConverter() },

                new Parsers.HtmlAgilityPack.Player.PreferredFootParser{ Converter = new FootConverter() },
                new Parsers.HtmlAgilityPack.Player.PositionParser{ Converter = new PositionConverter() },
                new Parsers.HtmlAgilityPack.Player.ShirtNumberParser{ Converter = new IntConverter() },
                new Parsers.HtmlAgilityPack.Player.CaptainParser{ Converter = new StringConverter() },
                new Parsers.HtmlAgilityPack.Player.ClubArrivalDateParser{ Converter = new DateConverter() },
                new Parsers.HtmlAgilityPack.Player.ContractExpirationDateParser{ Converter = new DateConverter() },
                new Parsers.HtmlAgilityPack.Player.MarketValueParser{ Converter = new DecimalConverter() },
                //new Parsers.HtmlAgilityPack.Player.ImgUrlParser{ Converter = new StringConverter() },
                new Parsers.HtmlAgilityPack.Player.ProfileUrlParser{ Converter = new StringConverter() }
            };

            this.GetChildsNodes = () =>
            {
                IList<List<(HtmlNode key, HtmlNode value)>> playersNodes = new List<List<(HtmlNode, HtmlNode)>>();
                connection.GetNodeFunc = () => { return connection.doc.DocumentNode; };

                HtmlNode table = connection.GetNode().SelectSingleNode("//table[@class='items']");
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

            this.Parsers.ToList().ForEach(p => p.OnSuccess += (o, e) => logger.LogMessage(LogLevel.Info, $"[Success parsing {e.Element.InternalName}]"));
            this.Parsers.ToList().ForEach(p => p.OnFailure += (o, e) => logger.LogException(LogLevel.Warning, $"[Error parsing {e.Element.InternalName} on node {e.Node.Name}], innertext: [{e.Node?.InnerText}], innerhtml: [{e.Node?.InnerHtml}]", e.Exception));
        }
    }
}