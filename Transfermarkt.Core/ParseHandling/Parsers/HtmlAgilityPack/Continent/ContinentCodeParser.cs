using HtmlAgilityPack;
using System;
using Transfermarkt.Core.Actors;
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
                return new Transfermarkt.Core.ParseHandling.Elements.Continent.ContinentCode { };
            };
        }
    }
}
