using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.Contracts;
using Transfermarkt.Core.Contracts.Converters;

namespace Transfermarkt.Core.Parsers.HtmlAgilityPack.Player
{
    public class NationalityParser : IHAPNationalityParser
    {
        public event EventHandler<CustomEventArgs> OnSuccess;
        public event EventHandler<CustomEventArgs> OnFailure;

        public IConverter<Nationality?> Converter { get; set; }

        private bool parsedAlready = false;

        public bool CanParse(HtmlNode node)
        {
            //if (parsedAlready)
            //{
            //    return false;
            //}

            var headerName = node.InnerText.Trim(' ', '\t', '\n');

            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            var equals = (headerName == "Nac.");
            if (equals)
            {
                parsedAlready = true;
            }

            //TODO: está em PT. Ir buscar a ficheiro de settings de acordo com a linguagem escolhida.
            return equals;
        }

        public Nationality? Parse(HtmlNode node)
        {
            Nationality? nat = null;

            try
            {
                var sp = node
                    .SelectNodes("img")
                    .Where(n => n.Attributes["class"]?.Value == "flaggenrahmen")
                    .Select(n => n.Attributes["title"].Value)?.ToArray().FirstOrDefault();

                nat = Converter.Convert(sp);

                OnSuccess?.Invoke(this, new CustomEventArgs("Success parsing Nationality"));
            }
            catch (Exception)
            {
                OnFailure?.Invoke(this, new CustomEventArgs("Error parsing Nationality"));
                throw;
            }

            return nat;
        }
    }
}
