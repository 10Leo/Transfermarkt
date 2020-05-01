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
    public interface ISection<TElement, TValue, TNode> where TElement : IElement<TValue> where TValue : IValue
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        void Parse(IPage<IDomain<TValue>, TElement, TValue, TNode> page);
    }

    public interface IElementsSection<TElement, TValue, TNode> : ISection<TElement, TValue, TNode> where TElement : IElement<TValue> where TValue : IValue
    {
        /// <summary>
        /// Parsers that parse the Elements of the Section.
        /// </summary>
        IEnumerable<IElementParser<TElement, TValue, TNode>> Parsers { get; set; }
    }

    public interface IChildsSection<TDomain, TElement, TValue, TNode> : ISection<TElement, TValue, TNode> where TDomain : IDomain<TValue> where TElement : IElement<TValue> where TValue : IValue
    {
        /// <summary>
        /// A kind of Page that might be accessible from the Section.
        /// </summary>
        IPage<TDomain, TElement, TValue, TNode> Page { get; set; }

        IList<string> Fetch(IPage<IDomain<TValue>, TElement, TValue, TNode> page);
    }

    public interface IChildsSamePageSection<TElement, TValue, TNode> : ISection<TElement, TValue, TNode> where TElement : IElement<TValue> where TValue : IValue
    {
        /// <summary>
        /// Parsers that parse the Elements of the Section.
        /// </summary>
        IEnumerable<IElementParser<TElement,TValue, TNode>> Parsers { get; set; }
    }
}
