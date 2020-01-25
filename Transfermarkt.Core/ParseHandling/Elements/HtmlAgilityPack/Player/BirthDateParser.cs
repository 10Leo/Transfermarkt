using HtmlAgilityPack;
using System;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Elements.HtmlAgilityPack.Player
{
    class BirthDateParser : ElementParser<HtmlNode, DateTime?>
    {
        public override string DisplayName { get; set; } = "Birth Date";

        public BirthDateParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == "Nasc. / idade";

            this.ParseFunc = node =>
            {
                var parsedStr = node.InnerText?.Split(new[] { " (" }, StringSplitOptions.None)?[0];
                return Converter.Convert(parsedStr);
            };
        }
    }
}
