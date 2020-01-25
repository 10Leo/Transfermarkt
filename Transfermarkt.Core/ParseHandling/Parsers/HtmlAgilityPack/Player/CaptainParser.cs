using HtmlAgilityPack;
using System;
using System.Linq;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Player
{
    class CaptainParser// : IElementParser<HtmlNode, int?>
    {
        public IConverter<int?> Converter { get; set; }

        public event EventHandler<CustomEventArgs> OnSuccess;
        public event EventHandler<CustomEventArgs> OnFailure;

        private string displayName = "Captain";
        private bool parsedAlready = false;

        public bool CanParse(HtmlNode node)
        {
            //if (parsedAlready)
            //{
            //    return false;
            //}

            var headerName = node?.InnerText?.Trim(' ', '\t', '\n');

            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            var equals = (headerName == "Jogadores");

            //TODO: está em PT. Ir buscar a ficheiro de settings de acordo com a linguagem escolhida.
            return equals;
        }

        public int? Parse(HtmlNode node)
        {
            int? parsedObj = null;

            try
            {
                var cap = node
                    .SelectNodes("table//tr[1]/td[2]/span")?
                    .FirstOrDefault(n => (n.Attributes["class"]?.Value).Contains("kapitaenicon-table"));
                var parsedStr = (cap == null) ? "0" : "1";

                parsedObj = Converter.Convert(parsedStr);

                OnSuccess?.Invoke(this, new CustomEventArgs($"Success parsing {displayName}."));
                parsedAlready = true;
            }
            catch (Exception)
            {
                OnFailure?.Invoke(this, new CustomEventArgs($"Error parsing {displayName}."));
                throw;
            }

            return parsedObj;
        }
    }
}
