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
        protected IPage<IDomain<TValue>, IElement<TValue>, TValue, TNode> Page { get; set; }

        public string Name { get; set; }
        public IEnumerable<IElementParser<IElement<TValue>, TValue, TNode>> Parsers { get; set; }

        public ElementsSection(string name, IPage<IDomain<TValue>, IElement<TValue>, TValue, TNode> page)
        {
            this.Name = name;
            this.Page = page;
            this.Parsers = new List<IElementParser<IElement<TValue>, TValue, TNode>>();
        }

        public void Parse()
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
                        var e = this.Page.Domain.SetElement(parsedObj);
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
        private bool fetched = false;
        private readonly IDictionary<Link, bool> linksParsed = new Dictionary<Link, bool>();
        protected IPage<IDomain<TValue>, IElement<TValue>, TValue, TNode> Page { get; set; }
        protected IPage<IDomain<TValue>, IElement<TValue>, TValue, TNode> ChildPage { get; set; }
        protected Func<IList<Link>> GetUrls { get; set; }

        public string Name { get; set; }
        public IList<Link> Children { get; set; }

        public ChildsSection(string name, IPage<IDomain<TValue>, IElement<TValue>, TValue, TNode> page)
        {
            this.Name = name;
            this.Page = page;
        }

        public IList<Link> Fetch()
        {
            if (fetched)
            {
                return Children;
            }
            if (!this.Page.Connection.IsConnected)
            {
                throw new Exception("No connection to the page made yet.");
            }

            Children = GetUrls?.Invoke();
            fetched = true;
            Children?.ToList().ForEach(l => linksParsed.Add(l, false));

            return Children;
        }

        public void Parse()
        {
            if (this.Page == null)
            {
                return;
            }
            if (!this.Page.Connection.IsConnected)
            {
                throw new Exception("No connection to the page made yet.");
            }
            if (fetched == false)
            {
                Fetch();
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

                var pageDomain = this.ChildPage.Parse(pageUrl.Url);
                linksParsed[pageUrl] = true;

                this.Page.Domain?.Children.Add(pageDomain);

                Type t = this.ChildPage.Domain.GetType();
                this.ChildPage.Domain = (IDomain<TValue>)Activator.CreateInstance(t);
                this.ChildPage.Connection.Reset();
            }
        }

        public void Parse(IEnumerable<Link> links)
        {
            if (this.Page == null)
            {
                return;
            }
            if (!this.Page.Connection.IsConnected)
            {
                throw new Exception("No connection to the page made yet.");
            }
            if (fetched == false)
            {
                Fetch();
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

                var pageDomain = this.ChildPage.Parse(pageToParse.Url);
                linksParsed[pageToParse] = true;

                this.Page.Domain?.Children.Add(pageDomain);

                Type t = this.ChildPage.Domain.GetType();
                this.ChildPage.Domain = (IDomain<TValue>)Activator.CreateInstance(t);
                this.ChildPage.Connection.Reset();
            }
        }
    }

    public abstract class ChildsSamePageSection<TDomain, TValue, TNode> : IChildsSamePageSection<IElement<TValue>, TValue, TNode> where TDomain : IDomain<TValue>, new() where TValue : IValue
    {
        private IList<List<(TNode key, TNode value)>> childDomainNodes;
        protected Func<IList<List<(TNode key, TNode value)>>> GetChildsNodes { get; set; }
        protected IPage<IDomain<TValue>, IElement<TValue>, TValue, TNode> Page { get; set; }

        public string Name { get; set; }
        public IEnumerable<IElementParser<IElement<TValue>, TValue, TNode>> Parsers { get; set; }

        public ChildsSamePageSection(string name, IPage<IDomain<TValue>, IElement<TValue>, TValue, TNode> page)
        {
            this.Name = name;
            this.Page = page;
        }

        public void Parse()
        {
            childDomainNodes = GetChildsNodes?.Invoke();

            if (childDomainNodes == null || childDomainNodes.Count == 0)
            {
                return;
            }

            foreach (List<(TNode key, TNode value)> childDomainNode in childDomainNodes)
            {
                TDomain childType = new TDomain();
                this.Page.Domain?.Children.Add(childType);

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