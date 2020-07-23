using HtmlAgilityPack;
using Page.Scraper.Contracts;
using System.Linq;
using Transfermarkt.Core.ParseHandling.Elements.Player;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Player
{
    class PositionParser : ElementParser<Position, PositionValue, HtmlNode>
    {
        public PositionParser()
        {
            this.CanParsePredicate = node => node?.InnerText?.Trim(Common.trimChars) == ParsersConfig.GetLabel(this.GetType(), ConfigType.PLAYER);

            this.ParseFunc = node =>
            {
                var parsedStr = node
                    .SelectNodes("table//tr[2]/td[1]")
                    .FirstOrDefault()
                    .InnerText;

                return new Position { Value = Converter.Convert(parsedStr) };
            };
        }
    }
}
