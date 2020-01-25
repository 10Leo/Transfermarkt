using HtmlAgilityPack;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Elements.HtmlAgilityPack.Competition
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
                var parsedStr = node.SelectSingleNode("//div[@id='wettbewerb_head']//div[@class='headerfoto']/img")?.GetAttributeValue<string>("src", null);
                return Converter.Convert(parsedStr);
            };
        }
    }
}
