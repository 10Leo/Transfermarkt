﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Net;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Contracts.Element;
using Transfermarkt.Core.ParseHandling.Contracts.Page;
using Transfermarkt.Core.ParseHandling.Converters;
using Transfermarkt.Core.ParseHandling.Elements.HtmlAgilityPack.Club;
using Transfermarkt.Core.ParseHandling.Elements.HtmlAgilityPack.Player;
using Transfermarkt.Core.Parsers.HtmlAgilityPack.Player;

namespace Transfermarkt.Core.ParseHandling.Pages
{
    public class ClubPage : IPage<ID, HtmlNode, IElement>
    {
        private readonly string url;
        private HtmlDocument doc;

        public ID Domain { get; set; }

        public IList<IElementParser<HtmlNode, IElement, dynamic>> Elements { get; set; }

        public ClubPage(string url)
        {
            this.url = url;

            IConverter<object> c = new IntConverter();

            this.Elements = new List<IElementParser<HtmlNode, IElement, object>>() {
                new HeightParser{ Converter = new IntConverter() },
                new MarketValueParser { Converter = new DecimalConverter() }
            };

            Connect();
        }

        #region Contract

        public void Parse()
        {
            this.Domain = new DClub();
            //club.Season = Season.Parse(doc.DocumentNode);


            HtmlNode table = GetTable();
            if (table == null)
            {
                return;
            }

            var headers = table.SelectNodes(".//thead/tr[th]");
            var rows = table.SelectNodes(".//tbody/tr[td]");
            HtmlNodeCollection headerCols = headers[0].SelectNodes("th");

            //each row is a player
            foreach (var row in rows)
            {
                DPlayer player = new DPlayer();
                this.Domain.Children.Add(player);

                //each column is an attribute
                HtmlNodeCollection cols = row.SelectNodes("td");

                for (int i = 0; i < cols.Count; i++)
                {
                    var header = headerCols[i];
                    var element = cols[i];

                    foreach (var elementParser in Elements)
                    {
                        if (elementParser.CanParse(header))
                        {
                            var parsedObj = elementParser.Parse(element);
                            var e = player.SetElement(parsedObj);
                        }
                    }
                }
            }
        }

        public void Save()
        {
        }

        #endregion

        private void Connect()
        {
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
        }

        private HtmlNode GetTable()
        {
            return doc.DocumentNode.SelectSingleNode("//table[@class='items']");
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
}
