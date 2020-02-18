using HtmlAgilityPack;
using System.Linq;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Player;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Player
{
    class NationalityParser : ElementParser<Nationality, Actors.Nationality, HtmlNode>
    {
        public NationalityParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == "Nac.";

            this.ParseFunc = node =>
            {
                var parsedStr = node
                    .SelectNodes("img")
                    .Where(n => n.Attributes["class"]?.Value == "flaggenrahmen")
                    .Select(n => n.Attributes["title"].Value)?.ToArray().FirstOrDefault();

                return new Nationality { Value = Converter.Convert(parsedStr) };
            };
        }
    }
}
