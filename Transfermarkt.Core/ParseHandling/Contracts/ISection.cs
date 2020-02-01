using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    /// <summary>
    /// Section of a Page.
    /// </summary>
    /// <typeparam name="TDomain"></typeparam>
    /// <typeparam name="TNode"></typeparam>
    /// <typeparam name="TElement"></typeparam>
    public interface ISection<TDomain, TNode, TElement> where TDomain : IDomain where TElement : IElement
    {
        /// <summary>
        /// Parsers that parse the Elements of the Section.
        /// </summary>
        IReadOnlyList<IElementParser<TNode, TElement, object>> Parsers { get; set; }

        /// <summary>
        /// A kind of Page that might be accessible from the Section.
        /// </summary>
        IPage<TDomain, TNode, TElement> Page { get; set; }

        /// <summary>
        /// For <see cref="IElement"/>s that migh exist on the current Section.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IElement> ParseElements();

        /// <summary>
        /// For <see cref="IDomain"/>s on different <see cref="IPage{TDomain, TNode, TElement}"/> accessible from the current one.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IDomain> ParseChilds();
    }
}
