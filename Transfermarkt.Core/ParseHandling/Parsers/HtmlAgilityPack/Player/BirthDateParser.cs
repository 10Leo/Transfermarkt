using HtmlAgilityPack;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Transfermarkt.Core.Contracts;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Player;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Player
{
    class BirthDateParser : ElementParser<BirthDate, DatetimeValue, HtmlNode>
    {
        public BirthDateParser()
        {
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == ParsersConfig.Get(this.GetType(), ConfigType.PLAYER);

            this.ParseFunc = node =>
            {
                var parsedStr = node.InnerText?.Split(new[] { " (" }, StringSplitOptions.None)?[0];

                return new BirthDate { Value = Converter.Convert(parsedStr) };
            };
        }
    }
}
