using HtmlAgilityPack;
using Transfermarkt.Core.Contracts;

namespace Transfermarkt.Core.Parsers.HtmlAgilityPack
{
    public interface IHAPMarketValueParser : IMarketValueParser<HtmlNode, decimal>
    {
    }
}