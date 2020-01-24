using HtmlAgilityPack;
using System;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Elements.HtmlAgilityPack.Club
{
    class CountryImgParser : ElementParser<HtmlNode, string>
    {
        public override string DisplayName { get; set; } = "Country Img";

        public CountryImgParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => true;

            this.ParseFunc = node =>
            {
                HtmlNode countryNode = node.SelectSingleNode("//div[@id='verein_head']//span[@class='mediumpunkt']//img[@class='flaggenrahmen vm']");
                string parsedStr = countryNode?.GetAttributeValue<string>("src", null);
                return Converter.Convert(parsedStr);
            };
        }
    }
}
