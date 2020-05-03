using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    //public abstract class Section<TNode, TValue> : ISection<IElement, TNode>
    //{
    //    public IConnection<TNode> Connection { get; set; }

    //    public Section(IConnection<TNode> connection)
    //    {
    //        this.Connection = connection;
    //    }

    //    public abstract void Parse(IPage<IDomain<TValue>, TNode, IElement<TValue>, TValue> page);
    //}

    public abstract class ElementsSection<TNode, TValue> : IElementsSection<IElement<TValue>, TValue, TNode> where TValue : IValue
    {
        private IList<(TNode key, TNode value)> elementsNodes;
        
        public IEnumerable<IElementParser<IElement<TValue>, TValue, TNode>> Parsers { get; set; }

        public Func<IList<(TNode key, TNode value)>> GetElementsNodes { get; set; }
        
        public ElementsSection()
        {
            this.Parsers = new List<IElementParser<IElement<TValue>, TValue, TNode>>();
        }

        public void Parse(IPage<IDomain<TValue>, IElement<TValue>, TValue, TNode> page)
        {
            if (Parsers != null)
            {
                elementsNodes = GetElementsNodes?.Invoke();

                if (elementsNodes != null && elementsNodes.Count > 0)
                {
                    foreach (var (key, value) in elementsNodes)
                    {
                        foreach (var parser in Parsers)
                        {
                            if (parser.CanParse(key))
                            {
                                var parsedObj = parser.Parse(value);
                                var e = page.Domain.SetElement(parsedObj);
                            }
                        }
                    }
                }

                Parsers.ToList().ForEach(p =>
                {
                    //TODO: recreate an instance of the parser? if yes, relocate this logic to the beginning of this method.
                    p.Reset();
                });
            }
        }
    }

    public abstract class ChildsSection<TNode, TValue> : IChildsSection<IDomain<TValue>, IElement<TValue>, TValue, TNode> where TValue : IValue
    {
        private IList<Link> pagesNodes;
        
        public IPage<IDomain<TValue>, IElement<TValue>, TValue, TNode> Page { get; set; }

        public Func<IList<Link>> GetUrls { get; set; }

        public IList<Link> Fetch(IPage<IDomain<TValue>, IElement<TValue>, TValue, TNode> page)
        {
            pagesNodes = GetUrls?.Invoke();

            return pagesNodes;
        }

        public void Parse(IPage<IDomain<TValue>, IElement<TValue>, TValue, TNode> page)
        {
            if (this.Page != null)
            {
                if (pagesNodes != null && pagesNodes.Count > 0)
                {
                    foreach (var pageUrl in pagesNodes)
                    {
                        var pageDomain = this.Page.Parse(pageUrl.Url);
                        page.Domain?.Children.Add(pageDomain);

                        Type t = this.Page.Domain.GetType();
                        this.Page.Domain = (IDomain<TValue>)Activator.CreateInstance(t);
                    }
                }
            }
        }
    }

    public abstract class ChildsSamePageSection<TDomain, TValue, TNode> : IChildsSamePageSection<IElement<TValue>, TValue, TNode> where TDomain : IDomain<TValue>, new() where TValue : IValue
    {
        private IList<List<(TNode key, TNode value)>> childDomainNodes;

        public IEnumerable<IElementParser<IElement<TValue>, TValue, TNode>> Parsers { get; set; }

        public Func<IList<List<(TNode key, TNode value)>>> GetChildsNodes { get; set; }

        public void Parse(IPage<IDomain<TValue>, IElement<TValue>, TValue, TNode> page)
        {
            {
                childDomainNodes = GetChildsNodes?.Invoke();

                if (childDomainNodes != null && childDomainNodes.Count > 0)
                {
                    foreach (List<(TNode key, TNode value)> childDomainNode in childDomainNodes)
                    {
                        TDomain childType = new TDomain();
                        page.Domain?.Children.Add(childType);

                        if (Parsers != null)
                        {
                            foreach ((TNode key, TNode value) in childDomainNode)
                            {
                                foreach (var parser in Parsers)
                                {
                                    if (parser.CanParse(key))
                                    {
                                        var parsedObj = parser.Parse(value);
                                        var e = childType.SetElement(parsedObj);
                                    }
                                }
                            }
                        }

                        Parsers.ToList().ForEach(p =>
                        {
                            p.Reset();
                        });
                    }
                }
            }
        }
    }
}