﻿using HtmlAgilityPack;
using System;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Continent
{
    class ContinentCodeParser : ElementParser<Elements.Continent.ContinentCode, StringValue, HtmlNode>
    {
        public ContinentCodeParser()
        {
            this.CanParsePredicate = node => "" == ParsersConfig.Get(this.GetType(), ConfigType.CONTINENT);

            this.ParseFunc = node =>
            {
                return new Transfermarkt.Core.ParseHandling.Elements.Continent.ContinentCode { };
            };
        }
    }
}
