﻿using HtmlAgilityPack;
using System;
using System.Linq;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Elements.HtmlAgilityPack.Player
{
    class PositionParser : ElementParser<HtmlNode, Position?>
    {
        public override string DisplayName { get; set; } = "Position";

        public PositionParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == "Jogadores";

            this.ParseFunc = node =>
            {
                var parsedStr = node
                    .SelectNodes("table//tr[2]/td[1]")
                    .FirstOrDefault()
                    .InnerText;
                return Converter.Convert(parsedStr);
            };
        }
    }
}
