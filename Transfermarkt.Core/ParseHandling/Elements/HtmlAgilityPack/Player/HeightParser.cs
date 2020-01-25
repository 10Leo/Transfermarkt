using HtmlAgilityPack;
using System;
using System.Text.RegularExpressions;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Elements.HtmlAgilityPack.Player
{
    class HeightParser : ElementParser<HtmlNode, int?>
    {
        public override string DisplayName { get; set; } = "Height";

        public HeightParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == "Altura";

            this.ParseFunc = node =>
            {
                var parsedStr = Regex.Replace(node.InnerText, "([a-zA-Z,_ ]+|(?<=[a-zA-Z ])[/-])", "");
                return Converter.Convert(parsedStr);
            };
        }
    }
}
