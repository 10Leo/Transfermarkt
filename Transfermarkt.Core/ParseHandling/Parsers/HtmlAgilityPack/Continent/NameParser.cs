using HtmlAgilityPack;
using System;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Continent
{
    class NameParser : ElementParser<HtmlNode>
    {
        public override IElement Element { get; } = new Transfermarkt.Core.ParseHandling.Elements.Continent.Name();

        public NameParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == "";

            this.ParseFunc = node =>
            {
                return Element;
            };
        }
    }
}
