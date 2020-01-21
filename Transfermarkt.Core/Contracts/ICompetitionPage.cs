using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Transfermarkt.Core.Actors;

namespace Transfermarkt.Core.Contracts
{
    public interface ICompetitionPage<TNode> : IPage<IDomain>
    {
        IElementParser<TNode, int?> Season { get; set; }
        IElementParser<TNode, Nationality?> Country { get; set; }
        IElementParser<TNode, string> Name { get; set; }
        IElementParser<TNode, string> CountryImg { get; set; }
        IElementParser<TNode, string> ImgUrl { get; set; }

        IClubPage<TNode> Club { get; set; }
    }
}
