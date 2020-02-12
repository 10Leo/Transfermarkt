using HtmlAgilityPack;
using System.Linq;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Player;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Player
{
    class NameParser : ElementParser<Name, HtmlNode>
    {
        public override Name Element { get; } = new Name();

        public NameParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == "Jogadores";

            this.ParseFunc = node =>
            {
                var parsedStr = node
                    .SelectNodes("table//tr[1]/td[2]/div[1]")
                    .FirstOrDefault()
                    .InnerText;

                Element.Value = Converter.Convert(parsedStr);
                return Element;
            };
        }
    }
}
