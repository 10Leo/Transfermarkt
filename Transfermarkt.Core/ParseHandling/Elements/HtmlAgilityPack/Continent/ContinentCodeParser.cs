using HtmlAgilityPack;
using System;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Elements.HtmlAgilityPack.Continent
{
    class ContinentCodeParser : ElementParser<HtmlNode, ContinentCode?>
    {
        public override string DisplayName { get; set; } = "Continent Code";

        public ContinentCodeParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => false;

            this.ParseFunc = node =>
            {
                ContinentCode? parsedObj = null;
                return parsedObj;
            };
        }
    }
}
