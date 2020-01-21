using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.Contracts;

namespace Transfermarkt.Core.Parsers.HtmlAgilityPack.Continent
{
    class ContinentCodeParser : IElementParser<HtmlNode, ContinentCode?>
    {
        public IConverter<ContinentCode?> Converter { get; set; }

        public event EventHandler<CustomEventArgs> OnSuccess;
        public event EventHandler<CustomEventArgs> OnFailure;

        public bool CanParse(HtmlNode node)
        {
            return true;
        }

        public ContinentCode? Parse(HtmlNode node)
        {
            return null;
        }
    }
}
