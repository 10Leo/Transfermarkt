using HtmlAgilityPack;
using System.Linq;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Player;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Player
{
    class CaptainParser : ElementParser<Captain, IntValue, HtmlNode>
    {
        public CaptainParser()
        {
            this.CanParsePredicate = node => node?.InnerText?.Trim(Common.trimChars) == ParsersConfig.GetLabel(this.GetType(), ConfigType.PLAYER);

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
