using HtmlAgilityPack;
using Page.Parser.Contracts;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Continent
{
    class NameParser : ElementParser<Elements.Continent.Name, StringValue, HtmlNode>
    {
        public NameParser()
        {
            this.CanParsePredicate = node => true;//"" == ParsersConfig.GetLabel(this.GetType(), ConfigType.CONTINENT);

            this.ParseFunc = node =>
            {
                var n1 = node.SelectSingleNode("//div[@id='stickyContent']");
                var parsedStr = node.SelectSingleNode("//div[@id='main']//div[@class='table-header']/h2")?.InnerText;
                return new Elements.Continent.Name { Value = Converter.Convert(parsedStr) };
            };
        }
    }
}
