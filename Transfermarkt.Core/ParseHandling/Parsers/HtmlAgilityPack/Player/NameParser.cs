using HtmlAgilityPack;
using Page.Parser.Contracts;
using System.Linq;
using Transfermarkt.Core.ParseHandling.Elements.Player;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Player
{
    class NameParser : ElementParser<Name, StringValue, HtmlNode>
    {
        public NameParser()
        {
            this.CanParsePredicate = node => node?.InnerText?.Trim(Common.trimChars) == ParsersConfig.GetLabel(this.GetType(), ConfigType.PLAYER);

            this.ParseFunc = node =>
            {
                var parsedStr = node
                    .SelectNodes("table//tr[1]/td[2]/div[1]")
                    .FirstOrDefault()
                    .InnerText;

                return new Name { Value = Converter.Convert(parsedStr) };
            };
        }
    }
}
