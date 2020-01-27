﻿using HtmlAgilityPack;
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
        public IDomain Domain { get; set; }

        public IReadOnlyList<ISection<IDomain, HtmlNode, IElement>> Sections { get; set; }

        public ClubPage()
        {
            this.Sections = new List<ISection<IDomain, HtmlNode, IElement>>
            {
                new ClubPageSection(),
                new ClubPlayersPageSection()
            };
        }

        #region Contract

        public IDomain Parse(string url)
        {
            var doc = Connect(url);

            this.Domain = new Club();

            foreach (var elementParser in Sections[0].Parsers)
            {
                var parsedObj = elementParser.Parse(doc.DocumentNode);
                var e = this.Domain.SetElement(parsedObj);
            }


            HtmlNode table = doc.DocumentNode.SelectSingleNode("//table[@class='items']");
            if (table == null)
            {
                return null;
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

                    foreach (var elementParser in Sections[1].Parsers)
                    {
                        if (elementParser.CanParse(header))
                        {
                            var parsedObj = elementParser.Parse(element);
                            var e = player.SetElement(parsedObj);
                        }
                    }
                }
            }

            return this.Domain;
        }

        public void Save()
        {
        }

        #endregion

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

        private void LogSuccess(Object o, CustomEventArgs e)
        {
            Console.WriteLine(".");
        }

        private void LogFailure(Object o, CustomEventArgs e)
        {
            Console.WriteLine(e.Message);
        }
    }

    class ClubPageSection : ISection<IDomain, HtmlNode, IElement>
    {
        public IReadOnlyList<IElementParser<HtmlNode, IElement, dynamic>> Parsers { get; set; }
        public IReadOnlyList<IPage<IDomain, HtmlNode, IElement>> Pages { get; set; }

        public ClubPageSection()
        {
            this.Parsers = new List<IElementParser<HtmlNode, IElement, object>>() {
                new Parsers.HtmlAgilityPack.Club.CountryParser{ Converter = new NationalityConverter() },
                new Parsers.HtmlAgilityPack.Club.NameParser{ Converter = new StringConverter() },
                new Parsers.HtmlAgilityPack.Club.SeasonParser{ Converter = new IntConverter() },
                new Parsers.HtmlAgilityPack.Club.ImgUrlParser{ Converter = new StringConverter() },
                new Parsers.HtmlAgilityPack.Club.CountryImgParser{ Converter = new StringConverter() }
            };

            this.Pages = new List<IPage<IDomain, HtmlNode, IElement>>();
        }
    }

    class ClubPlayersPageSection : ISection<IDomain, HtmlNode, IElement>
    {
        public IReadOnlyList<IElementParser<HtmlNode, IElement, dynamic>> Parsers { get; set; }
        public IReadOnlyList<IPage<IDomain, HtmlNode, IElement>> Pages { get; set; }

        public ClubPlayersPageSection()
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

            this.Pages = new List<IPage<IDomain, HtmlNode, IElement>>();
        }
    }
}
