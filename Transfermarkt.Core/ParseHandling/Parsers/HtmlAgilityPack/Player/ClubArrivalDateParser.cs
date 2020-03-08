using HtmlAgilityPack;
using System;
using Transfermarkt.Core.ParseHandling.Contracts;
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

                return new ClubArrivalDate { Value = Converter.Convert(parsedStr) };
            };
        }
    }
}
