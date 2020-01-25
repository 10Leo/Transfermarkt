using HtmlAgilityPack;
using System;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Elements.HtmlAgilityPack.Continent
{
    class NameParser : ElementParser<HtmlNode, string>
    {
        public override string DisplayName { get; set; } = "Name";

        public NameParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => false;

            this.ParseFunc = node =>
            {
                string parsedObj = string.Empty;
                return parsedObj;
            };
        }
    }
}
