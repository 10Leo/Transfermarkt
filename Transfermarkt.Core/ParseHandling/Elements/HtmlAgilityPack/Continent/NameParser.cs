using HtmlAgilityPack;
using System;
using Transfermarkt.Core.ParseHandling.Contracts;

namespace Transfermarkt.Core.ParseHandling.Elements.HtmlAgilityPack.Continent
{
    class NameParser// : IElementParser<HtmlNode, string>
    {
        public IConverter<string> Converter { get; set; }

        public event EventHandler<CustomEventArgs> OnSuccess;
        public event EventHandler<CustomEventArgs> OnFailure;

        public bool CanParse(HtmlNode node)
        {
            return true;
        }

        public string Parse(HtmlNode node)
        {
            return null;
        }
    }
}
