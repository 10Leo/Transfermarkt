using HtmlAgilityPack;
using System;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Competition;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Competition
{
    class CountryParser : ElementParser<Country, Actors.Nationality?, HtmlNode>
    {
        public CountryParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => true;

            this.ParseFunc = node =>
            {
                HtmlNode countryNode = node.SelectSingleNode("//div[@id='wettbewerb_head']//img[@class='flaggenrahmen']");
                var parsedStr = countryNode?.GetAttributeValue<string>("title", null);

                return new Country { Value = new NationalityValue { Value = Converter.Convert(parsedStr) } };
            };
        }
    }
}
