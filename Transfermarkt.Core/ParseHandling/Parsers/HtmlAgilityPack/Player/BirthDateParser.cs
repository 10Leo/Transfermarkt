using HtmlAgilityPack;
using Page.Parser.Contracts;
using System;
using Transfermarkt.Core.ParseHandling.Elements.Player;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Player
{
    class BirthDateParser : ElementParser<BirthDate, DatetimeValue, HtmlNode>
    {
        public BirthDateParser()
        {
            this.CanParsePredicate = node => node?.InnerText?.Trim(Common.trimChars) == ParsersConfig.GetLabel(this.GetType(), ConfigType.PLAYER);

            this.ParseFunc = node =>
            {
                var parsedStr = node.InnerText?.Split(new[] { " (" }, StringSplitOptions.None)?[0];

                return new BirthDate { Value = Converter.Convert(parsedStr) };
            };
        }
    }
}
