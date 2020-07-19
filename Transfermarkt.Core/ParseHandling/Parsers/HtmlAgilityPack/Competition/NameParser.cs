using HtmlAgilityPack;
using Page.Parser.Contracts;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Competition;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Competition
{
    class NameParser : ElementParser<Name, StringValue, HtmlNode>
    {
        public NameParser()
        {
            this.CanParsePredicate = node => true;//"" == ParsersConfig.GetLabel(this.GetType(), ConfigType.COMPETITION);

            this.ParseFunc = node =>
            {
                var parsedStr = node.SelectSingleNode("//div[@id='wettbewerb_head']//h1[@class='spielername-profil']")?.InnerText;

                return new Name { Value = Converter.Convert(parsedStr) };
            };
        }
    }
}
