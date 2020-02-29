﻿using HtmlAgilityPack;
using System;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Player;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Player
{
    class ClubArrivalDateParser : ElementParser<ClubArrivalDate, DatetimeValue, HtmlNode>
    {
        public ClubArrivalDateParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == "Na equipa desde";

            this.ParseFunc = node =>
            {
                var parsedStr = node.InnerText;

                return new ClubArrivalDate { Value = Converter.Convert(parsedStr) };
            };
        }
    }
}
