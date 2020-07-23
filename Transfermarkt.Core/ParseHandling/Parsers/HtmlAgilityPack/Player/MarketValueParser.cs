using HtmlAgilityPack;
using Page.Scraper.Contracts;
using System;
using Transfermarkt.Core.ParseHandling.Elements.Player;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Player
{
    class MarketValueParser : ElementParser<MarketValue, DecimalValue, HtmlNode>
    {
        public MarketValueParser()
        {
            this.CanParsePredicate = node => node?.InnerText?.Trim(Common.trimChars) == ParsersConfig.GetLabel(this.GetType(), ConfigType.PLAYER);

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

                return new MarketValue { Value = Converter.Convert(n.ToString()) };
            };
        }
    }
}
