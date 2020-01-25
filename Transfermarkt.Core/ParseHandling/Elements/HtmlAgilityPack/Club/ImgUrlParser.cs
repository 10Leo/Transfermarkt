using HtmlAgilityPack;
using System;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Elements.HtmlAgilityPack.Club
{
    class ImgUrlParser : ElementParser<HtmlNode, string>
    {
        public override string DisplayName { get; set; } = "Img Url";

        public ImgUrlParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => true;

            this.ParseFunc = node =>
            {
                var parsedStr = node.SelectSingleNode("//div[@id='verein_head']//div[@class='dataBild ']/img")?.GetAttributeValue<string>("src", null);
                return Converter.Convert(parsedStr);
            };
        }
    }
}
