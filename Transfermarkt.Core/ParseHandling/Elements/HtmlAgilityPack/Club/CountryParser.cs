using HtmlAgilityPack;
using System;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Elements.HtmlAgilityPack.Club
{
    class CountryParser : ElementParser<HtmlNode, Nationality?>
    {
        public override string DisplayName { get; set; } = "Country";

        public CountryParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => true;

            this.ParseFunc = node =>
            {
                //TODO: if error occurs, push it to a list containing some props that will then be passed to OnFailure
                HtmlNode countryNode = node.SelectSingleNode("//div[@id='verein_head']//span[@class='mediumpunkt']//img[@class='flaggenrahmen vm']");
                string parsedStr = countryNode?.GetAttributeValue<string>("title", null);
                return Converter.Convert(parsedStr);
            };
        }
    }
}
