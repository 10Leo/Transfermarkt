using HtmlAgilityPack;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Club;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Club
{
    class NameParser : ElementParser<Name, string, HtmlNode>
    {
        public NameParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => true;

            this.ParseFunc = node =>
            {
                var parsedStr = node.SelectSingleNode("//div[@id='verein_head']//h1[@itemprop='name']/span")?.InnerText;

                return new Name { /*Value = Converter.Convert(parsedStr)*/ };
            };
        }
    }
}
