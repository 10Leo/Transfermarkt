using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.Contracts;

namespace Transfermarkt.Core.Parsers.Player
{
    public class MarketValueParser : IMarketValueParser<HtmlNode>
    {
        public event EventHandler OnSuccess;
        public event EventHandler OnFailure;

        public bool CanParse(HtmlNode header)
        {
            var headerName = header.InnerText.Trim(' ', '\t', '\n');

            //TODO: está em PT. Ir buscar a ficheiro de settings de acordo com a linguagem escolhida.
            return (headerName == "Valor de mercado");
        }

        public string Parse(HtmlNode node)
        {
            int n = 0;

            try
            {
                var sp = node.InnerText?.Split(new[] { " " }, StringSplitOptions.None);

                if (sp == null || sp.Length < 2)
                {
                    OnFailure(this, new EventArgs());
                    return null;
                }

                var spl = sp[0].Split(new[] { "," }, StringSplitOptions.None);
                n = int.Parse(spl[0]);

                if (sp[1] == "M")
                {
                    n = n * 1000000;
                }
                else if (sp[1] == "mil")
                {
                    n = n * 1000;
                }

                OnSuccess(this, new EventArgs());
            }
            catch (Exception)
            {
                OnFailure(this, new EventArgs());
                throw;
            }
            
            return n.ToString();
        }
    }
}
