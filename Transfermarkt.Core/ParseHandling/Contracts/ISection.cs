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
        /// A name that describes the Section.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        void Parse();
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
        IList<Link> Children { get; set; }

        IList<Link> Fetch();
        void Parse(IEnumerable<Link> links);
    }

    public interface IChildsSamePageSection<TElement, TValue, TNode> : ISection<TElement, TValue, TNode> where TElement : IElement<TValue> where TValue : IValue
    {
        /// <summary>
        /// Parsers that parse the Elements of the Section.
        /// </summary>
        IEnumerable<IElementParser<TElement,TValue, TNode>> Parsers { get; set; }
    }
}
