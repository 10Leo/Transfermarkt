using Transfermarkt.Core.Actors;

namespace Transfermarkt.Core.Contracts
{
    public interface IClubPage<TNode> : IPage
    {
        IMarketValueParser<TNode, decimal> MarketValue { get; set; }
        INationalityParser<TNode, Nationality?> Nationality { get; set; }
    }
}
