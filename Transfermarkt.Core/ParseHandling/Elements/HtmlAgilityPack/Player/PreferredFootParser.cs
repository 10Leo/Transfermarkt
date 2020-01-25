using HtmlAgilityPack;
using System;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Elements.HtmlAgilityPack.Player
{
    class PreferredFootParser : ElementParser<HtmlNode, Foot?>
    {
        public override string DisplayName { get; set; } = "Preferred Foot";

        public PreferredFootParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == "Pé";

            this.ParseFunc = node =>
            {
                var parsedStr = node
                    .InnerText;
                return Converter.Convert(parsedStr);
            };
        }
    }
}
