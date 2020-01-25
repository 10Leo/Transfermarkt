using System.Collections.Generic;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public interface IPage<TDomain, TNode, TElement> where TDomain : IDomain where TElement : IElement
    {
        TDomain Domain { get; set; }

        IList<IElementParser<TNode, TElement, object>> Elements { get; set; }

        void Parse();
        void Save();
    }
}
