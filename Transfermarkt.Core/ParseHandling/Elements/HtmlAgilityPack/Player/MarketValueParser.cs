using HtmlAgilityPack;
using System;
using Transfermarkt.Core.ParseHandling.Contracts;
using Transfermarkt.Core.ParseHandling.Contracts.Element;

namespace Transfermarkt.Core.ParseHandling.Elements.HtmlAgilityPack.Player
{
    class MarketValueParser : ElementParser<HtmlNode, decimal?>
    {
        public override string DisplayName { get; set; } = "Market Value";

        public MarketValueParser()
        {
            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            this.CanParsePredicate = node => node?.InnerText?.Trim(' ', '\t', '\n') == "Valor de mercado";

            this.ParseFunc = node =>
            {
                var sp = node.InnerText?.Split(new[] { " " }, StringSplitOptions.None);

                if (sp == null || sp.Length < 2)
                {
                    throw new Exception("Invalid number of splits");
                }

                var spl = sp[0].Split(new[] { "," }, StringSplitOptions.None);
                decimal? n = decimal.Parse(spl[0]);

                if (sp[1] == "M")
                {
                    n = n * 1000000;
                }
                else if (sp[1] == "mil")
                {
                    n = n * 1000;
                }
                //TODO: use converter
                return n;
            };
        }
    }
}
