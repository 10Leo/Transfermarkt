using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public interface ISection<TDomain, TNode, TElement> where TDomain : IDomain where TElement : IElement
    {
        IReadOnlyList<IElementParser<TNode, TElement, object>> Parsers { get; set; }
        IPage<TDomain, TNode, TElement> Page { get; set; }
    }
}
