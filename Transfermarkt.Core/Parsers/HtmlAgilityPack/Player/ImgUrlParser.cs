﻿using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.Contracts;

namespace Transfermarkt.Core.Parsers.HtmlAgilityPack.Player
{
    class ImgUrlParser : IElementParser<HtmlNode, string>
    {
        public IConverter<string> Converter { get; set; }

        public event EventHandler<CustomEventArgs> OnSuccess;
        public event EventHandler<CustomEventArgs> OnFailure;

        private string displayName = "Img Url";
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
            if (equals)
            {
                parsedAlready = true;
            }

            //TODO: está em PT. Ir buscar a ficheiro de settings de acordo com a linguagem escolhida.
            return equals;
        }

        public string Parse(HtmlNode node)
        {
            string parsedObj = null;

            try
            {
                //TODO
                var parsedStr = "";

                parsedObj = Converter.Convert(parsedStr);

                OnSuccess?.Invoke(this, new CustomEventArgs($"Success parsing {displayName}."));
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
