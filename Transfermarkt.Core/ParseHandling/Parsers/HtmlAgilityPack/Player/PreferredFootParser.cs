﻿using HtmlAgilityPack;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Player;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Player
{
    class PreferredFootParser : ElementParser<PreferredFoot, FootValue, HtmlNode>
    {
        public PreferredFootParser()
        {
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == ParsersConfig.Get(this.GetType(), ConfigType.PLAYER);

            this.ParseFunc = node =>
            {
                var parsedStr = node
                    .InnerText;

                return new PreferredFoot { Value = Converter.Convert(parsedStr) };
            };
        }
    }
}
