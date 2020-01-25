using HtmlAgilityPack;
using System;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Elements.HtmlAgilityPack.Competition
{
    class NameParser : ElementParser<HtmlNode, string>
    {
        public override string DisplayName { get; set; } = "Name";

        public NameParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => true;

            this.ParseFunc = node =>
            {
                var parsedStr = node.SelectSingleNode("//div[@id='wettbewerb_head']//h1[@class='spielername-profil']")?.InnerText;
                string parsedObj = Converter.Convert(parsedStr);
                return parsedObj;
            };
        }
    }
}
