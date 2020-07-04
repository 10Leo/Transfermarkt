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
        protected Func<IList<(TNode key, TNode value)>> GetElementsNodes { get; set; }

        public string Name { get; set; }
        public IEnumerable<IElementParser<IElement<TValue>, TValue, TNode>> Parsers { get; set; }

        public ElementsSection(string name)
        {
            this.Name = name;
            this.Parsers = new List<IElementParser<IElement<TValue>, TValue, TNode>>();
        }

        public void Parse(IPage<IDomain<TValue>, IElement<TValue>, TValue, TNode> page)
        {
            if (Parsers == null)
            {
                return;
            }

            elementsNodes = GetElementsNodes?.Invoke();
            if (elementsNodes == null || elementsNodes.Count == 0)
            {
                return;
            }

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

            Parsers.ToList().ForEach(p =>
            {
                //TODO: recreate an instance of the parser? if yes, relocate this logic to the beginning of this method.
                p.Reset();
            });
        }
    }

    public abstract class ChildsSection<TNode, TValue> : IChildsSection<IDomain<TValue>, IElement<TValue>, TValue, TNode> where TValue : IValue
    {
        private readonly IDictionary<Link, bool> linksParsed = new Dictionary<Link, bool>();
        
        public string Name { get; set; }
        public IList<Link> Children { get; set; }

        protected IPage<IDomain<TValue>, IElement<TValue>, TValue, TNode> Page { get; set; }

        protected Func<IList<Link>> GetUrls { get; set; }
        private bool fetched = false;

        public ChildsSection(string name)
        {
            this.Name = name;
        }

        public IList<Link> Fetch(IPage<IDomain<TValue>, IElement<TValue>, TValue, TNode> page)
        {
            if (fetched)
            {
                return Children;
            }

            Children = GetUrls?.Invoke();
            fetched = true;
            Children?.ToList().ForEach(l => linksParsed.Add(l, false));

            return Children;
        }

        public void Parse(IPage<IDomain<TValue>, IElement<TValue>, TValue, TNode> page)
        {
            if (this.Page == null)
            {
                return;
            }
            if (fetched == false)
            {
                Fetch(page);
            }
            if (Children == null || Children.Count == 0)
            {
                return;
            }

            foreach (var pageUrl in Children)
            {
                if (linksParsed.ContainsKey(pageUrl) && linksParsed[pageUrl])
                {
                    // Page already parsed
                    continue;
                }

                var pageDomain = this.Page.Parse(pageUrl.Url);
                linksParsed[pageUrl] = true;

                page.Domain?.Children.Add(pageDomain);

                Type t = this.Page.Domain.GetType();
                this.Page.Domain = (IDomain<TValue>)Activator.CreateInstance(t);
            }
        }

        public void Parse(IPage<IDomain<TValue>, IElement<TValue>, TValue, TNode> page, IEnumerable<Link> links)
        {
            if (this.Page == null)
            {
                return;
            }
            if (fetched == false)
            {
                Fetch(page);
            }
            if (Children == null || Children.Count == 0)
            {
                return;
            }

            foreach (Link pageUrl in links)
            {
                Link pageToParse = Children.FirstOrDefault(c => c.Equals(pageUrl));
                if (pageToParse == null)
                {
                    continue;
                }

                if (linksParsed.ContainsKey(pageToParse) && linksParsed[pageToParse])
                {
                    // Page already parsed
                    continue;
                }

                var pageDomain = this.Page.Parse(pageToParse.Url);
                linksParsed[pageToParse] = true;

                page.Domain?.Children.Add(pageDomain);

                Type t = this.Page.Domain.GetType();
                this.Page.Domain = (IDomain<TValue>)Activator.CreateInstance(t);
            }
        }
    }

    public abstract class ChildsSamePageSection<TDomain, TValue, TNode> : IChildsSamePageSection<IElement<TValue>, TValue, TNode> where TDomain : IDomain<TValue>, new() where TValue : IValue
    {
        private IList<List<(TNode key, TNode value)>> childDomainNodes;
        protected Func<IList<List<(TNode key, TNode value)>>> GetChildsNodes { get; set; }

        public string Name { get; set; }
        public IEnumerable<IElementParser<IElement<TValue>, TValue, TNode>> Parsers { get; set; }

        public ChildsSamePageSection(string name)
        {
            this.Name = name;
        }

        public void Parse(IPage<IDomain<TValue>, IElement<TValue>, TValue, TNode> page)
        {
            childDomainNodes = GetChildsNodes?.Invoke();

            if (childDomainNodes == null || childDomainNodes.Count == 0)
            {
                return;
            }

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