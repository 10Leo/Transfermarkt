using System.Collections.Generic;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public interface IPage<TDomain, TNode, TElement> where TDomain : IDomain where TElement : IElement
    {
        TDomain Domain { get; set; }

        IReadOnlyList<ISection<TNode, TElement>> Sections { get; set; }

        void Parse();
        void Save();
    }
}
