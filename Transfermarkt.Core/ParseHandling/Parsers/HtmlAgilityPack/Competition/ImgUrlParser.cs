using HtmlAgilityPack;
using Page.Parser.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Competition;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Competition
{
    class ImgUrlParser : ElementParser<ImgUrl, StringValue, HtmlNode>
    {
        public ImgUrlParser()
        {
            this.CanParsePredicate = node => true;//"" == ParsersConfig.GetLabel(this.GetType(), ConfigType.COMPETITION);

            this.ParseFunc = node =>
            {
                var parsedStr = node.SelectSingleNode("//div[@id='wettbewerb_head']//div[@class='headerfoto']/img")?.GetAttributeValue<string>("src", null);

                return new ImgUrl { Value = Converter.Convert(parsedStr) };
            };
        }
    }
}
