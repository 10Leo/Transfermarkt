using HtmlAgilityPack;
using Page.Parser.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Club;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Club
{
    class SeasonParser : ElementParser<Season, IntValue, HtmlNode>
    {
        public SeasonParser()
        {
            this.CanParsePredicate = node => true;//"" == ParsersConfig.GetLabel(this.GetType(), ConfigType.CLUB);

            this.ParseFunc = node =>
            {
                int? parsedStr = node.SelectSingleNode("//select[@name='saison_id']//option[@selected='selected']")?.GetAttributeValue<int>("value", 0);
                if (!parsedStr.HasValue)
                {
                    parsedStr = 0;
                }

                return new Season { Value = Converter.Convert(parsedStr.ToString()) };
            };
        }
    }
}
