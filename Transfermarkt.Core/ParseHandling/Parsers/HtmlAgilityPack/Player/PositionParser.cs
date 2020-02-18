using HtmlAgilityPack;
using System.Linq;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Player;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Player
{
    class PositionParser : ElementParser<Position, Actors.Position, HtmlNode>
    {
        public PositionParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == "Jogadores";

            this.ParseFunc = node =>
            {
                var parsedStr = node
                    .SelectNodes("table//tr[2]/td[1]")
                    .FirstOrDefault()
                    .InnerText;

                return new Position { Value = Converter.Convert(parsedStr) };
            };
        }
    }
}
