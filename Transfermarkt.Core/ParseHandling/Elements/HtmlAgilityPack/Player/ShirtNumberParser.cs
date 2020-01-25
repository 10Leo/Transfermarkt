using HtmlAgilityPack;
using System;
using System.Linq;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Elements.HtmlAgilityPack.Player
{
    class ShirtNumberParser : ElementParser<HtmlNode, int?>
    {
        public override string DisplayName { get; set; } = "Shirt Number";

        public ShirtNumberParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == "#";

            this.ParseFunc = node =>
            {
                var parsedStr = node
                    .SelectNodes("div")
                    .Where(n => n.Attributes["class"].Value == "rn_nummer")
                    .FirstOrDefault()
                    .InnerText;
                return Converter.Convert(parsedStr);
            };
        }
    }
}
