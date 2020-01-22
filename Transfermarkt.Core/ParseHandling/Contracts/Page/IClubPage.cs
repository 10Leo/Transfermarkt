using System;
using Transfermarkt.Core.Actors;
using Transfermarkt.Core.ParseHandling.Contracts.Element;

namespace Transfermarkt.Core.ParseHandling.Contracts.Page
{
    public interface IClubPage<TNode> : IPage<IDomain>
    {
        //TODO: consider subpage to encapsulate logical parts together
        IElementParser<TNode, int?> Season { get; set; }
        IElementParser<TNode, Nationality?> Country { get; set; }
        IElementParser<TNode, string> Name { get; set; }
        IElementParser<TNode, string> CountryImg { get; set; }
        IElementParser<TNode, string> ImgUrl { get; set; }

        IElementParser<TNode, string> ProfileUrl { get; set; }
        IElementParser<TNode, int?> ShirtNumber { get; set; }
        IElementParser<TNode, string> PlayerName { get; set; }
        IElementParser<TNode, string> ShortName { get; set; }
        IElementParser<TNode, string> PlayerImgUrl { get; set; }
        IElementParser<TNode, Position?> Position { get; set; }
        IElementParser<TNode, int?> Captain { get; set; }
        IElementParser<TNode, DateTime?> BirthDate { get; set; }
        INationalityParser<TNode> Nationality { get; set; }
        IElementParser<TNode, int?> Height { get; set; }
        IElementParser<TNode, Foot?> PreferredFoot { get; set; }
        IElementParser<TNode, DateTime?> ClubArrivalDate { get; set; }
        IElementParser<TNode, DateTime?> ContractExpirationDate { get; set; }
        IMarketValueParser<TNode> MarketValue { get; set; }
    }
}
