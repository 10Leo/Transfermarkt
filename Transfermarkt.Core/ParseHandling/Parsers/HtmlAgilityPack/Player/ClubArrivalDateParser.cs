﻿using HtmlAgilityPack;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Player;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Player
{
    class ClubArrivalDateParser : ElementParser<HtmlNode>
    {
        public override IElement Element { get; } = new ClubArrivalDate();

        public ClubArrivalDateParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == "Na equipa desde";

            this.ParseFunc = node =>
            {
                var parsedStr = node.InnerText;

                Element.Value = Converter.Convert(parsedStr);
                return Element;
            };
        }
    }
}
