using HtmlAgilityPack;
using Page.Parser.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Club;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Club
{
    class CountryImgParser : ElementParser<CountryImg, StringValue, HtmlNode>
    {
        public CountryImgParser()
        {
            this.CanParsePredicate = node => true;// "" == ParsersConfig.GetLabel(this.GetType(), ConfigType.CLUB);

            this.ParseFunc = node =>
            {
                HtmlNode countryNode = node.SelectSingleNode("//div[@id='verein_head']//span[@class='mediumpunkt']//img[@class='flaggenrahmen vm']");
                string parsedStr = countryNode?.GetAttributeValue<string>("src", null);

                return new CountryImg { Value = Converter.Convert(parsedStr) };
            };
        }
    }
}
