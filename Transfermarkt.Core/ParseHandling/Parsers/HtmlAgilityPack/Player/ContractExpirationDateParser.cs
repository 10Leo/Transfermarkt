using HtmlAgilityPack;
using System;
using System.Text.RegularExpressions;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Player;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Player
{
    class ContractExpirationDateParser : ElementParser<ContractExpirationDate, DatetimeValue, HtmlNode>
    {
        public ContractExpirationDateParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == "Contrato até";

            this.ParseFunc = node =>
            {
                var parsedStr = Regex.Replace(node.InnerText, @"\.", "/");

                return new ContractExpirationDate(DateTime.Now) { Value = Converter.Convert(parsedStr) };
            };
        }
    }
}
