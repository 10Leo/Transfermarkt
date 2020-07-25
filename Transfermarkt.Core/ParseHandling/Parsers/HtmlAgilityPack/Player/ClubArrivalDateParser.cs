using HtmlAgilityPack;
using Page.Scraper.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Player;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Player
{
    class ClubArrivalDateParser : ElementParser<ClubArrivalDate, DatetimeValue, HtmlNode>
    {
        public ClubArrivalDateParser()
        {
            this.CanParsePredicate = node => node?.InnerText?.Trim(Common.trimChars) == ParsersConfig.GetLabel(this.GetType(), ConfigType.PLAYER);

            this.ParseFunc = node =>
            {
                var parsedStr = node.InnerText;

                return new ClubArrivalDate(parsedStr);
            };
        }
    }
}
