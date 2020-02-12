using HtmlAgilityPack;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Player;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Player
{
    class PreferredFootParser : ElementParser<PreferredFoot, HtmlNode>
    {
        public override PreferredFoot Element { get; } = new PreferredFoot();

        public PreferredFootParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == "Pé";

            this.ParseFunc = node =>
            {
                var parsedStr = node
                    .InnerText;

                Element.Value = Converter.Convert(parsedStr);
                return Element;
            };
        }
    }
}
