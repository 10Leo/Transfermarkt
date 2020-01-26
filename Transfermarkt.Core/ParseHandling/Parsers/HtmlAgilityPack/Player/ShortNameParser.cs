﻿using HtmlAgilityPack;
using System.Linq;
using Transfermarkt.Core.ParseHandling.Elements.Player;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Player
{
    class ShortNameParser : ElementParser<HtmlNode>
    {
        public override string DisplayName { get; set; } = "Short Name";

        public ShortNameParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == "Jogadores";

            this.ParseFunc = node =>
            {
                var parsedStr = node
                    .SelectNodes("table//tr[1]/td[2]/div[2]")
                    .FirstOrDefault()
                    .InnerText;
                return new ShortName { Value = Converter.Convert(parsedStr) };
            };
        }
    }
}
