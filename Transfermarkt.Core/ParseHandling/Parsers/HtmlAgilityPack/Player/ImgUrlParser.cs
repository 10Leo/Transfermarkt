﻿using HtmlAgilityPack;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Player;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Player
{
    class ImgUrlParser : ElementParser<HtmlNode>
    {
        public override IElement Element { get; } = new ImgUrl();

        public ImgUrlParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == "Jogadores";

            this.ParseFunc = node =>
            {
                //TODO
                var parsedStr = string.Empty;

                Element.Value = Converter.Convert(parsedStr);
                return Element;
            };
        }
    }
}