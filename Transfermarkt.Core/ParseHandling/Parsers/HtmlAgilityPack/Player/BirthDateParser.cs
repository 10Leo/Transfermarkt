using HtmlAgilityPack;
using System;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Player;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Player
{
    class BirthDateParser : ElementParser<BirthDate, HtmlNode>
    {
        public BirthDateParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == "Nasc. / idade";

            this.ParseFunc = node =>
            {
                var parsedStr = node.InnerText?.Split(new[] { " (" }, StringSplitOptions.None)?[0];

                return new BirthDate { Value = Converter.Convert(parsedStr) };
            };
        }
    }
}
