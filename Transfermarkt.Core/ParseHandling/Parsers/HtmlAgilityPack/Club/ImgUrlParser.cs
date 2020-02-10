using HtmlAgilityPack;
using System;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Club;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Club
{
    class ImgUrlParser : ElementParser<HtmlNode>
    {
        public override IElement Element { get; } = new ImgUrl();

        public ImgUrlParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => true;

            this.ParseFunc = node =>
            {
                var parsedStr = node.SelectSingleNode("//div[@id='verein_head']//div[@class='dataBild ']/img")?.GetAttributeValue<string>("src", null);

                Element.Value = Converter.Convert(parsedStr);
                return Element;
            };
        }
    }

}
