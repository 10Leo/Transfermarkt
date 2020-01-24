using HtmlAgilityPack;
using System;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Elements.HtmlAgilityPack.Player
{
    class PlayerImgUrlParser : ElementParser<HtmlNode, string>
    {
        public override string DisplayName { get; set; } = "Player Img Url";

        public PlayerImgUrlParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == "Jogadores";

            this.ParseFunc = node =>
            {
                //TODO
                var parsedStr = "";
                return Converter.Convert(parsedStr);
            };
        }
    }
}
