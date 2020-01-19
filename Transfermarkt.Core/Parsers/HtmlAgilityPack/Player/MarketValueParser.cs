using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.Contracts;

namespace Transfermarkt.Core.Parsers.HtmlAgilityPack.Player
{
    public class MarketValueParser : IMarketValueParser<HtmlNode>
    {
        public IConverter<decimal?> Converter { get; set; }

        public event EventHandler<CustomEventArgs> OnSuccess;
        public event EventHandler<CustomEventArgs> OnFailure;

        private string displayName = "Market Value";
        private bool parsedAlready = false;

        public bool CanParse(HtmlNode node)
        {
            //if (parsedAlready)
            //{
            //    return false;
            //}

            var headerName = node.InnerText.Trim(' ', '\t', '\n');

            var equals = (headerName == "Valor de mercado");
            if (equals)
            {
                parsedAlready = true;
            }

            //TODO: está em PT. Ir buscar a ficheiro de settings de acordo com a linguagem escolhida.
            return equals;
        }

        public decimal? Parse(HtmlNode node)
        {
            decimal? n = null;

            try
            {
                var sp = node.InnerText?.Split(new[] { " " }, StringSplitOptions.None);

                if (sp == null || sp.Length < 2)
                {
                    OnFailure?.Invoke(this, new CustomEventArgs("Error parsing Market Value"));
                    return null;
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

                OnSuccess?.Invoke(this, new CustomEventArgs("Success parsing Market Value"));
            }
            catch (Exception)
            {
                OnFailure?.Invoke(this, new CustomEventArgs("Error parsing Market Value"));
                throw;
            }
            
            return n;
        }
    }
}
