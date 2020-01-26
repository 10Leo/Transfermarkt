using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public interface ISection<TNode, TElement> where TElement : IElement
    {
        IReadOnlyList<IElementParser<TNode, TElement, object>> Elements { get; set; }
    }
}
