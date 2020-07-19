using HtmlAgilityPack;
using Page.Parser.Contracts;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Continent
{
    class ContinentCodeParser : ElementParser<Elements.Continent.ContinentCode, ContinentCodeValue, HtmlNode>
    {
        public ContinentCodeParser()
        {
            this.CanParsePredicate = node => true;//"" == ParsersConfig.GetLabel(this.GetType(), ConfigType.CONTINENT);

            this.ParseFunc = node =>
            {
                var parsedStr = node.SelectSingleNode("//div[@id='main']//div[@class='table-header']/h2")?.InnerText;
                return new Elements.Continent.ContinentCode { Value = Converter.Convert(parsedStr) };
            };
        }
    }
}
