using HtmlAgilityPack;
using System;
using System.Linq;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.Parsers.HtmlAgilityPack.Player
{
    class ShortNameParser : ElementParser<HtmlNode, string>
    {
        public override string DisplayName { get; set; } = "Short Name";

        public ShortNameParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == "Jogadores";

            this.ParseFunc = node =>
            {
                var parsedStr = node
                    .SelectNodes("table//tr[1]/td[2]/div[2]")
                    .FirstOrDefault()
                    .InnerText;
                return Converter.Convert(parsedStr);
            };
        }
    }
}
