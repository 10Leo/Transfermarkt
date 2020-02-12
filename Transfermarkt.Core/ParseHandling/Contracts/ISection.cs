﻿using System;
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
    public interface ISection<TNode, TElement> where TElement : IElement
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="page"></param>
        void Parse(IPage<IDomain, TNode, TElement> page);
    }

    public interface IElementsSection<TNode, TElement> : ISection<TNode, TElement> where TElement : IElement
    {
        /// <summary>
        /// Parsers that parse the Elements of the Section.
        /// </summary>
        IEnumerable<IElementParser<TElement, TNode, object>> Parsers { get; set; }
    }

    public interface IChildsSection<TDomain, TNode, TElement> : ISection<TNode, TElement> where TDomain : IDomain where TElement : IElement
    {
        /// <summary>
        /// A kind of Page that might be accessible from the Section.
        /// </summary>
        IPage<TDomain, TNode, TElement> Page { get; set; }
    }

    public interface IChildsSamePageSection<TNode, TElement> : ISection<TNode, TElement> where TElement : IElement
    {
        /// <summary>
        /// Parsers that parse the Elements of the Section.
        /// </summary>
        IEnumerable<IElementParser<TElement, TNode, object>> Parsers { get; set; }
    }
}
