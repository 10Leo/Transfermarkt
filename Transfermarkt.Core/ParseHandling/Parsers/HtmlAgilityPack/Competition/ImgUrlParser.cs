using HtmlAgilityPack;
using System;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Competition;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Competition
{
    class ImgUrlParser : ElementParser<ImgUrl, StringValue, HtmlNode>
    {
        public ImgUrlParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => true;

            this.ParseFunc = node =>
            {
                var parsedStr = node.SelectSingleNode("//div[@id='wettbewerb_head']//div[@class='headerfoto']/img")?.GetAttributeValue<string>("src", null);
                
                return new ImgUrl { Value = Converter.Convert(parsedStr) };
            };
        }
    }
}
