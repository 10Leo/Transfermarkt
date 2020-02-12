using HtmlAgilityPack;
using System.Linq;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Player;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Player
{
    class CaptainParser : ElementParser<Captain, HtmlNode>
    {
        public CaptainParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == "Jogadores";

            this.ParseFunc = node =>
            {
                var cap = node
                    .SelectNodes("table//tr[1]/td[2]/span")?
                    .FirstOrDefault(n => (n.Attributes["class"]?.Value).Contains("kapitaenicon-table"));
                var parsedStr = (cap == null) ? "0" : "1";

                return new Captain { Value = Converter.Convert(parsedStr) };
            };
        }
    }
}
