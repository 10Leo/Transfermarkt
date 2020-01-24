using HtmlAgilityPack;
using System;
using System.Text.RegularExpressions;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Elements.HtmlAgilityPack.Player
{
    class ContractExpirationDateParser// : IElementParser<HtmlNode, DateTime?>
    {
        public IConverter<DateTime?> Converter { get; set; }

        public event EventHandler<CustomEventArgs> OnSuccess;
        public event EventHandler<CustomEventArgs> OnFailure;

        private string displayName = "Contract Expiration Date";
        private bool parsedAlready = false;

        public bool CanParse(HtmlNode node)
        {
            //if (parsedAlready)
            //{
            //    return false;
            //}

            var headerName = node?.InnerText?.Trim(' ', '\t', '\n');

            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            var equals = (headerName == "Contrato até");

            //TODO: está em PT. Ir buscar a ficheiro de settings de acordo com a linguagem escolhida.
            return equals;
        }

        public DateTime? Parse(HtmlNode node)
        {
            DateTime? parsedObj = null;

            try
            {
                var parsedStr = Regex.Replace(node.InnerText, @"\.", "/");

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
