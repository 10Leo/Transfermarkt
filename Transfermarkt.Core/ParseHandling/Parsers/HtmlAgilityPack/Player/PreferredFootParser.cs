﻿using HtmlAgilityPack;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Player;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Player
{
    class PreferredFootParser : ElementParser<PreferredFoot, Actors.Foot?, HtmlNode>
    {
        public PreferredFootParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == "Pé";

            this.ParseFunc = node =>
            {
                var parsedStr = node
                    .InnerText;

                return new PreferredFoot { Value = new FootValue { Value = Converter.Convert(parsedStr) } };
            };
        }
    }
}
