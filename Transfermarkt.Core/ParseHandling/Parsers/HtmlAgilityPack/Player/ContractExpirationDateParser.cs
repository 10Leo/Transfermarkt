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
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == ParsersConfig.GetLabel(this.GetType(), ConfigType.PLAYER);

            this.ParseFunc = node =>
            {
                var parsedStr = Regex.Replace(node.InnerText, @"\.", "/");

                return new ContractExpirationDate(DateTime.Now) { Value = Converter.Convert(parsedStr) };
            };
        }
    }
}
