using HtmlAgilityPack;
using System;
using Transfermarkt.Core.Elements.Player;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Player
{
    class MarketValueParser : IElementParser<HtmlNode, IElement, object>
    {
        public IConverter<object> Converter { get; set; }

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

            //TODO: está em PT. Ir buscar a ficheiro de settings de acordo com a linguagem escolhida.
            return equals;
        }

        public IElement Parse(HtmlNode node)
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
                parsedAlready = true;
            }
            catch (Exception)
            {
                OnFailure?.Invoke(this, new CustomEventArgs("Error parsing Market Value"));
                throw;
            }
            
            return new MarketValue { Value = n };
        }
    }
}
