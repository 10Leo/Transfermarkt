using System.Collections.Generic;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public interface IPage<TDomain, TNode, TValue> where TDomain : IDomain
    {
        TDomain Domain { get; set; }

        IList<IElementParser<TNode, TValue>> Elements { get; set; }

        void Parse();
        void Save();
    }
}
