using HtmlAgilityPack;
using System.Linq;
using Transfermarkt.Core.ParseHandling.Elements.Player;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Player
{
    class ShirtNumberParser : ElementParser<HtmlNode>
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
                return new ShirtNumber { Value = Converter.Convert(parsedStr) };
            };
        }
    }
}
