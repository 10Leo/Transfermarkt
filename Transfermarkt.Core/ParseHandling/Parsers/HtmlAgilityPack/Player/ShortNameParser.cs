using HtmlAgilityPack;
using Page.Parser.Contracts;
using System.Linq;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Player;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Player
{
    class ShortNameParser : ElementParser<ShortName, StringValue, HtmlNode>
    {
        public ShortNameParser()
        {
            this.CanParsePredicate = node => node?.InnerText?.Trim(Common.trimChars) == ParsersConfig.GetLabel(this.GetType(), ConfigType.PLAYER);

            this.ParseFunc = node =>
            {
                var parsedStr = node
                    .SelectNodes("table//tr[1]/td[2]/div[2]")
                    .FirstOrDefault()
                    .InnerText;

                return new ShortName{ Value = Converter.Convert(parsedStr) };
            };
        }
    }
}
