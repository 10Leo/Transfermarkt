using HtmlAgilityPack;
using System;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Elements.HtmlAgilityPack.Competition
{
    class SeasonParser : ElementParser<HtmlNode, int?>
    {
        public override string DisplayName { get; set; } = "Season";

        public SeasonParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => true;

            this.ParseFunc = node =>
            {
                int? parsedStr = node.SelectSingleNode("//select[@name='saison_id']//option")?.GetAttributeValue<int>("value", 0);
                if (!parsedStr.HasValue)
                {
                    parsedStr = 0;
                }
                //TODO: the value to pass is an int but the metthod requires a string. Maybe change the receiver argument to be a generic.
                int? parsedObj = Converter.Convert(parsedStr.ToString());
                return parsedObj;
            };
        }
    }
}
