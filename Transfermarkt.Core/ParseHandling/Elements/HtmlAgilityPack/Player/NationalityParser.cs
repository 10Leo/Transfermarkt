using HtmlAgilityPack;
using System;
using System.Linq;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Contracts.Element;

namespace Transfermarkt.Core.ParseHandling.Elements.HtmlAgilityPack.Player
{
    class NationalityParser : ElementParser<HtmlNode, Nationality?>
    {
        public override string DisplayName { get; set; } = "Nationality";

        public NationalityParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == "Nac.";

            this.ParseFunc = node =>
            {
                var sp = node
                    .SelectNodes("img")
                    .Where(n => n.Attributes["class"]?.Value == "flaggenrahmen")
                    .Select(n => n.Attributes["title"].Value)?.ToArray().FirstOrDefault();

                return Converter.Convert(sp);
            };
        }
    }
}
