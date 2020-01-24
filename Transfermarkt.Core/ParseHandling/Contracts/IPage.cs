using System.Collections.Generic;
using Transfermarkt.Core.Actors;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public interface IPage<TDomain, TNode, TElement> where TDomain : ID where TElement : IElement
    {
        TDomain Domain { get; set; }

        IList<IElementParser<TNode, TElement, object>> Elements { get; set; }

        void Parse();
        void Save();
    }
}
