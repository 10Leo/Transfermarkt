﻿using HtmlAgilityPack;
using Transfermarkt.Core.ParseHandling.Elements.Club;

namespace Transfermarkt.Core.ParseHandling.Parsers.HtmlAgilityPack.Club
{
    class NameParser// : IElementParser<HtmlNode, string>
    {
        private string displayName = "Name";
        private bool parsedAlready = false;

        public IConverter<string> Converter { get; set; }

        public event EventHandler<CustomEventArgs> OnSuccess;
        public event EventHandler<CustomEventArgs> OnFailure;

        public bool CanParse(HtmlNode node)
        {
            //if (parsedAlready)
            //{
            //    return false;
            //}
            return true;
        }

        public string Parse(HtmlNode node)
        {
            string parsedObj = null;

            try
            {
                var parsedStr = node.SelectSingleNode("//div[@id='verein_head']//h1[@itemprop='name']/span")?.InnerText;

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
