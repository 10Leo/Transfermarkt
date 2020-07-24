﻿using HtmlAgilityPack;
using Page.Scraper.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Competition;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Competition
{
    class SeasonParser : ElementParser<Season, IntValue, HtmlNode>
    {
        public SeasonParser()
        {
            this.CanParsePredicate = node => true;//"" == ParsersConfig.GetLabel(this.GetType(), ConfigType.COMPETITION);

            this.ParseFunc = node =>
            {
                int? parsedStr = node.SelectSingleNode("//select[@name='saison_id']//option[@selected='selected']")?.GetAttributeValue<int>("value", 0);
                if (!parsedStr.HasValue)
                {
                    parsedStr = 0;
                }

                //TODO: the value to pass is an int but the metthod requires a string. Maybe change the receiver argument to be a generic.
                return new Season(parsedStr.ToString());
            };
        }
    }
}
