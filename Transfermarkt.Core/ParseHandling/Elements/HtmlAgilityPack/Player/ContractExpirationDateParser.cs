using HtmlAgilityPack;
using System;
using System.Text.RegularExpressions;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Elements.HtmlAgilityPack.Player
{
    class ContractExpirationDateParser : ElementParser<HtmlNode, DateTime?>
    {
        public override string DisplayName { get; set; } = "Contract Expiration Date";

        public ContractExpirationDateParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == "Contrato até";

            this.ParseFunc = node =>
            {
                var parsedStr = Regex.Replace(node.InnerText, @"\.", "/");
                return Converter.Convert(parsedStr);
            };
        }
    }
}
