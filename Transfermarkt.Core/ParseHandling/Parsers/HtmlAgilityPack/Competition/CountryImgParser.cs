using HtmlAgilityPack;
using Page.Parser.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Competition;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Competition
{
    class CountryImgParser : ElementParser<CountryImg, StringValue, HtmlNode>
    {
        public CountryImgParser()
        {
            this.CanParsePredicate = node => true;//"" == ParsersConfig.GetLabel(this.GetType(), ConfigType.COMPETITION);

            this.ParseFunc = node =>
            {
                HtmlNode countryNode = node.SelectSingleNode("//div[@id='wettbewerb_head']//img[@class='flaggenrahmen']");
                var parsedStr = countryNode?.GetAttributeValue<string>("src", null);

                return new CountryImg { Value = Converter.Convert(parsedStr) };
            };
        }
    }
}
