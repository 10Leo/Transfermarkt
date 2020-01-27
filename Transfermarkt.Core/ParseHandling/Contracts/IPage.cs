using System.Collections.Generic;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public interface IPage<TDomain, TNode, TElement> where TDomain : IDomain where TElement : IElement
    {
        TDomain Domain { get; set; }

        IReadOnlyList<ISection<TDomain, TNode, TElement>> Sections { get; set; }

        TDomain Parse(string url);
        void Save();
    }
}
