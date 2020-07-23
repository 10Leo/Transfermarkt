using HtmlAgilityPack;
using Page.Scraper.Contracts;
using System.Text.RegularExpressions;
using Transfermarkt.Core.ParseHandling.Elements.Player;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Player
{
    class HeightParser : ElementParser<Height, IntValue, HtmlNode>
    {
        public HeightParser()
        {
            this.CanParsePredicate = node => node?.InnerText?.Trim(Common.trimChars) == ParsersConfig.GetLabel(this.GetType(), ConfigType.PLAYER);

            this.ParseFunc = node =>
            {
                var parsedStr = Regex.Replace(node.InnerText, "([a-zA-Z,_ ]+|(?<=[a-zA-Z ])[/-])", "");

                return new Height { Value = Converter.Convert(parsedStr) };
            };
        }
    }
}
