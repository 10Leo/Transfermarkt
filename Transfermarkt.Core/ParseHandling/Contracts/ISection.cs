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
    public interface ISection<TElement, TNode> where TElement : IElement
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        void Parse(IPage<IDomain, TElement, TNode> page);
    }

    public interface IElementsSection<TElement, TNode> : ISection<TElement, TNode> where TElement : IElement
    {
        /// <summary>
        /// Parsers that parse the Elements of the Section.
        /// </summary>
        IEnumerable<IElementParser<TElement, TNode>> Parsers { get; set; }
    }

    public interface IChildsSection<TDomain, TElement, TNode> : ISection<TElement, TNode> where TDomain : IDomain where TElement : IElement
    {
        /// <summary>
        /// A kind of Page that might be accessible from the Section.
        /// </summary>
        IPage<TDomain, TElement, TNode> Page { get; set; }
    }

    public interface IChildsSamePageSection<TElement, TNode> : ISection<TElement, TNode> where TElement : IElement
    {
        /// <summary>
        /// Parsers that parse the Elements of the Section.
        /// </summary>
        IEnumerable<IElementParser<TElement, TNode>> Parsers { get; set; }
    }
}
