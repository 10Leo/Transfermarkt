using HtmlAgilityPack;
using System;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Competition;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Competition
{
    //TODO: consider extracting IElement from the page and pass it as a generic. e.g. CountryImgParser<CountryImg>
    class CountryImgParser : ElementParser<CountryImg, string, HtmlNode>
    {
        public CountryImgParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => true;

            this.ParseFunc = node =>
            {
                HtmlNode countryNode = node.SelectSingleNode("//div[@id='wettbewerb_head']//img[@class='flaggenrahmen']");
                var parsedStr = countryNode?.GetAttributeValue<string>("src", null);

                return new CountryImg { Value = new StringValue { Value = Converter.Convert(parsedStr) } };
            };
        }
    }
}
