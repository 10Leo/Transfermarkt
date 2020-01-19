using HtmlAgilityPack;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.Contracts;

namespace Transfermarkt.Core.Parsers.HtmlAgilityPack
{
    public interface IHAPNationalityParser : INationalityParser<HtmlNode, Nationality?>
    {
    }
}
