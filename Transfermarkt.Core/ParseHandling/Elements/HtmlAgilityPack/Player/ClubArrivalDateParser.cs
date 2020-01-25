using HtmlAgilityPack;
using System;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Elements.HtmlAgilityPack.Player
{
    class ClubArrivalDateParser : ElementParser<HtmlNode, DateTime?>
    {
        public override string DisplayName { get; set; } = "Club Arrival Date";

        public ClubArrivalDateParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == "Na equipa desde";

            this.ParseFunc = node =>
            {
                var parsedStr = node.InnerText;
                return Converter.Convert(parsedStr);
            };
        }
    }
}
