﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.Contracts;

namespace Transfermarkt.Core.Parsers.HtmlAgilityPack.Player
{
    class ClubArrivalDateParser : IElementParser<HtmlNode, DateTime?>
    {
        public IConverter<DateTime?> Converter { get; set; }

        public event EventHandler<CustomEventArgs> OnSuccess;
        public event EventHandler<CustomEventArgs> OnFailure;

        private string displayName = "Club Arrival Date";
        private bool parsedAlready = false;

        public bool CanParse(HtmlNode node)
        {
            //if (parsedAlready)
            //{
            //    return false;
            //}

            var headerName = node?.InnerText?.Trim(' ', '\t', '\n');

            //TODO: change so that this value comes from a settings json file according to what's defined on config.
            var equals = (headerName == "Na equipa desde");

            //TODO: está em PT. Ir buscar a ficheiro de settings de acordo com a linguagem escolhida.
            return equals;
        }

        public DateTime? Parse(HtmlNode node)
        {
            DateTime? parsedObj = null;

            try
            {
                var parsedStr = node.InnerText;

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