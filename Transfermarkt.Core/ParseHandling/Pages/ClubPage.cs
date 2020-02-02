﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Converters;

namespace Transfermarkt.Core.ParseHandling.Pages
{
    public class ClubPage : Page<HtmlNode>
    {
        public ClubPage(HAPConnection connection) : base(connection)
        {
            this.Domain = new Club();

            this.Sections = new List<ISection<IDomain, HtmlNode, IElement>>
            {
                new ClubPageSection(connection),
                new ClubPlayersPageSection(connection)
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

    class ClubPageSection : IElementsSection<IDomain, HtmlNode, IElement>
    {
        public IReadOnlyList<IElementParser<HtmlNode, IElement, object>> Parsers { get; set; }

        public Func<IList<(HtmlNode key, HtmlNode value)>> GetElementsNodes { get; set; }

        public ClubPageSection(HAPConnection connection)
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

    class ClubPlayersPageSection : IChildsSamePageSection<IDomain, HtmlNode, IElement>
    {
        public IPage<IDomain, HtmlNode, IElement> Page { get; set; }

        public Func<IList<(IDomain child, List<(HtmlNode key, HtmlNode value)>)>> GetChildsNodes { get; set; }
        public IReadOnlyList<IElementParser<HtmlNode, IElement, object>> Parsers { get; set; }

        public ClubPlayersPageSection(HAPConnection connection)
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
                new Parsers.HtmlAgilityPack.Player.ImgUrlParser{ Converter = new StringConverter() },
                new Parsers.HtmlAgilityPack.Player.ProfileUrlParser{ Converter = new StringConverter() }
            };

            this.GetChildsNodes = () =>
            {
                IList<(IDomain child, List<(HtmlNode key, HtmlNode value)>)> playersNodes = new List<(IDomain, List<(HtmlNode, HtmlNode)>)>();
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

                    playersNodes.Add((new Player(), attribs));

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
        }

        public void Parse(IPage<IDomain, HtmlNode, IElement> page)
        {
            {
                IList<(IDomain child, List<(HtmlNode key, HtmlNode value)>)> childDomainNodes = GetChildsNodes?.Invoke();

                if (childDomainNodes != null && childDomainNodes.Count > 0)
                {
                    foreach ((IDomain, List<(HtmlNode key, HtmlNode value)>) childDomainNode in childDomainNodes)
                    {
                        IDomain childType = childDomainNode.Item1;
                        page.Domain?.Children.Add(childType);

                        foreach ((HtmlNode key, HtmlNode value) in childDomainNode.Item2)
                        {
                            foreach (var parser in Parsers)
                            {
                                if (parser.CanParse(key))
                                {
                                    var parsedObj = parser.Parse(value);
                                    var e = childType.SetElement(parsedObj);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
