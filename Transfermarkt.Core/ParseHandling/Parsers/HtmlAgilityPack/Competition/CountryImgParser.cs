using HtmlAgilityPack;
using System;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Competition;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Competition
{
    //TODO: consider extracting IElement from the page and pass it as a generic. e.g. CountryImgParser<CountryImg>
    class CountryImgParser : ElementParser<CountryImg, HtmlNode>
    {
        public override CountryImg Element { get; } = new CountryImg();

        public CountryImgParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => true;

            this.ParseFunc = node =>
            {
                HtmlNode countryNode = node.SelectSingleNode("//div[@id='wettbewerb_head']//img[@class='flaggenrahmen']");
                var parsedStr = countryNode?.GetAttributeValue<string>("src", null);

                Element.Value = Converter.Convert(parsedStr);
                return Element;
            };
        }
    }
}
