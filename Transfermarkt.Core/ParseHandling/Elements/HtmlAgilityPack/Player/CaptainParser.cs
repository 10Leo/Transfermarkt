using HtmlAgilityPack;
using System;
using System.Linq;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Elements.HtmlAgilityPack.Player
{
    class CaptainParser : ElementParser<HtmlNode, int?>
    {
        public override string DisplayName { get; set; } = "Captain";

        public CaptainParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == "Jogadores";

            this.ParseFunc = node =>
            {
                var cap = node
                    .SelectNodes("table//tr[1]/td[2]/span")?
                    .FirstOrDefault(n => (n.Attributes["class"]?.Value).Contains("kapitaenicon-table"));
                return Converter.Convert((cap == null) ? "0" : "1");
            };
        }
    }
}
