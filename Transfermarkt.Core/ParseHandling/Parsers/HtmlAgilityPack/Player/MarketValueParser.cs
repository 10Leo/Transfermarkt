﻿using HtmlAgilityPack;
using System;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Elements.Player;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Player
{
    class MarketValueParser : ElementParser<HtmlNode>
    {
        public override IElement Element { get; } = new MarketValue();

        public MarketValueParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == "Valor de mercado";

            this.ParseFunc = node =>
            {
                decimal? n = null;
                var sp = node.InnerText?.Split(new[] { " " }, StringSplitOptions.None);

                if (sp == null || sp.Length < 2)
                {
                    throw new Exception("Invalid number of splits");
                }

                var spl = sp[0].Split(new[] { "," }, StringSplitOptions.None);
                n = decimal.Parse(spl[0]);

                if (sp[1] == "M")
                {
                    n = n * 1000000;
                }
                else if (sp[1] == "mil")
                {
                    n = n * 1000;
                }

                Element.Value = Converter.Convert(n.ToString());
                return Element;
            };
        }
    }
}