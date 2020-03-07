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
        //TODO: unit test to guarantee all of them have the suffix "Parser", which is expected on load of theses configs
        public BirthDateParser()
        {
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == ParsersConfig.GetLabel(this.GetType(), ConfigType.PLAYER);

            this.ParseFunc = node =>
            {
                var parsedStr = node.InnerText?.Split(new[] { " (" }, StringSplitOptions.None)?[0];

                return new BirthDate { Value = Converter.Convert(parsedStr) };
            };
        }
    }
}
