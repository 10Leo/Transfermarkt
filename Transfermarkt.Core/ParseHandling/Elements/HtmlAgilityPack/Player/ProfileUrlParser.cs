using HtmlAgilityPack;
using System;
using System.Linq;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Elements.HtmlAgilityPack.Player
{
    class ProfileUrlParser : ElementParser<HtmlNode, string>
    {
        public override string DisplayName { get; set; } = "Profile Url";

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
                return Converter.Convert(parsedStr);
            };
        }
    }
}
