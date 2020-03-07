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
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == ParsersConfig.GetLabel(this.GetType(), ConfigType.PLAYER);

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
