﻿using HtmlAgilityPack;
using System.Text.RegularExpressions;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Player;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Player
{
    class HeightParser : ElementParser<HtmlNode>
    {
        public override IElement Element { get; } = new Height();

        public HeightParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == "Altura";

            this.ParseFunc = node =>
            {
                var parsedStr = Regex.Replace(node.InnerText, "([a-zA-Z,_ ]+|(?<=[a-zA-Z ])[/-])", "");

                Element.Value = Converter.Convert(parsedStr);
                return Element;
            };
        }
    }
}