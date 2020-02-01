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

        /// <summary>
        /// For <see cref="IDomain"/>s on same <see cref="IPage{TDomain, TNode, TElement}"/>.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IElement> ParseElements();

        /// <summary>
        /// For <see cref="IDomain"/>s on different <see cref="IPage{T}"/> accessible from the current one.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IDomain> ParseUrls();

        /// <summary>
        /// For <see cref="IDomain"/>s on same page.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IDomain> ParseChilds();
    }
}
