using HtmlAgilityPack;
using System;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Club;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Club
{
    class CountryImgParser : ElementParser<CountryImg, StringValue, HtmlNode>
    {
        public CountryImgParser()
        {
            //TODO: right now this class handles the nodes passed the same amount of times because there's not a defined condition to see if the class can handle it
            this.CanParsePredicate = node => "" == ParsersConfig.Get(this.GetType(), ConfigType.CLUB);

            this.ParseFunc = node =>
            {
                HtmlNode countryNode = node.SelectSingleNode("//div[@id='verein_head']//span[@class='mediumpunkt']//img[@class='flaggenrahmen vm']");
                string parsedStr = countryNode?.GetAttributeValue<string>("src", null);

                return new CountryImg { Value = Converter.Convert(parsedStr) };
            };
        }
    }
}
