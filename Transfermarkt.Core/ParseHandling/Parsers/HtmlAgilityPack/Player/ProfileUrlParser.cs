using HtmlAgilityPack;
using System.Linq;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Player;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Player
{
    class ProfileUrlParser : ElementParser<ProfileUrl, StringValue, HtmlNode>
    {
        public ProfileUrlParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == "Jogadores";

            this.ParseFunc = node =>
            {
                var parsedStr = node
                    .SelectNodes("table//td//a")
                    .FirstOrDefault(n => n.Attributes["class"]?.Value == "spielprofil_tooltip")
                    .Attributes["href"].Value;

                return new ProfileUrl { Value = Converter.Convert(parsedStr) };
            };
        }
    }
}
