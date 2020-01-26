using HtmlAgilityPack;
using System;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Competition;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Competition
{
    class CountryImgParser : ElementParser<HtmlNode>
    {
        public override string DisplayName { get; set; } = "Country Img";

        public CountryImgParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == "";

            this.ParseFunc = node =>
            {
                HtmlNode countryNode = node.SelectSingleNode("//div[@id='wettbewerb_head']//img[@class='flaggenrahmen']");
                var parsedStr = countryNode?.GetAttributeValue<string>("src", null);
                return new CountryImg { Value = Converter.Convert(parsedStr) };
            };
        }
    }
}
