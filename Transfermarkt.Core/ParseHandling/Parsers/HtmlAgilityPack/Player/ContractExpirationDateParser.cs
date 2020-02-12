using HtmlAgilityPack;
using System.Text.RegularExpressions;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Player;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Player
{
    class ContractExpirationDateParser : ElementParser<ContractExpirationDate, HtmlNode>
    {
        public override ContractExpirationDate Element { get; } = new ContractExpirationDate();

        public ContractExpirationDateParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == "Contrato até";

            this.ParseFunc = node =>
            {
                var parsedStr = Regex.Replace(node.InnerText, @"\.", "/");

                Element.Value = Converter.Convert(parsedStr);
                return Element;
            };
        }
    }
}
