﻿using HtmlAgilityPack;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Competition;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Competition
{
    class NameParser : ElementParser<Name, string, HtmlNode>
    {
        public NameParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => true;

            this.ParseFunc = node =>
            {
                var parsedStr = node.SelectSingleNode("//div[@id='wettbewerb_head']//h1[@class='spielername-profil']")?.InnerText;

                return new Name { Value = new StringValue { Value = Converter.Convert(parsedStr) } };
            };
        }
    }
}
