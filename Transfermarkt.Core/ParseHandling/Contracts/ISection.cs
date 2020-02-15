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
    public interface ISection<TNode, TElement, TValue> where TElement : IElement<TValue>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        void Parse(IPage<IDomain<TValue>, TNode, TElement, TValue> page);
    }

    public interface IElementsSection<TNode, TElement, TValue> : ISection<TNode, TElement, TValue> where TElement : IElement<TValue>
    {
        /// <summary>
        /// Parsers that parse the Elements of the Section.
        /// </summary>
        IEnumerable<IElementParser<TElement, TNode, TValue>> Parsers { get; set; }
    }

    public interface IChildsSection<TDomain, TNode, TElement, TValue> : ISection<TNode, TElement, TValue> where TDomain : IDomain<TValue> where TElement : IElement<TValue>
    {
        /// <summary>
        /// A kind of Page that might be accessible from the Section.
        /// </summary>
        IPage<TDomain, TNode, TElement, TValue> Page { get; set; }
    }

    public interface IChildsSamePageSection<TNode, TElement, TValue> : ISection<TNode, TElement, TValue> where TElement : IElement<TValue>
    {
        /// <summary>
        /// Parsers that parse the Elements of the Section.
        /// </summary>
        IEnumerable<IElementParser<TElement, TNode, TValue>> Parsers { get; set; }
    }
}
