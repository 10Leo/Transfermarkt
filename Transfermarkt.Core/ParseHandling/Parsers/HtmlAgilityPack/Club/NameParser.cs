using HtmlAgilityPack;
using Page.Parser.Contracts;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Club;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Club
{
    class NameParser : ElementParser<Name, StringValue, HtmlNode>
    {
        public NameParser()
        {
            this.CanParsePredicate = node => true;//"" == ParsersConfig.GetLabel(this.GetType(), ConfigType.CLUB);

            this.ParseFunc = node =>
            {
                var parsedStr = node.SelectSingleNode("//div[@id='verein_head']//h1[@itemprop='name']/span")?.InnerText;

                return new Name { Value = Converter.Convert(parsedStr) };
            };
        }
    }
}
