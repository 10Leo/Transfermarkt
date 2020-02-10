using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Transfermarkt.Core.ParseHandling.Contracts
{
    public abstract class Section<TNode> : ISection<IDomain, TNode, IElement>
    {
        public IConnection<TNode> Connection { get; set; }

        public Section(IConnection<TNode> connection)
        {
            this.Connection = connection;
        }

        public abstract void Parse(IPage<IDomain, TNode, IElement> page);
    }

    public abstract class ElementsSection<TNode> : IElementsSection<IDomain, TNode, IElement>
    {
        public IEnumerable<IElementParser<TNode, IElement, object>> Parsers { get; set; }

        public Func<IList<(TNode key, TNode value)>> GetElementsNodes { get; set; }

        public ElementsSection()
        {
            this.Parsers = new List<IElementParser<TNode, IElement, object>>();
        }

        public void Parse(IPage<IDomain, TNode, IElement> page)
        {
            if (Parsers != null)
            {
                IList<(TNode key, TNode value)> elementsNodes = GetElementsNodes?.Invoke();

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
            }
        }
    }

    public abstract class ChildsSection<TNode> : IChildsSection<IDomain, TNode, IElement>
    {
        public IPage<IDomain, TNode, IElement> Page { get; set; }

        public Func<IList<string>> GetUrls { get; set; }

        public void Parse(IPage<IDomain, TNode, IElement> page)
        {
            if (this.Page != null)
            {
                IList<string> pagesNodes = GetUrls?.Invoke();

                if (pagesNodes != null && pagesNodes.Count > 0)
                {
                    foreach (var pageUrl in pagesNodes)
                    {
                        var pageDomain = this.Page.Parse(pageUrl);
                        page.Domain?.Children.Add(pageDomain);

                        Type t = this.Page.Domain.GetType();
                        this.Page.Domain = (IDomain)Activator.CreateInstance(t);
                    }
                }
            }
        }
    }

    public abstract class ChildsSamePageSection<TDomain, TNode> : IChildsSamePageSection<IDomain, TNode, IElement> where TDomain : IDomain, new()
    {
        public IEnumerable<IElementParser<TNode, IElement, object>> Parsers { get; set; }

        public Func<IList<List<(TNode key, TNode value)>>> GetChildsNodes { get; set; }

        public void Parse(IPage<IDomain, TNode, IElement> page)
        {
            {
                IList<List<(TNode key, TNode value)>> childDomainNodes = GetChildsNodes?.Invoke();

                if (childDomainNodes != null && childDomainNodes.Count > 0)
                {
                    foreach (List<(TNode key, TNode value)> childDomainNode in childDomainNodes)
                    {
                        TDomain childType = new TDomain();
                        page.Domain?.Children.Add(childType);

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
                }
            }
        }
    }
}