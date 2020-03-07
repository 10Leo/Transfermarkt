using HtmlAgilityPack;
using System;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Continent
{
    class NameParser : ElementParser<Elements.Continent.Name, StringValue, HtmlNode>
    {
        public NameParser()
        {
            this.CanParsePredicate = node => "" == ParsersConfig.GetLabel(this.GetType(), ConfigType.CONTINENT);

            this.ParseFunc = node =>
            {
                return new Elements.Continent.Name { };
            };
        }
    }
}
