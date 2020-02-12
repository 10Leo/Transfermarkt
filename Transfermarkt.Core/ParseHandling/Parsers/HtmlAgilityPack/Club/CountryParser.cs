using HtmlAgilityPack;
using System;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Club;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Club
{
    class CountryParser : ElementParser<Country, HtmlNode>
    {
        public override Country Element { get; } = new Country();

        public CountryParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => true;

            this.ParseFunc = node =>
            {
                HtmlNode countryNode = node.SelectSingleNode("//div[@id='verein_head']//span[@class='mediumpunkt']//img[@class='flaggenrahmen vm']");
                string parsedStr = countryNode?.GetAttributeValue<string>("title", null);

                Element.Value = Converter.Convert(parsedStr);
                return Element;
            };
        }
    }
}
