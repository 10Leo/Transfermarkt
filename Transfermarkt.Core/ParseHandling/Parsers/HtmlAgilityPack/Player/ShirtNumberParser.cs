using HtmlAgilityPack;
using System.Linq;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Player;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Player
{
    class ShirtNumberParser : ElementParser<ShirtNumber, IntValue, HtmlNode>
    {
        public ShirtNumberParser()
        {
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == ParsersConfig.GetLabel(this.GetType(), ConfigType.PLAYER);

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
